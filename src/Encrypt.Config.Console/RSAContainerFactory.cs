using System.Security.Cryptography;

namespace Encrypt.Config.Console {
    public class RSAContainerFactory
    {
        public CspKeyContainerInfo Create(string containerName)
        {
            CspParameters cspParams = new CspParameters();

            cspParams.KeyContainerName = containerName;

            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams);

            return new CspKeyContainerInfo(cspParams);
        }
    }
}