using System;
using System.Security.Cryptography;
namespace Hastane_Proje
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 128)
                throw new ArgumentException("Şifre 8 ile 128 karakter arasında olmalıdır.");
            var salt = new byte[16];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(salt);
            using (var key = new Rfc2898DeriveBytes(password, salt, 600000, HashAlgorithmName.SHA256))
                return "PBKDF2$600000$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(key.GetBytes(32));
        }
        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || password.Length > 128 || string.IsNullOrEmpty(stored)) return false;
            var parts = stored.Split('$');
            int iterations;
            if (parts.Length != 4 || parts[0] != "PBKDF2" || !int.TryParse(parts[1], out iterations) || iterations < 600000 || iterations > 2000000) return false;
            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                if (salt.Length != 16 || expected.Length != 32) return false;
                using (var key = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    var actual = key.GetBytes(expected.Length);
                    int difference = 0;
                    for (int i = 0; i < actual.Length; i++) difference |= actual[i] ^ expected[i];
                    return difference == 0;
                }
            }
            catch (FormatException) { return false; }
        }
    }
}
