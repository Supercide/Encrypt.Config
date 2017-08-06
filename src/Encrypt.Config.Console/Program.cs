using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Encrypt.Config.Json;
using Encrypt.Config.RSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Contains("-jc") && args.Contains("-pbo"))
            {
                throw new InvalidOperationException();
            }

            Dictionary<string, string> commands = new Dictionary<string, string>();

            for (var index = 0; index < args.Length; index+=2)
            {
                commands.Add(args[index], args[index+1]);
            }

            if(commands.ContainsKey("-pbi"))
            {
                var publicKey = File.ReadAllText(commands["-pbi"]);

                using (var wrapper = RSAContainerFactory.CreateFromPublicKey(commands["-n"], publicKey, commands["-u"]))
                using (var fileEncrypter = new JsonConfigurationFileEncrypter(wrapper))
                {
                    if(commands.ContainsKey("-jc"))
                    {
                        using (var reader = new JsonTextReader(new StreamReader(commands["-jc"])))
                        {
                            JObject jsonEncrypted = fileEncrypter.Encrypt(JObject.Load(reader));

                            File.WriteAllText("appsettings.encrypted.json", jsonEncrypted.ToString(Formatting.None));
                        }
                    }
                }
            } else
            {
                using (var wrapper = RSAContainerFactory.Create(commands["-n"], commands["-u"]))
                    using (var fileEncrypter = new JsonConfigurationFileEncrypter(wrapper))
                    {
                        var export = wrapper.Export(false);

                        if (commands.ContainsKey("-jc"))
                        {
                            using (var reader = new JsonTextReader(new StreamReader(commands["-jc"])))
                            {
                                JObject jsonEncrypted = fileEncrypter.Encrypt(JObject.Load(reader));

                                File.WriteAllText("appsettings.encrypted.json", jsonEncrypted.ToString(Formatting.None));
                            }
                        }

                        if (commands.ContainsKey("-pbo"))
                        {
                            File.WriteAllText(commands["-pbo"], export);
                        }
                    }
            }
        }
    }
}