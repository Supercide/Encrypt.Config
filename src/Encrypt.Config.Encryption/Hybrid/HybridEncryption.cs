using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.Encryption.Hybrid {
    // TODO: Sign data with private key

    public class HybridEncryption : IHybridEncryption
    {
        private readonly IAsymmetricKeyEncryption _asymmetricKeyEncryption;
        private readonly ISymmetricKeyEncryption _symmetricKeyEncryption;
        private readonly IDigitalSignature _digitalSignature;

        protected HybridEncryption(IAsymmetricKeyEncryption asymmetricKeyEncryption, 
                                ISymmetricKeyEncryption symmetricKeyEncryption, 
                                IDigitalSignature digitalSignature)
        {
            _asymmetricKeyEncryption = asymmetricKeyEncryption;
            _symmetricKeyEncryption = symmetricKeyEncryption;
            _digitalSignature = digitalSignature;
        }

        public (EncryptionSettings key, byte[] encryptedData) EncryptData(byte[] sessionKey, byte[] data, byte[] Iv)
        {
            var encryptedData = _symmetricKeyEncryption.Encrypt(data, sessionKey, Iv);

            var encryptedSessionKey = _asymmetricKeyEncryption.EncryptData(sessionKey);

            byte[] hmacHash;

            using (var hmac = new HMACSHA256(sessionKey))
            {
                hmacHash = hmac.ComputeHash(encryptedData);
            }

            var signature = _digitalSignature.SignData(hmacHash);

            return (new EncryptionSettings(encryptedSessionKey, Iv, hmacHash, signature), encryptedData);
        }

        public static HybridEncryption CreateEncryption(string targetsPublicKey, string signatureContainer)
        {
            return new HybridEncryption(RSAEncryption.FromPublicKey(targetsPublicKey),
                                        new AESEncryption(), 
                                        RSAEncryption.FromExistingContainer(signatureContainer));
        }
        

        
    }
}