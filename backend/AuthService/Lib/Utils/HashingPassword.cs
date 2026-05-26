using System.Security.Cryptography;

namespace AuthService.Lib.Utils
{
    public static class HashingPassword
    {
        public static string ComputeHash(string password)
        {
            int iterations = 100; // Количество итераций для PBKDF2
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return $"{iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string hash)
        {
            // Получаем старый хэш и соль из строки
            string[] parts = hash.Split(':');
            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHash = Convert.FromBase64String(parts[2]);

            // Вычисляем новый хэш с использованием старой соли и количества итераций
            using Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
    }
}
