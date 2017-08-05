using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Encrypt.Config.RSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.Json
{
    public class JsonConfigurationFileEncrypter : IDisposable
    {
        private readonly RSAWrapper _rsaWrapper;
        private readonly RandomNumberGenerator _rng;

        public JsonConfigurationFileEncrypter(RSAWrapper RSAWrapper)
        {
            _rsaWrapper = RSAWrapper;
            _rng = new RNGCryptoServiceProvider();
        }

        public string Encrypt(string jsonConfigPath)
        {
            JsonConfigurationFileParser parser = new JsonConfigurationFileParser();
            JsonReader reader = new JsonReader();

            var obj = reader.Read(File.Open(jsonConfigPath, FileMode.Open));

            var parsedJson = parser.Parse(obj);

            foreach (var kvp in parsedJson)
            {
                var bytes = Encoding.UTF8.GetBytes(kvp.Value);
                byte[] salt = new byte[24];
                _rng.GetBytes(salt);
                var encryptedBytes = _rsaWrapper.Encrypt(bytes, salt);

                string base64 = Convert.ToBase64String(encryptedBytes);
                string saltBase64 = Convert.ToBase64String(salt);

                string[] properties = kvp.Key.Split('.');

                JToken current = obj;//[properties[0]];

                for (var i = 0; i < properties.Length; i++)
                {
                    current = current[properties[i]];
                    if (i == properties.Length - 2)
                    {
                        current[properties[i + 1]] = $"{base64}.{saltBase64}";
                    }
                }
            }

           return JsonConvert.SerializeObject(obj);
        }

        public void Dispose()
        {
            _rsaWrapper?.Dispose();
            _rng?.Dispose();
        }
    }
}
