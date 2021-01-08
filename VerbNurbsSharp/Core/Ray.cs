using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    //ToDo why Ray an Plan are the same?
    /// <summary>
    /// A Ray is simply an origin point and normal.
    /// </summary>
    public class Ray : Serializable<Ray>
    {
        public Vector Direction { get; set; }
        public Point Origin { get; set; }
        public Ray(Point origin, Vector direction)
        {
            this.Direction = direction;
            this.Origin = origin;
        }

        public override Ray FromJson(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize a ray to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
