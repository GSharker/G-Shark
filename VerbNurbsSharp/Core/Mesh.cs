using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Core
{
    /// <summary>
    /// A simple data structure representing a mesh. MeshData does not check for legality.
    /// </summary>
    public class Mesh : Serializable<Mesh>
    {
        public List<Triangle> Faces { get; set; }
        public List<Point> Points { get; set; }
        public List<Vector> Normals { get; set; }
        public List<UV> UVs { get; set; }

        public Mesh(List<Triangle> faces, List<Point> points, List<Vector> normals, List<UV> uVs)
        {
            Faces = faces;
            Points = points;
            Normals = normals;
            UVs = uVs;
        }

        internal static Mesh Empty() => new Mesh(
            new List<Triangle>(),
            new List<Point>(),
            new List<Vector>(),
            new List<UV>()
            );

        public override Mesh FromJson(string s)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize a mesh to JSON
        /// </summary>
        /// <returns></returns>
        public override string ToJson() => JsonConvert.SerializeObject(this);
    }
}
