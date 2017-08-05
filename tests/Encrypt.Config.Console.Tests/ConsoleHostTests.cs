using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    // TODO: Don't encrypt and port key in the same opersation, only encrypted with imported public key

    [TestFixture]
    public class ConsoleHostTests
    {
        private readonly string[] _files;

        public ConsoleHostTests()
        {
            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .ToArray();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void GivenArguments_WhenGeneratingKey_GeneratesKey()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new []{"-u", $"{currentUser}","-pbo","publicKey.xml", "-n", "TestContainer"});

            FileAssert.Exists("publicKey.xml");
        }

        [Test]
        public void GivenOptionToEncryptConfig_WhenCallingCommand_ThenEncryptsConfig()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "-u", $"{currentUser}", "-pbo", "publicKey.xml", "-n", "TestContainer" , "-jc", "appsettings.json"});

            FileAssert.Exists("appsettings.encrypted.json");
        }

        [Test]
        public void GivenEncryptedConfig_AndPrivateKey_WhenDecryptingConfigOptionToEncryptConfig_WhenCallingCommand_ThenEncryptsConfig()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "-u", $"{currentUser}", "-pbo", "publicKey.xml", "-n", "TestContainer", "-jc", "appsettings.json" });

            FileAssert.Exists("appsettings.encrypted.json");
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            if(File.Exists("publicKey.xml"))
            {
                File.Delete("publicKey.xml");
            }

            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys");

            var newFiles = files.Except(_files);

            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
            }
        }
    }
}
