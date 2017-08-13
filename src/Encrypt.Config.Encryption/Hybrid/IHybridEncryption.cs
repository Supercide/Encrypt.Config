namespace Encrypt.Config.Encryption.Hybrid {
    public interface IHybridEncryption
    {
        (EncryptionSettings key, byte[] encryptedData) EncryptData(byte[] sessionKey, byte[] data, byte[] Iv);
    }

    public interface IHybridDecryption
    {
        byte[] DecryptData(EncryptionSettings encryptionSettings, byte[] data);
    }
}