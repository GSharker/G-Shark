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
        public List<Tri> Faces { get; set; }
        public List<Point> Points { get; set; }
        public List<Vector> Normals { get; set; }
        public List<UV> UVs { get; set; }

        public MeshData(List<Tri> faces, List<Point> points, List<Vector> normals, List<UV> uVs)
        {
            Faces = faces;
            Points = points;
            Normals = normals;
            UVs = uVs;
        }

        internal static MeshData Empty() => new MeshData(
            new List<Tri>(),
            new List<Point>(),
            new List<Vector>(),
            new List<UV>()
            );
    }
}
