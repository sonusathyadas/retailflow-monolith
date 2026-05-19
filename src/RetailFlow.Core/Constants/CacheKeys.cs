namespace RetailFlow.Core.Constants
{
    /// <summary>
    /// Redis cache key prefixes — centralised to prevent key collisions.
    /// </summary>
    public static class CacheKeys
    {
        public const string ProductPrefix  = "product:";
        public const string CartPrefix     = "cart:";
        public const string SessionPrefix  = "session:";

        public static string Product(string id)  => ProductPrefix + id;
        public static string Cart(int userId)    => CartPrefix + userId;
        public static string Session(int userId) => SessionPrefix + userId;
    }
}
