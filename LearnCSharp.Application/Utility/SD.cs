namespace LearnCSharp.Application.Utility
{
    public static class SD
    {
        #region Role

        public const string RoleCustomer = "Customer";
        public const string RoleAdmin = "Admin";

        #endregion Role

        #region OrderStatus

        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";

        #endregion OrderStatus

        #region PaymentStatus

        public const string PaymentPending = "Pending";
        public const string PaymenPaid = "Paid";
        public const string PaymenFailed = "Failed";
        public const string PaymenRefunded = "Refunded";

        #endregion PaymentStatus

        #region StripeMetadataKeys

        public const string StripeMetadataKeysOrderId = "orderId";
        public const string StripeMetadataKeysUserId = "userId";

        #endregion StripeMetadataKeys

        #region cache

        public const string CategoriesAll = "categories:all";

        #endregion cache
    }
}