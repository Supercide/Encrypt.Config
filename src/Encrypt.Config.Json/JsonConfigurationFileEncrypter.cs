using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Encrypt.Config.RSA;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.Json
{
    public class JsonConfigurationFileEncrypter : IDisposable
    {
        private readonly RSACryptoServiceProvider _rsaCryptoServiceProvider;
        private readonly RandomNumberGenerator _rng;

        public JsonConfigurationFileEncrypter(RSACryptoServiceProvider rsaCryptoServiceProvider)
        {
            _rsaCryptoServiceProvider = rsaCryptoServiceProvider;
            _rng = new RNGCryptoServiceProvider();
        }

        public JObject Encrypt(JObject jsonObject)
        {
            var configurationProperties = GetConfigurationProperties(jsonObject);

            foreach (var kvp in configurationProperties)
            {
                byte[] salt = GenerateSalt();

                var encodedPropertyValue = Encoding.Unicode.GetBytes(kvp.Value);

                var encryptedValue = _rsaCryptoServiceProvider.Encrypt(salt.Concat(encodedPropertyValue).ToArray(), RSAEncryptionPadding.Pkcs1);

                string base64 = Convert.ToBase64String(encryptedValue);

                string saltBase64 = Convert.ToBase64String(salt);

                UpdateValue(kvp.Key, jsonObject, $"{base64}.{saltBase64}");
            }

            return jsonObject;
        }

        public JObject Decrypt(JObject jsonObject)
        {
            var configurationProperties = GetConfigurationProperties(jsonObject);

            foreach (var kvp in configurationProperties)
            {
                var encryptedValueAndSalt = kvp.Value.Split('.');

                byte[] salt = Convert.FromBase64String(encryptedValueAndSalt[1]);
                byte[] encryptedValue = Convert.FromBase64String(encryptedValueAndSalt[0]);

                var decryptedValue = _rsaCryptoServiceProvider.Decrypt(encryptedValue, RSAEncryptionPadding.Pkcs1).Skip(salt.Length).ToArray();

                var propertyValue = Encoding.Unicode.GetString(decryptedValue);

                UpdateValue(kvp.Key, jsonObject, propertyValue);
            }

            return jsonObject;
        }


        private static IDictionary<string, string> GetConfigurationProperties(JObject obj)
        {
            JsonConfigurationFileParser jsonConfigurationFileParser = new JsonConfigurationFileParser();
            var configurationProperties = jsonConfigurationFileParser.Parse(obj);
            return configurationProperties;
        }

        private static void UpdateValue(string path, JObject obj, string value)
        {
            string[] propertyPath = path.Split('.');

            JToken current = obj;

            for (var i = 0; i < propertyPath.Length; i++)
            {
                current = current[propertyPath[i]];
                if(i == propertyPath.Length - 2)
                {
                    current[propertyPath[i + 1]] = value;
                }
            }
        }

        private byte[] GenerateSalt()
        {
            var salt = new byte[24];
            _rng.GetBytes(salt);
            return salt;
        }

        public void Dispose()
        {
            _rsaCryptoServiceProvider?.Dispose();
            _rng?.Dispose();
        }
    }
}
