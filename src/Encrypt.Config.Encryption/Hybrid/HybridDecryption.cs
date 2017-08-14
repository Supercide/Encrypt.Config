using System.Security.Cryptography;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.Encryption.Hybrid {
    public class HybridDecryption : IHybridDecryption
    {
        private readonly IAsymmetricKeyEncryption _asymmetricKeyEncryption;
        private readonly ISymmetricKeyEncryption _symmetricKeyEncryption;
        private readonly IDigitalSignature _digitalSignature;

        protected HybridDecryption(IAsymmetricKeyEncryption asymmetricKeyEncryption,
                                   ISymmetricKeyEncryption symmetricKeyEncryption,
                                   IDigitalSignature digitalSignature)
        {
            _asymmetricKeyEncryption = asymmetricKeyEncryption;
            _symmetricKeyEncryption = symmetricKeyEncryption;
            _digitalSignature = digitalSignature;
        }

        public byte[] DecryptData(EncryptionSettings encryptionSettings, byte[] data)
        {
            var decryptedSessionKey = _asymmetricKeyEncryption.DecryptData(encryptionSettings.SessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(data);

                if (!Compare(encryptionSettings.HMACHash, hmacToCheck))
                {
                    throw new CryptographicException("HMAC signatures do not match");
                }

                if (!_digitalSignature.VerifyData(encryptionSettings.HMACHash, encryptionSettings.Signature))
                {
                    throw new CryptographicException("Signatures cannot be verified");
                }
            }

            var decryptedData = _symmetricKeyEncryption.Decrypt(data, decryptedSessionKey, encryptionSettings.IV);

            return decryptedData;
        }

        public static HybridDecryption CreateDecryption(string decryptionContainerName, string signiturePublicKey)
        {
            return new HybridDecryption(RSAEncryption.FromExistingContainer(decryptionContainerName), 
                                        new AESEncryption(), 
                                        RSAEncryption.FromPublicKey(signiturePublicKey));
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