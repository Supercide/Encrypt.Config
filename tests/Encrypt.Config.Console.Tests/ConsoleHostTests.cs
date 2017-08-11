using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using Encrypt.Config.ConsoleHost;
using Encrypt.Config.Encryption.Asymmetric;
using Encrypt.Config.Encryption.Random;
using Encrypt.Config.Encryption.RSA;
using Encrypt.Config.Encryption.Symmetric;
using Encrypt.Config.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    //TODO:Help command

    [TestFixture]
    public class ConsoleHostTests
    {
        private readonly string[] _files;

        public ConsoleHostTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()))
                              .ToArray();
        }

        /*
         * sets ACL
         */

        [Test]
        public void GivenCommandToExportKey_WhenCallingCommand_ThenExportsPublicKey()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            FileAssert.Exists("key");
        }

        [Test]
        public void GivenCommandToExportKey_WhenCallingCommand_ThenExportsPublicKey_WithEncryptedSessionKey()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            Assert.Fail();
        }

        [Test]
        public void GivenCommandToExportKey_WhenCallingCommand_ThenExportsPublicKey_WithIV()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            Assert.Fail();
        }

        [Test]
        public void GivenCommandToExportKey_WhenCallingCommand_ThenExportsPublicKey_WithHMAC()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[]{"create", "keys",
                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                $"-{WellKnownCommandArguments.EXPORT_KEY}", "key",
                $"-{WellKnownCommandArguments.EXPORT_PUBLIC_KEY}",
                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            Assert.Fail();
        }

       [Test]
        public void GivenCreateKeysCommand_WhenCreatingKeys_ThenSetsACLOnKeyContainer()
        {
            var currentUser = WindowsIdentity.GetCurrent().Name;

            Program.Main(new[] { "create", "keys",
                                $"-{WellKnownCommandArguments.USERNAME}", $"{currentUser}",
                                $"-{WellKnownCommandArguments.CONTAINER_NAME}", "TestContainer"});

            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys");

            var containerFile = files.Except(_files).Single();

            var controlList = File.GetAccessControl(containerFile);

            var accessRule = controlList.GetAccessRules(true, true, typeof(NTAccount))
                                         .Cast<AuthorizationRule>()
                                         .Single();

            Assert.That(accessRule.IdentityReference.Value, Is.EqualTo(currentUser));
        }

        [TearDown]
        public void CleanUp()
        {
            if(File.Exists("publicKey.xml"))
            {
                File.Delete("publicKey.xml");
            }

            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                                 .Concat(Directory.EnumerateFiles(Directory.GetCurrentDirectory()));

            var newFiles = files.Except(_files);

            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
            }
        }
    }
}
