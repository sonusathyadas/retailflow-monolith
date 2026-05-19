namespace RetailFlow.Core.Constants
{
    /// <summary>
    /// Centralised role name constants — avoids magic strings in [Authorize] attributes.
    /// </summary>
    public static class RoleNames
    {
        public const string Customer         = "Customer";
        public const string Admin            = "Admin";
        public const string WarehouseManager = "WarehouseManager";
        public const string FinanceManager   = "FinanceManager";
    }
}
