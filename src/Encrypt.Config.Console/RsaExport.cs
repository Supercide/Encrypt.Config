using System.Security.Cryptography;

namespace Encrypt.Config.Console {
    public class RsaExport
    {
        public RSAParameters RsaParameters { get; set; }
        public string Key { get; set; }
    }
}