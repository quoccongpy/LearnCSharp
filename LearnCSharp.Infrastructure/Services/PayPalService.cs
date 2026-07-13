using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.Paypal;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Persistence.ConfigOptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LearnCSharp.Infrastructure.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly PayPalSettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IMemoryCache _cache;

        public PayPalService(HttpClient httpClient, IOptions<PayPalSettings> settings, IUnitOfWork unitOfWork, IExchangeRateService exchangeRateService, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _unitOfWork = unitOfWork;
            _exchangeRateService = exchangeRateService;
            _cache = cache;
        }

        public async Task<PayPalCreateResultDTO> CreatePayPalOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetByIdIncludeAsync(a => a.Id == orderId, includes: a => a.Payments);
            if (order == null)
            {
                throw new ApplicationException("Order not found");
            }
            if (order.Status == SD.PaymentPaid || order.Status == SD.Processing)
            {
                throw new ApplicationException("Order has already been paid.");
            }
            var payment = order.Payments.FirstOrDefault(p => p.PaymentMethod == SD.Paypal && p.PaymentStatus == SD.PaymentPending);
            if (payment == null)
            {
                payment = new Domain.Entities.Payment
                {
                    OrderId = order.Id,
                    PaymentMethod = SD.Paypal,
                    Amount = order.TotalMoney,
                    PaymentStatus = SD.PaymentPending
                };
                await _unitOfWork.Payment.CreateAsync(payment);
                await _unitOfWork.CompleteAsync();
            }
            var amountUsd = await _exchangeRateService.ConvertVndToUsdAsync(order.TotalMoney);
            var accessToken = await GetAccessTokenAsync();

            var requestBody = new PayPalCreateOrderRequest()
            {
                Intent = SD.IntentPaypal,
                PurchaseUnits = new List<PurchaseUnit>
                {
                    new PurchaseUnit
                    {
                        ReferenceId = order.Id.ToString(),
                        Description = $"Thanh toán đơn hàng #{order.Id}",
                        Amount = new Amount
                        {
                            CurrencyCode = _settings.Currency,
                            Value = amountUsd.ToString("F2", CultureInfo.InvariantCulture)
                        }
                    }
                },
                ApplicationContext = new PayPalApplicationContext()
                {
                    ReturnUrl = _settings.ReturnUrl,
                    CancelUrl = _settings.CancelUrl,
                    BrandName = "LearnReactjs Store",
                    LandingPage = "LOGIN",
                    UserAction = "PAY_NOW"
                }
            };
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/checkout/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"PayPal API error ({response.StatusCode}): {errorBody}");
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<PayPalCreateOrderResponse>(json);

            if (result == null)
            {
                throw new ApplicationException("Invalid PayPal create order response.");
            }
            var paypalOrderId = result.Id;
            var approvalUrl = result.Links.FirstOrDefault(x => x.Rel == "approve")?.Href;
            if (string.IsNullOrWhiteSpace(approvalUrl))
            {
                throw new ApplicationException("Approval url not found.");
            }
            payment.GatewayOrderId = paypalOrderId;
            payment.GatewayMetadata = json;
            _unitOfWork.Payment.Update(payment);
            await _unitOfWork.CompleteAsync();
            return new PayPalCreateResultDTO
            {
                OrderId = order.Id,
                PayPalOrderId = paypalOrderId,
                ApprovalUrl = approvalUrl
            };
        }

        public async Task<PayPalCaptureResultDTO> CapturePaymentAsync(string paypalOrderId)
        {
            var payment = await _unitOfWork.Payment.GetByfilterAsync(a => a.GatewayOrderId == paypalOrderId);
            if (payment == null)
            {
                throw new ApplicationException("Payment transaction record not found for this PayPal order.");
            }
            var order = await _unitOfWork.Order.GetByIdAsync(payment.OrderId);
            if (order == null)
            {
                throw new ApplicationException("Associated order not found.");
            }

            if (order.Status == SD.Processing || order.Status == SD.PaymentPaid)
            {
                return new PayPalCaptureResultDTO
                {
                    Success = true,
                    Message = "Order has already been paid and is being processed.",
                    OrderId = order.Id,
                    PaymentMethod = SD.Paypal
                };
            }
            var accessToken = await GetAccessTokenAsync();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/checkout/orders/{paypalOrderId}/capture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                payment.PaymentStatus = SD.PaymentFailed;
                payment.GatewayMetadata = json;

                _unitOfWork.Payment.Update(payment);
                await _unitOfWork.CompleteAsync();

                return new PayPalCaptureResultDTO
                {
                    Success = false,
                    Message = $"PayPal payment capture failed API status ({response.StatusCode}).",
                    OrderId = order.Id,
                    PaymentMethod = SD.Paypal
                };
            }
            var result = JsonSerializer.Deserialize<PayPalCaptureResponse>(json);
            if (result == null)
            {
                throw new ApplicationException("Invalid PayPal capture response.");
            }

            var status = result.Status;
            payment.GatewayMetadata = json;
            if (status == "COMPLETED")
            {
                var capture = result.PurchaseUnits.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault();
                if (capture == null)
                {
                    throw new ApplicationException("Capture information not found.");
                }
                payment.PaymentStatus = SD.PaymentPaid;
                payment.GatewayTransactionId = capture.Id;
                payment.PaymentDate = DateTime.UtcNow;
                order.Status = SD.Processing;

                _unitOfWork.Payment.Update(payment);
                _unitOfWork.Order.Update(order);
                await _unitOfWork.CompleteAsync();
                var finalAmount = decimal.Parse(capture.Amount.Value, CultureInfo.InvariantCulture);

                return new PayPalCaptureResultDTO
                {
                    Success = true,
                    Message = "Payment completed successfully.",
                    OrderId = order.Id,
                    TransactionId = capture.Id,
                    PaymentMethod = SD.Paypal,
                    Amount = finalAmount
                };
            }
            else
            {
                payment.PaymentStatus = SD.PaymentFailed;
                _unitOfWork.Payment.Update(payment);
                await _unitOfWork.CompleteAsync();

                return new PayPalCaptureResultDTO
                {
                    Success = false,
                    Message = $"Payment not completed. Status: {status}",
                    OrderId = order.Id,
                    PaymentMethod = SD.Paypal
                };
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (_cache.TryGetValue(SD.CacheKeyPaypal, out string token))
            {
                return token;
            }

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.ClientId}:{_settings.SecretKey}"));
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var tokenResponse = await response.Content.ReadFromJsonAsync<PayPalTokenResponse>();

            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                throw new ApplicationException("Cannot get PayPal access token.");
            }
            _cache.Set(SD.CacheKeyPaypal, tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 300));
            return tokenResponse.AccessToken;
        }
    }
}