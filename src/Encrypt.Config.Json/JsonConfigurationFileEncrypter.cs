using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Random;
using Newtonsoft.Json.Linq;
using Encrypt.Config.Encryption.Symmetric;

namespace Encrypt.Config.Json
{
    public class JsonConfigurationFileEncrypter
    {
        private readonly IHybridEncryption _hybridEncryption;
        private IGenerateRandomBytes _randomBytesGenerator;

        public JsonConfigurationFileEncrypter(IHybridEncryption hybridEncryption, IGenerateRandomBytes randomBytesGenerator)
        {
            _hybridEncryption = hybridEncryption;

            _randomBytesGenerator = randomBytesGenerator;
        }

        public JObject Encrypt(JObject jsonObject, string publicKey)
        {
            var configurationProperties = GetConfigurationProperties(jsonObject);

            foreach (var kvp in configurationProperties)
            {
                var data = Encoding.Unicode.GetBytes(kvp.Value);

                var sessionKey = _randomBytesGenerator.Generate(32);
                var iv = _randomBytesGenerator.Generate(16);

                var encryptedValue = _hybridEncryption.EncryptData(sessionKey, data, iv, publicKey);

                string base64Data = Convert.ToBase64String(encryptedValue.Data);
                string base64Hash = Convert.ToBase64String(encryptedValue.HMACHash);
                string base64Key = Convert.ToBase64String(encryptedValue.SessionKey);
                string base64IV = Convert.ToBase64String(encryptedValue.IV);

                UpdateValue(kvp.Key, jsonObject, $"{base64Data}.{base64Hash}.{base64Key}.{base64IV}");
            }

            return jsonObject;
        }

        public JObject Decrypt(JObject jsonObject)
        {
            var configurationProperties = GetConfigurationProperties(jsonObject);

            foreach (var kvp in configurationProperties)
            {
                var encryptedData = kvp.Value.Split('.');

                var base64Data = Convert.FromBase64String(encryptedData[0]);
                var base64Hash = Convert.FromBase64String(encryptedData[1]);
                var base64Key = Convert.FromBase64String(encryptedData[2]);
                var base64IV = Convert.FromBase64String(encryptedData[3]);

                var decryptedValue = _hybridEncryption.DecryptData(new EncryptedData(base64Key, base64Data, base64IV, base64Hash));

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
    }
}
