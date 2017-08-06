using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Encrypt.Config.Console.ConsoleStateMachine.Factories;
using Encrypt.Config.Console.ConsoleStateMachine.States;
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

            if (context.ContainsKey(WellKnownCommandArguments.JSON_CONFIGURATION_PATH) && 
                context.ContainsKey(WellKnownCommandArguments.EXPORT_PUBLIC_KEY))
            {
                throw new InvalidOperationException();
            }

            if(context.ContainsKey(WellKnownCommandArguments.PUBLIC_KEY_PATH))
            {
                EncryptConfigurationFromPublicKey(context);
            } else
            {
                ExportPublicKey(context);
            }
        }

        private static void ExportPublicKey(Dictionary<string, string> context)
        {
            using (var wrapper = RSAContainerFactory.Create(context[WellKnownCommandArguments.SET_CONTAINER_NAME], context[WellKnownCommandArguments.SET_USERNAME]))
            {
                var export = wrapper.Export(false);

                if(context.ContainsKey(WellKnownCommandArguments.EXPORT_PUBLIC_KEY))
                {
                    File.WriteAllText(context[WellKnownCommandArguments.EXPORT_PUBLIC_KEY], export);
                }
            }
        }

        private static void EncryptConfigurationFromPublicKey(Dictionary<string, string> commands)
        {
            var publicKey = File.ReadAllText(commands[WellKnownCommandArguments.PUBLIC_KEY_PATH]);

            using (var wrapper = RSAContainerFactory.CreateFromPublicKey(commands[WellKnownCommandArguments.SET_CONTAINER_NAME], publicKey, commands[WellKnownCommandArguments.SET_USERNAME]))
            using (var fileEncrypter = new JsonConfigurationFileEncrypter(wrapper))
            {
                if(commands.ContainsKey(WellKnownCommandArguments.JSON_CONFIGURATION_PATH))
                {
                    using (var reader = new JsonTextReader(new StreamReader(commands[WellKnownCommandArguments.JSON_CONFIGURATION_PATH])))
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

        public ConsoleState CreateState(string command)
        {
            IConsoleStateFactory[] factories = new[]
            {
                new CreateStateFactory(),
            };

            return factories.Single(factory => factory.CanParse(command))
                            .Create();
        }
    }
}