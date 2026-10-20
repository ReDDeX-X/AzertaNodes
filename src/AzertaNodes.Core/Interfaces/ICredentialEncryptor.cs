namespace AzertaNodes.Core.Interfaces
{
    public interface ICredentialEncryptor
    {
        (byte[] EncryptedValue, byte[] IV) Encrypt(string plainText);
        string Decrypt(byte[] encryptedValue, byte[] iv);
    }
}
