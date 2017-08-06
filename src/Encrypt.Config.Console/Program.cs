using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Encrypt.Config.Json;
using Encrypt.Config.RSA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.Console
{
    public class Program
    {
        public const string COMMAND_DELIMINATOR = "-";

        public static void Main(string[] args)
        {
            Dictionary<string, string> context = ToCommands(args);

            if (context.ContainsKey(WellKnownCommands.JSON_CONFIGURATION_PATH) && 
                context.ContainsKey(WellKnownCommands.EXPORT_PUBLIC_KEY))
            {
                throw new InvalidOperationException();
            }

            if(context.ContainsKey(WellKnownCommands.PUBLIC_KEY_PATH))
            {
                EncryptConfigurationFromPublicKey(context);
            } else
            {
                ExportPublicKey(context);
            }
        }

        private static void ExportPublicKey(Dictionary<string, string> context)
        {
            using (var wrapper = RSAContainerFactory.Create(context[WellKnownCommands.SET_CONTAINER_NAME], context[WellKnownCommands.SET_USERNAME]))
            {
                var export = wrapper.Export(false);

                if(context.ContainsKey(WellKnownCommands.EXPORT_PUBLIC_KEY))
                {
                    File.WriteAllText(context[WellKnownCommands.EXPORT_PUBLIC_KEY], export);
                }
            }
        }

        private static void EncryptConfigurationFromPublicKey(Dictionary<string, string> commands)
        {
            var publicKey = File.ReadAllText(commands[WellKnownCommands.PUBLIC_KEY_PATH]);

            using (var wrapper = RSAContainerFactory.CreateFromPublicKey(commands[WellKnownCommands.SET_CONTAINER_NAME], publicKey, commands[WellKnownCommands.SET_USERNAME]))
            using (var fileEncrypter = new JsonConfigurationFileEncrypter(wrapper))
            {
                if(commands.ContainsKey(WellKnownCommands.JSON_CONFIGURATION_PATH))
                {
                    using (var reader = new JsonTextReader(new StreamReader(commands[WellKnownCommands.JSON_CONFIGURATION_PATH])))
                    {
                        JObject jsonEncrypted = fileEncrypter.Encrypt(JObject.Load(reader));

                        File.WriteAllText("appsettings.encrypted.json", jsonEncrypted.ToString(Formatting.None));
                    }
                }
            }
        }

        private static Dictionary<string, string> ToCommands(string[] args)
        {
            string[] commandsWithArguments = Regex.Split(string.Join(" ", args), "(^-|\\s-)")
                                     .Where(x => x.Trim() != "-" && !string.IsNullOrWhiteSpace(x))
                                     .ToArray();

            Dictionary<string, string> commandDictionary = new Dictionary<string, string>();

            foreach (var commandLine in commandsWithArguments)
            {
                var sections = commandLine.Split(' ');

                commandDictionary.Add(sections[0], string.Join(" ", sections.Skip(1)));
            }

            return commandDictionary;
        }
    }
}