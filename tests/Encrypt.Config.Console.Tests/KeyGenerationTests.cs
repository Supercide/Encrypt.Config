using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Encrypt.Config.Console.Tests
{
    class KeyGenerationTests
    {
        private IEnumerable<string> _files;

        [OneTimeSetUp]
        public void Setup()
        {
            _files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys")
                              .ToArray();
        }

        [Test]
        public void GivenContainerName_WhenCreeatingContainer_CreatesContainer_WithName()
        {
            RSAContainerFactory factory = new RSAContainerFactory();

            var containerName = $"{Guid.NewGuid()}";

            var containerInfo = factory.Create(containerName);

            Assert.That(containerInfo.KeyContainerName, Is.EqualTo(containerName));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            var files = Directory.EnumerateFiles(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys");

            var newFiles = files.Except(_files);

            foreach (var newFile in newFiles)
            {
                File.Delete(newFile);
            }
        }
    }
}
