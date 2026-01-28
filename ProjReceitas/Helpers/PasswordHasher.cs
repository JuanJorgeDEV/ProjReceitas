using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjReceitas.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "A senha não pode ser nula ou vazia.");
            }

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "A senha fornecida não pode ser nula ou vazia.");
            }
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentNullException(nameof(hashedPassword), "O hash da senha armazenada não pode ser nulo ou vazio.");
            }

            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                if (hashBytes.Length < SaltSize + HashSize)
                {
                    return false;
                }

                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(HashSize);

                uint diff = (uint)hashBytes.Length ^ (uint)(salt.Length + hash.Length);
                for (int i = 0; i < HashSize && i < (hashBytes.Length - SaltSize); i++)
                {
                    diff |= (uint)(hashBytes[i + SaltSize] ^ hash[i]);
                }
                return diff == 0;
            }
            catch (FormatException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}