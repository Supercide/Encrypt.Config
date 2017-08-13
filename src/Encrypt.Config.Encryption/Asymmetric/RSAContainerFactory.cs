using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using Encrypt.Config.Encryption.Constants;

namespace Encrypt.Config.Encryption.Asymmetric {
    internal class RSAContainerFactory
    {
        public static RSACryptoServiceProvider Create(string containerName, string username)
        {
            var cspParams = CreateCspParameters(containerName);

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams)
            {
                PersistKeyInCsp = true
            };

            SetFileAccessRule(username, rsaProvider.CspKeyContainerInfo.UniqueKeyContainerName);

            return rsaProvider;
        }

        private static void SetFileAccessRule(string username, string uniqueKeyContainerName)
        {
            var filePath = Path.Combine(WellKnownPaths.RSA_MACHINEKEYS, uniqueKeyContainerName);

            var fs = new FileSecurity(filePath, AccessControlSections.All);

            AuthorizationRuleCollection accessRules = fs.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

            fs.SetAccessRuleProtection(true, false); 

            foreach (FileSystemAccessRule accessRule in accessRules) 
            {
                fs.PurgeAccessRules(accessRule.IdentityReference);  
            }

            fs.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.FullControl, AccessControlType.Allow));

            File.SetAccessControl(filePath, fs);
        }

        private static CspParameters CreateCspParameters(string containerName)
        {
            CspParameters cspParams = new CspParameters
            {
                KeyContainerName = containerName,
                KeyNumber = (int) KeyNumber.Exchange,
                Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.NoPrompt,
            };

            return cspParams;
        }

        public static RSACryptoServiceProvider CreateFromPublicKey(string key)
        {
            CspParameters cspParams = CreateCspParameters($"{Guid.NewGuid()}");

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams)
            {
                PersistKeyInCsp = false
            };

            rsaProvider.FromXmlString(key);

            return rsaProvider;
        }

        public static RSACryptoServiceProvider CreateFromContainer(string containerName)
        {
            CspParameters cspParams = CreateCspParameters(containerName);

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams)
            {
                PersistKeyInCsp = true
            };

            return rsaProvider;
        }
    }
}