using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Payment;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Persistence.ConfigOptions;
using Microsoft.Extensions.Options;
using Stripe;

namespace LearnCSharp.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _stripeSettings;
        private readonly IExchangeRateService _exchangeRateService;

        public PaymentService(IUnitOfWork unitOfWork, IOptions<StripeSettings> stripeSettings, IExchangeRateService exchangeRateService)
        {
            _unitOfWork = unitOfWork;
            _stripeSettings = stripeSettings.Value;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<PaymentIntentResultDTO> CreatePaymentIntentAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(orderId);
            var paymentIntentService = new PaymentIntentService();
            if (order == null)
            {
                throw new ApplicationException("Order not found");
            }
            if (order.PaymentStatus == SD.PaymenPaid)
            {
                throw new ApplicationException("Order has already been paid.");
            }

            if (!string.IsNullOrEmpty(order.PaymentIntentId))
            {
                var existingIntent =await paymentIntentService.GetAsync(order.PaymentIntentId);

                switch (existingIntent.Status)
                {
                    case "requires_payment_method":
                    case "requires_confirmation":
                    case "requires_action":

                        return new PaymentIntentResultDTO
                        {
                            OrderId = order.Id,
                            ClientSecret = existingIntent.ClientSecret,
                            PublishableKey = _stripeSettings.PublicableKey
                        };

                    case "processing":
                        throw new ApplicationException("Payment is processing.");
                    case "succeeded":
                        order.PaymentStatus = SD.PaymenPaid;
                        _unitOfWork.Order.Update(order);
                        await _unitOfWork.CompleteAsync();
                        throw new InvalidOperationException("Order has already been paid and settled.");
                    case "canceled":
                        order.PaymentIntentId = null;
                        break;
                }
            }
            var amount = await _exchangeRateService.ConvertVndToUsdAsync(order.TotalMoney);
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)Math.Round(amount * 100),
                Currency = _stripeSettings.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { SD.StripeMetadataKeysOrderId, order.Id.ToString() },
                    { SD.StripeMetadataKeysUserId, order.UserId.ToString() }
                },
            };
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = $"payment-{order.Id}"
            };
            var paymentIntent = await paymentIntentService.CreateAsync(options, requestOptions);
            order.PaymentIntentId = paymentIntent.Id;
            order.PaymentStatus = SD.PaymentPending;
            order.Status = SD.PaymentPending;
            _unitOfWork.Order.Update(order);
            await _unitOfWork.CompleteAsync();
            return new PaymentIntentResultDTO
            {
                OrderId = order.Id,
                ClientSecret = paymentIntent.ClientSecret,
                PublishableKey = _stripeSettings.PublicableKey
            };
        }

        public async Task HandleWebhookAsync(string json, string stripeSignature)
        {
            var stripeEvent = EventUtility.ConstructEvent(json,stripeSignature,_stripeSettings.WebhookSecret );
            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    paymentIntent.Metadata.TryGetValue(SD.StripeMetadataKeysOrderId, out var orderIdStr);
                    int.TryParse(orderIdStr, out int orderId);
                    var order = await _unitOfWork.Order.GetByIdAsync(orderId);
                    if (order.PaymentStatus == SD.PaymenPaid)
                    {
                        return;
                    }
                    if (order != null)
                    {
                        order.PaymentStatus = SD.PaymenPaid;
                        order.Status = SD.Processing;
                        _unitOfWork.Order.Update(order);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
            else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    paymentIntent.Metadata.TryGetValue(SD.StripeMetadataKeysOrderId, out var orderIdStr);
                    int.TryParse(orderIdStr, out int orderId);
                    var order = await _unitOfWork.Order.GetByIdAsync(orderId);
                    if (order != null)
                    {
                        order.PaymentStatus = SD.PaymenFailed;
                        _unitOfWork.Order.Update(order);
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
        }
    }
}