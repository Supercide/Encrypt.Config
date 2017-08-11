using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Encrypt.Config.ConsoleHost.ConsoleStateMachine;
using Encrypt.Config.ConsoleHost.ConsoleStateMachine.Factories;
using Encrypt.Config.ConsoleHost.ConsoleStateMachine.States;

namespace Encrypt.Config.ConsoleHost
{
    public class Program
    {
        public const string COMMAND_DELIMINATOR = "-";

        private static readonly IConsoleStateFactory[] _factories = {
            new CreateStateFactory(),
            new JsonConfigEncryptionStateFactory(),
            new ExportStateFactory()
        };

        public static void Main(string[] args)
        {
            var context = ToCommands(args);

            do
            {
                context.Request();

            } while (context.State.GetType() != typeof(EndState));


            
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