using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Lib.Utils
{
    public static class Hashing
    {
        public static string ComputeHash(string token, string secret)
        {
            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(secret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }

        public static byte[] ComputeHashBytes(string token, string secret)
        {
            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(secret));
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        }
    }
}
