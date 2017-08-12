using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

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

                if (!Compare(encryptedData.HMACHash, hmacToCheck))
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
        private const int SESSION_KEY_INDEX = 0;
        private const int DATA_INDEX = 4;
        private const int IV_INDEX = 8;
        private const int HMAC_INDEX = 16;

        public byte[] SessionKey { get; }
        public byte[] Data { get; }
        public byte[] IV { get; }
        public byte[] HMACHash { get; }

        public EncryptedData(byte[] sessionKey, byte[] encryptedData, byte[] iv, byte[] hmacHash)
        {
            SessionKey = sessionKey;
            Data = encryptedData;
            IV = iv;
            HMACHash = hmacHash;
        }

        public static EncryptedData FromBlob(byte[] blob)
        {
            var sessionKeyLength = ExtractHeader(SESSION_KEY_INDEX, blob);

            var dataLength = ExtractHeader(DATA_INDEX, blob);

            var iVLength = ExtractHeader(IV_INDEX, blob);

            var hmacLength = ExtractHeader(HMAC_INDEX, blob);

            var sessionKey = ExtractData(0, sessionKeyLength, blob);
            var data = ExtractData(sessionKeyLength, dataLength, blob);
            var iv = ExtractData(sessionKeyLength + dataLength, iVLength, blob);
            var hmac = ExtractData(sessionKeyLength + dataLength + iVLength, hmacLength, blob);

            return new EncryptedData(sessionKey, data, iv, hmac);
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

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public byte[] ExportToBlob()
        {
            var header = GetBytes(SessionKey.Length)
                            .Concat(GetBytes(Data.Length))
                            .Concat(GetBytes(IV.Length))
                            .Concat(GetBytes(HMACHash.Length));

            List<byte> blob = new List<byte>();

            blob.AddRange(header);
            blob.AddRange(SessionKey);
            blob.AddRange(Data);
            blob.AddRange(IV);
            blob.AddRange(HMACHash);

            return blob.ToArray();
        }
    }
}