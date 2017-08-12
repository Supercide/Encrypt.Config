namespace Encrypt.Config.Encryption.Asymmetric {
    public interface IHybridEncryption
    {
        (EncryptionKey key, byte[] encryptedData) EncryptData(byte[] sessionKey, byte[] data, byte[] Iv);

        byte[] DecryptData(EncryptionKey encryptionKey, byte[] data);
    }
}