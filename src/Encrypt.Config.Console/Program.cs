using System.Collections.Generic;
using System.IO;

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

            var rsaContainerFactory = new RSAContainerFactory();

            using (var wrapper = rsaContainerFactory.Create(commands["-n"], commands["-u"]))
            {
                var export = wrapper.Export(false);



                if(commands.ContainsKey("-jc"))
                {
                    var json = File.ReadAllText(commands["-jc"]);
                }

                if (commands.ContainsKey("-pbo"))
                {
                    File.WriteAllText(commands["-pbo"], export.Key);
                }
            }
        }
    }
}