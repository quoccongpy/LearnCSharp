namespace LearnCSharp.Application.Utility
{
    public static class SD
    {
        #region Role

        public const string RoleCustomer = "Customer";
        public const string RoleAdmin = "Admin";

        #endregion Role

        #region OrderStatus

        public const string Pending = "Pending";                       // Chờ cửa hàng xác nhận
        public const string Confirmed = "Confirmed";                   // Cửa hàng đã xác nhận
        public const string Preparing = "Preparing";                   // Đang chuẩn bị món
        public const string ReadyForPickup = "ReadyForPickup";         // Đã chuẩn bị xong, chờ shipper
        public const string Delivering = "Delivering";                 // Đang giao hàng
        public const string Delivered = "Delivered";                   // Đã giao thành công
        public const string Cancelled = "Cancelled";                   // Khách hủy
        public const string Rejected = "Rejected";                     // Cửa hàng từ chối

        #endregion OrderStatus

        #region PaymentStatus

        public const string PaymentPending = "Pending";
        public const string PaymentPaid = "Paid";
        public const string PaymentFailed = "Failed";
        public const string PaymenRefunded = "Refunded";

        #endregion PaymentStatus

        #region StripeMetadataKeys

        public const string StripeMetadataKeysOrderId = "orderId";
        public const string StripeMetadataKeysUserId = "userId";
        public const string StripeMetadataKeysPaymentId = "paymentId";

        #endregion StripeMetadataKeys

        #region TypePayment

        public const string OrderType = "other";
        public const string VNPay = "vnpay";
        public const string Stripe = "stripe";
        public const string Paypal = "paypal";

        #endregion TypePayment

        #region cache

        public const string CategoriesAll = "categories:all";
        public const string CacheKeyPaypal = "PAYPAL_ACCESS_TOKEN";

        #endregion cache

        #region PaypalPayment

        public const string IntentPaypal = "CAPTURE";

        #endregion PaypalPayment
    }
}