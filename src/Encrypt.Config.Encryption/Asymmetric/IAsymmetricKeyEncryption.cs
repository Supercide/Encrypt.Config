using System.Security.Cryptography;
using Encrypt.Config.Encryption.RSA;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.Encryption.Asymmetric {
    public interface IAsymmetricKeyEncryption
    {
        string ExportKey(bool includePrivate);

        byte[] EncryptData(byte[] data, string publicKey);

        byte[] DecryptData(byte[] data);
    }

    public interface IHybridEncryption
    {
        EncryptedData EncryptData(byte[] sessionKey, byte[] data, byte[] Iv, string publicKey);

        byte[] DecryptData(EncryptedData encryptedData);
    }

    public class HybridEncryption : IHybridEncryption
    {
        private readonly IAsymmetricKeyEncryption _asymmetricKeyEncryption;
        private readonly ISymmetricKeyEncryption _symmetricKeyEncryption;

        public HybridEncryption(IAsymmetricKeyEncryption asymmetricKeyEncryption, ISymmetricKeyEncryption symmetricKeyEncryption)
        {
            _asymmetricKeyEncryption = asymmetricKeyEncryption;
            _symmetricKeyEncryption = symmetricKeyEncryption;
        }

        public EncryptedData EncryptData(byte[] sessionKey, byte[] data, byte[] Iv, string publicKey)
        {
            var encryptedData = _symmetricKeyEncryption.Encrypt(data, sessionKey, Iv);

            var encryptedSessionKey = _asymmetricKeyEncryption.EncryptData(sessionKey, publicKey);
            byte[] hmacHash;
            using (var hmac = new HMACSHA256(sessionKey))
            {
                hmacHash = hmac.ComputeHash(encryptedData);
            }

            return new EncryptedData(encryptedSessionKey, encryptedData, Iv, hmacHash);
        }

        public byte[] DecryptData(EncryptedData encryptedData)
        {
            var decryptedSessionKey = _asymmetricKeyEncryption.DecryptData(encryptedData.SessionKey);

            using (var hmac = new HMACSHA256(decryptedSessionKey))
            {
                var hmacToCheck = hmac.ComputeHash(encryptedData.Data);

                if (!Compare(encryptedData.HmacHash, hmacToCheck))
                {
                    throw new CryptographicException("HMAC signatures do not match");
                }
            }

            var decryptedData = _symmetricKeyEncryption.Decrypt(encryptedData.Data, decryptedSessionKey, encryptedData.IV);

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

    public class EncryptedData
    {
        public byte[] SessionKey { get; }
        public byte[] Data { get; }
        public byte[] IV { get; }
        public byte[] HmacHash { get; }

        public byte[] HMAC { get; }

        public EncryptedData(byte[] sessionKey, byte[] encryptedData, byte[] iv, byte[] hmacHash)
        {
            SessionKey = sessionKey;
            Data = encryptedData;
            IV = iv;
            HmacHash = hmacHash;
        }
    }
}