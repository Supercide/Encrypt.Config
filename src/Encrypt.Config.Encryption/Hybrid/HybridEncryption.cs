using System.Security.Cryptography;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.Encryption.Hybrid {
    public class HybridEncryption : IHybridEncryption
    {
        private readonly IAsymmetricKeyEncryption _asymmetricKeyEncryption;
        private readonly ISymmetricKeyEncryption _symmetricKeyEncryption;

        public HybridEncryption(IAsymmetricKeyEncryption asymmetricKeyEncryption, ISymmetricKeyEncryption symmetricKeyEncryption)
        {
            _asymmetricKeyEncryption = asymmetricKeyEncryption;
            _symmetricKeyEncryption = symmetricKeyEncryption;
        }

        public (EncryptionKey key, byte[] encryptedData) EncryptData(byte[] sessionKey, byte[] data, byte[] Iv)
        {
            var encryptedData = _symmetricKeyEncryption.Encrypt(data, sessionKey, Iv);

            var encryptedSessionKey = _asymmetricKeyEncryption.EncryptData(sessionKey);
            byte[] hmacHash;
            using (var hmac = new HMACSHA256(sessionKey))
            {
                hmacHash = hmac.ComputeHash(encryptedData);
            }

            return (new EncryptionKey(encryptedSessionKey, Iv, hmacHash), encryptedData);
        }

        public byte[] DecryptData(EncryptionKey encryptionKey, byte[] data)
        {
            var decryptedSessionKey = _asymmetricKeyEncryption.DecryptData(encryptionKey.SessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(data);

                if (!Compare(encryptionKey.HMACHash, hmacToCheck))
                {
                    throw new CryptographicException("HMAC signatures do not match");
                }
            }

            var decryptedData = _symmetricKeyEncryption.Decrypt(data, decryptedSessionKey, encryptionKey.IV);

            return decryptedData;
        }

        private static bool Compare(byte[] array1, byte[] array2)
        {
            var result = array1.Length == array2.Length;

            for (var i = 0; i < array1.Length && i < array2.Length; ++i)
            {
                result &= array1[i] == array2[i];
            }

            return result;
        }
    }
}