using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Encrypt.Config.Encryption.Asymmetric {
    public interface IAsymmetricKeyEncryption
    {
        string ExportKey(bool includePrivate);

        byte[] EncryptData(byte[] data);

        byte[] DecryptData(byte[] data);
    }

    public interface IHybridEncryption
    {
        (EncryptionKey key, byte[] encryptedData) EncryptData(byte[] sessionKey, byte[] data, byte[] Iv);

        byte[] DecryptData(EncryptionKey encryptionKey, byte[] data);
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

    public class EncryptionKey
    {
        private const int SESSION_KEY_INDEX = 12;
        private const int IV_INDEX = 16;
        private const int HMAC_INDEX = 20;

        public byte[] SessionKey { get; }
        public byte[] IV { get; }
        public byte[] HMACHash { get; }

        public EncryptionKey(byte[] sessionKey, byte[] iv, byte[] hmacHash)
        {
            SessionKey = sessionKey;
            IV = iv;
            HMACHash = hmacHash;
        }

        public static EncryptionKey FromBlob(byte[] blob)
        {
            var sessionKeyLength = ExtractHeader(SESSION_KEY_INDEX, blob);

            var iVLength = ExtractHeader(IV_INDEX, blob);

            var hmacLength = ExtractHeader(HMAC_INDEX, blob);

            var sessionKey = ExtractData(0, sessionKeyLength, blob);
            var iv = ExtractData(sessionKeyLength, iVLength, blob);
            var hmac = ExtractData(sessionKeyLength + iVLength, hmacLength, blob);

            return new EncryptionKey(sessionKey, iv, hmac);
        }

        private static byte[] ExtractData(int index, int length, byte[] blob)
        {
            return blob.Skip(index)
                       .Take(length)
                       .ToArray();
        }
        private static int ExtractHeader(int index, byte[] blob)
        {
            var headerBlob = blob.Skip(index)
                             .Take(4)
                             .ToArray();

            return BitConverter.ToInt32(headerBlob, 0);
        }

        private byte[] GetBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            return bytes;
        }

        public byte[] ExportToBlob()
        {
            var header = GetBytes(SessionKey.Length)
                            .Concat(GetBytes(IV.Length))
                            .Concat(GetBytes(HMACHash.Length));

            List<byte> blob = new List<byte>();

            blob.AddRange(header);
            blob.AddRange(SessionKey);
            blob.AddRange(IV);
            blob.AddRange(HMACHash);

            return blob.ToArray();
        }
    }
}