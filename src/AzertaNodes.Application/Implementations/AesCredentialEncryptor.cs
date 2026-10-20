using AzertaNodes.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using AzertaNodes.Core.Entities;

namespace AzertaNodes.Application.Implementations
{
    public class AesCredentialEncryptor : ICredentialEncryptor
    {
        private readonly byte[] key;

        public AesCredentialEncryptor(IConfiguration configuration)
        {
            var rawKey = configuration["Encryption:MasterKey"];
            if (string.IsNullOrWhiteSpace(rawKey)) throw new InvalidOperationException("Master key for encryption is missing in configuration.");
            key = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
        }

        public (byte[] EncryptedValue, byte[] IV) Encrypt(string plainText)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(plainText);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8, leaveOpen: false))
            {
                sw.Write(plainText);
            }

            return (ms.ToArray(), aes.IV);
        }

        public string Decrypt(byte[] encryptedValue, byte[] iv)
        {
            ArgumentNullException.ThrowIfNull(encryptedValue);
            ArgumentNullException.ThrowIfNull(iv);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            try
            {
                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(encryptedValue);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException(UserErrorMessages.FailedToDecryptProvidedData, ex);
            }
        }
    }
}