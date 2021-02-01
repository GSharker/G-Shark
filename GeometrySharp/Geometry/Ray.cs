using Newtonsoft.Json;
using System;
using GeometrySharp.Core;

namespace GeometrySharp.Geometry
{
    // ToDo this class need to be tested.
    // ToDo this class need to be commented.
    // Note this class can be developed bit more looking Hypar or RhinoCommon.
    // Note Ray and Plane are the same.

    /// <summary>
    /// A Ray is simply an origin point and normal.
    /// </summary>
    public class Ray : Serializable<Ray>
    {
        public Vector3 Direction { get; set; }
        public Vector3 Origin { get; set; }
        public Ray(Vector3 origin, Vector3 direction)
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
