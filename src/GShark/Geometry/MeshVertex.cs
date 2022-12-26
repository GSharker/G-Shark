using System.Collections.Generic;
using System.Linq;

namespace GShark.Geometry
{
  /// <summary>
  ///     Represents a vertex of a mesh.
  /// </summary>
  public class MeshVertex : Point3
  {
    /// <summary>
    ///     Initializes a new instance of the <see cref="MeshVertex" /> class.
    /// </summary>
    public MeshVertex()
    {
    }


    /// <summary>
    ///     Initializes a new instance of the <see cref="MeshVertex" /> class from a 3D point.
    /// </summary>
    /// <param name="pt">Point to copy coordinates from.</param>
    public MeshVertex(Point3 pt)
      : base(pt)
    {
    }


    /// <summary>
    ///     Initializes a new instance of the <see cref="MeshVertex" /> class from it's cartesian
    ///     coordinates.
    /// </summary>
    /// <param name="x">X Coordinate.</param>
    /// <param name="y">Y Coordinate.</param>
    /// <param name="z">Z Coordinate.</param>
    public MeshVertex(double x, double y, double z)
      : base(x, y, z)
    {
    }

    /// <summary>
    ///     Gets or sets the half-edge this vertex is attached to.
    /// </summary>
    public MeshHalfEdge HalfEdge { get; set; }

    /// <summary>
    ///     Gets or sets the index of the vertex.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    ///     Computes the valence of the vertex.
    /// </summary>
    /// <returns></returns>
    public int Valence() => AdjacentHalfEdges().Count();
    
    /// <summary>
    ///     Check if vertex is isolated, meaning corresponding half-edge is null.
    /// </summary>
    /// <returns></returns>
    public bool IsIsolated() => HalfEdge == null;
    
    /// <summary>
    ///     Check if vertex is on mesh boundary.
    /// </summary>
    /// <returns></returns>
    public bool OnBoundary() => AdjacentHalfEdges().Any(halfEdge => halfEdge.OnBoundary);
    
    /// <summary>
    ///     Returns a list with all adjacent HE_HalfEdge of this vertex.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MeshHalfEdge> AdjacentHalfEdges()
    {
      var current = HalfEdge;
      do
      {
        yield return current;
        current = current.Twin.Next;
      } while (current != HalfEdge);
    }

    /// <summary>
    ///     Returns a list with all adjacent HE_Face of a vertex.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MeshFace> AdjacentFaces()
    {
      var current = HalfEdge;
      do
      {
        if (!current.OnBoundary)
          yield return current.Face;
        current = current.Twin.Next;
      } while (current != HalfEdge);
    }

    /// <summary>
    ///     Returns a list with all the adjacent HE_Vertex of this vertex.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MeshVertex> AdjacentVertices()
    {
      var current = HalfEdge;
      do
      {
        yield return current.Twin.Vertex;
        current = current.Twin.Next;
      } while (current != HalfEdge);
    }

    /// <summary>
    ///     Returns a list with all the adjacent HE_Edge of this vertex.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MeshEdge> AdjacentEdges()
    {
      var current = HalfEdge;
      do
      {
        yield return current.Edge;
        current = current.Twin.Next;
      } while (current != HalfEdge);
    }

    /// <summary>
    ///     Returns a list with all the adjacent <see cref="MeshCorner"/> of this vertex.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MeshCorner> AdjacentCorners()
    {
      var current = HalfEdge;
      do
      {
        if (!current.OnBoundary)
          yield return current.Next.Corner;
        current = current.Twin.Next;
      } while (current != HalfEdge);
    }

    /// <summary>
    ///     Returns the string representation of this vertex.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => "V " + Index;
  }
}