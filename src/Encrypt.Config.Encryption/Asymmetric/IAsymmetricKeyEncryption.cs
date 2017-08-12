namespace Encrypt.Config.Encryption.Asymmetric {
    public interface IAsymmetricKeyEncryption
    {
        string ExportKey(bool includePrivate);

        byte[] EncryptData(byte[] data);

        byte[] DecryptData(byte[] data);
    }
}