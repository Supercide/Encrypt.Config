using System.Security.Cryptography;

namespace Encrypt.Config.Encryption.Random
{
    public class RNGCryptoRandomBytesGenerator : IGenerateRandomBytes
    {
        public byte[] Generate(int length)
        {
            using (var rand = new RNGCryptoServiceProvider())
            {
                var randomBytesbuffer = new byte[length];

                rand.GetBytes(randomBytesbuffer);

                return randomBytesbuffer;
            }
        }
    }
}
