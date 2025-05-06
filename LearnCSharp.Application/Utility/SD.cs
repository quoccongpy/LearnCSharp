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
    }
}