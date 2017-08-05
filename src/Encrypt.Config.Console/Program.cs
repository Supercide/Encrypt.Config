using System.Collections.Generic;
using System.IO;
using Encrypt.Config.Json;
using Encrypt.Config.RSA;

namespace Encrypt.Config.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<string, string> commands = new Dictionary<string, string>();

            for (var index = 0; index < args.Length; index+=2)
            {
                commands.Add(args[index], args[index+1]);
            }

            using (var wrapper = RSAContainerFactory.Create(commands["-n"], commands["-u"]))
            using (var fileEncrypter = new JsonConfigurationFileEncrypter(wrapper))
            {
                var export = wrapper.Export(false);

                if(commands.ContainsKey("-jc"))
                {
                    var jsonEncrypted = fileEncrypter.Encrypt(commands["-jc"]);

                    File.WriteAllText("appsettings.encrypted.json", jsonEncrypted);
                }

                if (commands.ContainsKey("-pbo"))
                {
                    File.WriteAllText(commands["-pbo"], export.Key);
                }
            }
        }
    }
}