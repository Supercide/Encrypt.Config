using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace Encrypt.Config.RSA {
    public class RSAContainerFactory
    {
        public static RSAWrapper Create(string containerName, string username)
        {
            var cspParams = CreateCspParamerters(containerName);

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams)
            {
                PersistKeyInCsp = true
            };

            RSAWrapper wrapper = new RSAWrapper(rsaProvider);

            SetFileAccessRule(username, rsaProvider.CspKeyContainerInfo.UniqueKeyContainerName);

            return wrapper;
        }

        private static void SetFileAccessRule(string username, string uniqueKeyContainerName)
        {
            var fs = new FileSecurity();
            fs.SetAccessRuleProtection(false, false);
            File.SetAccessControl(Path.Combine(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys", uniqueKeyContainerName), fs);

            fs.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.Read, AccessControlType.Allow));
            File.SetAccessControl(Path.Combine(@"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys", uniqueKeyContainerName), fs);
        }

        private static CspParameters CreateCspParamerters(string containerName)
        {
            CspParameters cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                KeyNumber = (int) KeyNumber.Exchange,
                Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.NoPrompt,
            };

            return cspParams;
        }

        public static RSAWrapper CreateFromPublicKey(string containerName, string publicKey, string username)
        {
            CspParameters cspParams = CreateCspParamerters(containerName);

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams)
            {
                PersistKeyInCsp = true
            };
            
            rsaProvider.FromXmlString(publicKey);

            return new RSAWrapper(rsaProvider);
        }
    }
}