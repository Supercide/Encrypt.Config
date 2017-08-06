using System;
using System.IO;
using Encrypt.Config.Json;
using Encrypt.Config.RSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.ConsoleHost.ConsoleStateMachine.States {
    public class JsonConfigEncryptionState : ConsoleState
    {
        public override void Handle(Context context)
        {
            try
            {
                using (RSAWrapper wrapper = CreateContainer(context))
                {
                    string path;
                    if(!context.Arguments.TryGetValue(WellKnownCommandArguments.JSON_CONFIGURATION_PATH, out path))
                    {
                        throw new InvalidOperationException();
                    }

                    using (var fileEncrypter = new JsonConfigurationFileEncrypter(wrapper))
                    {
                        using (var reader = new JsonTextReader(new StreamReader(path)))
                        {
                            JObject jsonEncrypted = fileEncrypter.Encrypt(JObject.Load(reader));

                            File.WriteAllText("appsettings.encrypted.json", jsonEncrypted.ToString(Formatting.None));
                        }
                    }
                    SetEndState(context);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private static RSAWrapper CreateContainer(Context context)
        {
            string keyPath;
            if(!context.Arguments.TryGetValue(WellKnownCommandArguments.IMPORT_KEY, out keyPath))
            {
                throw new InvalidOperationException();
            }

            var key = File.ReadAllText(keyPath);

            return new RSAContainerFactory.CreateFromPublicKey(key);
        }
    }
}