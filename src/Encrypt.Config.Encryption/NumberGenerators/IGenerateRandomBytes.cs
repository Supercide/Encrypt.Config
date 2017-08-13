namespace Encrypt.Config.Encryption.NumberGenerators {
    public interface IGenerateRandomBytes
    {
        byte[] Generate(int length);
    }
}