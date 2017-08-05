using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Encrypt.Config.Json {
    public class JsonReader
    {
        public JObject Read(Stream input)
        {
            JsonTextReader reader = new JsonTextReader(new StreamReader(input))
            {
                DateParseHandling = DateParseHandling.None
            };

            JObject jsonConfig = JObject.Load(reader);

            return jsonConfig;
        }
    }
}