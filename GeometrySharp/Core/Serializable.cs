using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ToDo this class has to be commented.
namespace GeometrySharp.Core
{

    public abstract class Serializable<T>
    {
        public abstract T FromJson(string s);
        public abstract string ToJson();
    }
}
