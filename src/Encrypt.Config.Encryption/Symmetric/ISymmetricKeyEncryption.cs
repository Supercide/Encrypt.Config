namespace Encrypt.Config.Encryption {
    public interface ISymmetricKeyEncryption
    {
        byte[] Encrypt(byte[] data, byte[] key, byte[] iv);

        byte[] Decrypt(byte[] data, byte[] key, byte[] iv);
    }
}