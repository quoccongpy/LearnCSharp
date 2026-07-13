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
            var order = await _unitOfWork.Order.GetByIdIncludeAsync(a => a.Id == orderId, includes: a => a.Payments);
            var paymentIntentService = new PaymentIntentService();
            if (order == null)
            {
                throw new ApplicationException("Order not found");
            }
            if (order.Status == SD.PaymentPaid)
            {
                throw new ApplicationException("This order has already been paid and processed.");
            }
            var payment = order.Payments.FirstOrDefault(a => a.PaymentMethod == SD.Stripe && a.PaymentStatus == SD.PaymentPending);
            if (payment == null)
            {
                payment = new Domain.Entities.Payment
                {
                    OrderId = order.Id,
                    PaymentMethod = SD.Stripe,
                    Amount = order.TotalMoney,
                    PaymentStatus = SD.PaymentPending
                };

                await _unitOfWork.Payment.CreateAsync(payment);
                await _unitOfWork.CompleteAsync();
            }
            if (!string.IsNullOrEmpty(payment.GatewayOrderId))
            {
                var existingIntent = await paymentIntentService.GetAsync(payment.GatewayOrderId);

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
                        payment.PaymentStatus = SD.PaymentPaid;
                        _unitOfWork.Payment.Update(payment);
                        await _unitOfWork.CompleteAsync();
                        throw new InvalidOperationException("Order has already been paid and settled.");
                    case "canceled":
                        payment.GatewayOrderId = null;
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
                    { SD.StripeMetadataKeysUserId, order.UserId.ToString() },
                    { SD.StripeMetadataKeysPaymentId, payment.Id.ToString() },
                },
            };
            var requestOptions = new RequestOptions
            {
                IdempotencyKey = $"payment-{order.Id}"
            };
            var paymentIntent = await paymentIntentService.CreateAsync(options, requestOptions);
            payment.GatewayOrderId = paymentIntent.Id;
            payment.PaymentStatus = SD.PaymentPending;
            payment.GatewayMetadata = paymentIntent.RawJObject?.ToString();
            _unitOfWork.Order.Update(order);
            _unitOfWork.Payment.Update(payment);
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
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _stripeSettings.WebhookSecret);

                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent == null) return;
                    paymentIntent.Metadata.TryGetValue(SD.StripeMetadataKeysOrderId, out var orderIdStr);
                    paymentIntent.Metadata.TryGetValue(SD.StripeMetadataKeysPaymentId, out var paymentIdStr);

                    if (int.TryParse(orderIdStr, out int orderId) && int.TryParse(paymentIdStr, out int paymentId))
                    {
                        var order = await _unitOfWork.Order.GetByIdIncludeAsync(a => a.Id == orderId, includes: a => a.Payments);
                        if (order == null) return;
                        if (order.Status == SD.Processing || order.Status == SD.PaymentPaid)
                        {
                            return;
                        }
                        var payment = order.Payments.FirstOrDefault(p => p.Id == paymentId);
                        if (payment != null)
                        {
                            payment.PaymentStatus = SD.PaymentPaid;
                            payment.PaymentDate = DateTime.UtcNow;
                            payment.GatewayTransactionId = paymentIntent.LatestChargeId;
                            payment.GatewayMetadata = paymentIntent.RawJObject?.ToString();
                            _unitOfWork.Payment.Update(payment);
                        }
                        order.Status = SD.Processing;
                        _unitOfWork.Order.Update(order);
                        await _unitOfWork.CompleteAsync();
                    }
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent == null) return;

                    paymentIntent.Metadata.TryGetValue(SD.StripeMetadataKeysOrderId, out var orderIdStr);
                    paymentIntent.Metadata.TryGetValue(SD.StripeMetadataKeysPaymentId, out var paymentIdStr);

                    if (int.TryParse(orderIdStr, out int orderId) && int.TryParse(paymentIdStr, out int paymentId))
                    {
                        var order = await _unitOfWork.Order.GetByIdIncludeAsync(a => a.Id == orderId, includes: a => a.Payments);
                        if (order == null) return;

                        var payment = order.Payments.FirstOrDefault(p => p.Id == paymentId);
                        if (payment != null)
                        {
                            payment.PaymentStatus = SD.PaymentFailed;
                            payment.GatewayMetadata = paymentIntent.RawJObject?.ToString();

                            _unitOfWork.Payment.Update(payment);
                        }

                        await _unitOfWork.CompleteAsync();
                    }
                }
            }
            catch (StripeException ex)
            {
                throw new ApplicationException("Webhook signature verification failed", ex);
            }
        }
    }
}