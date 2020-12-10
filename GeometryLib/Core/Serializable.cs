using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryLib.Core
{

    public class Serializable<T>: Coordinates
    {
        public static T FromJson(string s) => JsonConvert.DeserializeObject<T>(s);
        public static string ToJson(T t) => JsonConvert.SerializeObject(t,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
            );
    }
}
