namespace Encrypt.Config.Encryption.Random {
    public interface IGenerateRandomBytes
    {
        byte[] Generate(int length);
    }
}