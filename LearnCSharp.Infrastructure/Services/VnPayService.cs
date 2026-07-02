using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.VNPay;
using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using LearnCSharp.Infrastructure.Payments.VNPay;
using LearnCSharp.Infrastructure.Persistence.ConfigOptions;
using Microsoft.Extensions.Options;

namespace LearnCSharp.Infrastructure.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _vnPaySettings;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(IOptions<VnPaySettings> vnPaySettings, IUnitOfWork unitOfWork)
        {
            _vnPaySettings = vnPaySettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreatePaymentUrl(int orderId, string ipAddress)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ApplicationException("Order not found.");
            }
            if (order.PaymentStatus == SD.PaymentPaid)
            {
                throw new ApplicationException("Order has already been paid.");
            }
            var vnPay = new VnPayLibrary();
            vnPay.AddRequestData("vnp_Version", _vnPaySettings.Version);
            vnPay.AddRequestData("vnp_Command", _vnPaySettings.Command);
            vnPay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
            vnPay.AddRequestData("vnp_Amount", (order.TotalMoney * 100).ToString());
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", _vnPaySettings.CurrCode);
            //vnPay.AddRequestData("vnp_IpAddr", KeySecurity.GetIpAddress(context));
            vnPay.AddRequestData("vnp_IpAddr", ipAddress);
            vnPay.AddRequestData("vnp_Locale", _vnPaySettings.Locale);
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {orderId}");
            vnPay.AddRequestData("vnp_OrderType", SD.OrderType);
            vnPay.AddRequestData("vnp_ReturnUrl", _vnPaySettings.ReturnUrl);
            vnPay.AddRequestData("vnp_TxnRef", order.Id.ToString());
            vnPay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            var paymentUrl = vnPay.CreateRequestUrl(_vnPaySettings.BaseUrl, _vnPaySettings.HashSecret);
            order.PaymentStatus = SD.PaymentPending;
            order.PaymentMethod = SD.VNPay.ToUpper();
            _unitOfWork.Order.Update(order);
            await _unitOfWork.CompleteAsync();
            Console.WriteLine(paymentUrl);
            return paymentUrl;
        }

        public PaymentResultDTO PaymentExecute(Dictionary<string, string> collections)
        {
            var vnPay = BuildVnPayResponse(collections);
            if (!collections.TryGetValue("vnp_SecureHash", out var secureHash) || !IsValidSignature(vnPay, collections))
            {
                return new PaymentResultDTO { Success = false, Message = "Invalid signature." };
            }
            if (!long.TryParse(vnPay.GetResponseData("vnp_TxnRef"), out var orderId))
            {
                return new PaymentResultDTO
                {
                    Success = false,
                    Message = "Invalid order id."
                };
            }
            var transactionId = vnPay.GetResponseData("vnp_TransactionNo");
            var responseCode = vnPay.GetResponseData("vnp_ResponseCode");
            var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");
            var amountInfo = vnPay.GetResponseData("vnp_Amount");

            return new PaymentResultDTO
            {
                Success = responseCode == "00",
                OrderId = orderId,
                TransactionId = transactionId,
                PaymentMethod = SD.VNPay,
                OrderDescription = orderInfo,
                VnPayResponseCode = responseCode,
                Amount= decimal.Parse(amountInfo) / 100,
                Message = responseCode == "00" ? "Payment successful." : "Payment failed."
            };
        }

        public async Task<IpnResponseDTO> ProcessIpnAsync(Dictionary<string, string> query)
        {
            var vnPay = BuildVnPayResponse(query);

            if (!IsValidSignature(vnPay, query))
            {
                return CreateIpnResponse("97", "Invalid signature");
            }

            var order = await GetOrderAsync(vnPay);

            if (order == null)
            {
                return CreateIpnResponse("01", "Order not found");
            }

            if (!ValidateAmount(vnPay, order))
            {
                return CreateIpnResponse("04", "Invalid amount");
            }

            if (order.PaymentStatus == SD.PaymentPaid)
            {
                return CreateIpnResponse("02", "Order already confirmed");
            }

            await UpdateOrderAsync(vnPay, order);

            return CreateIpnResponse("00", "Confirm Success");
        }

        private VnPayLibrary BuildVnPayResponse(IDictionary<string, string> query)
        {
            var vnPay = new VnPayLibrary();

            foreach (var (key, value) in query)
            {
                if (key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value);
                }
            }
            return vnPay;
        }

        private bool IsValidSignature(VnPayLibrary vnPay, IDictionary<string, string> query)
        {
            if (!query.TryGetValue("vnp_SecureHash", out var secureHash))
            {
                return false;
            }
            return vnPay.ValidateSignature(secureHash, _vnPaySettings.HashSecret);
        }

        private async Task<Order> GetOrderAsync(VnPayLibrary vnPay)
        {
            if (!int.TryParse(vnPay.GetResponseData("vnp_TxnRef"), out var orderId))
            {
                return null;
            }
            return await _unitOfWork.Order.GetByIdAsync(orderId);
        }

        private bool ValidateAmount(VnPayLibrary vnPay, Order order)
        {
            if (!long.TryParse(vnPay.GetResponseData("vnp_Amount"),
                out var amount))
            {
                return false;
            }
            var expectedAmount = Convert.ToInt64(order.TotalMoney * 100);
            return amount == expectedAmount;
        }

        private async Task UpdateOrderAsync(VnPayLibrary vnPay, Order order)
        {
            var responseCode = vnPay.GetResponseData("vnp_ResponseCode");

            var transactionStatus = vnPay.GetResponseData("vnp_TransactionStatus");

            var transactionNo = vnPay.GetResponseData("vnp_TransactionNo");
            order.PaymentTransactionId = transactionNo;

            if (responseCode == "00" && transactionStatus == "00")
            {
                order.PaymentStatus = SD.PaymentPaid;
                order.Status = SD.Processing;
            }
            else
            {
                order.PaymentStatus = SD.PaymentFailed;
            }
            _unitOfWork.Order.Update(order);
            await _unitOfWork.CompleteAsync();
        }

        private IpnResponseDTO CreateIpnResponse(string rspCode, string message)
        {
            return new IpnResponseDTO
            {
                RspCode = rspCode,
                Message = message
            };
        }
    }
}