using Newtonsoft.Json;
using System;
using GeometrySharp.Core;

// ToDo this class need to be tested.
// ToDo this class need to be commented.
// Note this class can be developed bit more looking Hypar or RhinoCommon.
namespace GeometrySharp.Geometry
{
    /// <summary>
    /// A Plane is simply an origin point and normal.
    /// </summary>
    public class Plane : Serializable<Plane>
    {
        public Vector3 Normal { get; set; }
        public Vector3 Origin { get; set; }
        public Plane(Vector3 origin, Vector3 direction)
        {
            this.Normal = direction;
            this.Origin = origin;
        }

        public override Plane FromJson(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize the plane to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
