using System.Security.Cryptography;

namespace Encrypt.Config.RSA {
    public class RSAExport
    {
        public RSAParameters RsaParameters { get; set; }
        public string Key { get; set; }
    }
}