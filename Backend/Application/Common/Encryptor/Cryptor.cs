using System.Security.Cryptography;

namespace CookBook.Application.Common.Encryptor
{
    public interface ICryptor
    {
        string Encrypt(string plainText);
        bool VerifyEncrypt(string cipherText, string password);
    }
    public class Cryptor : ICryptor
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; //256 bits

        #region Encrypt
        public string Encrypt(string plainText)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a key from the password and salt
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Combine the salt and hash
            byte[] hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            // Convert the byte array to a string
            string cipherText = Convert.ToBase64String(hashBytes);

            return cipherText;
        }
        #endregion

        #region Verify
        public bool VerifyEncrypt(string cipherText, string password)
        {
            // Extract the salt and hash from the stored hashed password
            byte[] hashBytes = Convert.FromBase64String(cipherText);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Derive a key from the password and salt
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[SaltSize + i] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
