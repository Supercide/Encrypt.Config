using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Encrypt.Config.Console.ConsoleStateMachine;
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

        private static readonly IConsoleStateFactory[] _factories = {
            new CreateStateFactory(),
            new JsonConfigEncryptionStateFactory(),
            new ImportStateFactory(),
            new ExportStateFactory()
        };

        public static void Main(string[] args)
        {
            var context = ToCommands(args);

            context.Request();

            /*
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
            */
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

        private static Context ToCommands(string[] args)
        {
            string[] commandsWithArguments = Regex.Split(string.Join(" ", args), "(^-|\\s-)")
                                                  .Where(x => x.Trim() != "-" && !string.IsNullOrWhiteSpace(x))
                                                  .ToArray();

            var consoleState = CreateState(commandsWithArguments.First());

            Dictionary<string, string> commandDictionary = new Dictionary<string, string>();

            foreach (var commandLine in commandsWithArguments.Skip(1))
            {
                var sections = commandLine.Split(' ');

                commandDictionary.Add(sections[0], string.Join(" ", sections.Skip(1)));
            }

            return new Context(commandDictionary, consoleState);
        }

        public static ConsoleState CreateState(string command)
        {
            return _factories.Single(factory => factory.CanParse(command))
                             .Create();
        }
    }
}