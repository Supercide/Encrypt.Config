using System;
using System.IO;
using System.Security.Cryptography;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Random;
using Encrypt.Config.Encryption.RSA;
using Encrypt.Config.Encryption.Symmetric;
using Encrypt.Config.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class JsonConfigEncryptionState : ConsoleState
    {
        public override void Handle(Context context)
        {
            try
            {
                string path;
                if(!context.Arguments.TryGetValue(WellKnownCommandArguments.JSON_CONFIGURATION_PATH, out path))
                {
                    throw new InvalidOperationException();
                }

                var fileEncrypter = new JsonConfigurationFileEncrypter(new HybridEncryption(new RSAEncryption(), new AESEncryption()), new RNGCryptoRandomBytesGenerator());

                using (var reader = new JsonTextReader(new StreamReader(path)))
                {
                    //TODO grab public key
                    JObject jsonEncrypted = fileEncrypter.Encrypt(JObject.Load(reader), "");

                    File.WriteAllText("appsettings.encrypted.json", jsonEncrypted.ToString(Formatting.None));
                }

                SetEndState(context);

            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}