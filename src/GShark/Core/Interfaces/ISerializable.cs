using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GShark.Core.Interfaces
{
    public interface ISerializable<T>
    {
        public T FromJson(string s);
        public string ToJson();
    }
}
