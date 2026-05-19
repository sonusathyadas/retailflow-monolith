using System;
using System.Security.Cryptography;

namespace RetailFlow.Shared.Helpers
{
    public static class TokenHelper
    {
        /// <summary>
        /// Generates a cryptographically secure random refresh token.
        /// </summary>
        public static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
