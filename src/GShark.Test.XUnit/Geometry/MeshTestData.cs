using System.Collections.Generic;
using GShark.Geometry;

namespace GShark.Test.XUnit.Geometry
{
  public class MeshTestData
  {
    public const double Tolerance = 0.001;

    public static IEnumerable<object[]> Meshes =>
      new List<object[]>
      {
        new object[] { FlatSquare },
        new object[] { FlatTriangle },
        new object[] { Pyramid },
        new object[] { Cube }
      };

    public static IEnumerable<object[]> Meshes_Type =>
      new List<object[]>
      {
        new object[] { FlatSquare, false, true, false },
        new object[] { FlatTriangle, true, false, false },
        new object[] { Cube, false, true, false }
      };

    public static IEnumerable<object[]> Meshes_Area =>
      new List<object[]>
      {
        new object[] { FlatSquare, 1.0 },
        new object[] { FlatTriangle, 0.5 },
        new object[] { Cube, 6 }
      };

    public static IEnumerable<object[]> Meshes_Boundary =>
      new List<object[]>
      {
        new object[] { FlatSquare, 1 },
        new object[] { FlatTriangle, 1 },
        new object[] { Cube, 0 },
        new object[] { Pyramid, 0 }
      };

    public static IEnumerable<object[]> Meshes_Euler =>
      new List<object[]>
      {
        new object[] { FlatSquare, 1 },
        new object[] { FlatTriangle, 1 },
        new object[] { Cube, 2 },
        new object[] { Pyramid, 2 }
      };

    public static Mesh FlatSquare
    {
      get
      {
        var ptA = new Point3(0, 0, 0);
        var ptB = new Point3(1, 0, 0);
        var ptC = new Point3(1, 1, 0);
        var ptD = new Point3(0, 1, 0);
        var vertices = new List<Point3> { ptA, ptB, ptC, ptD };
        var face = new List<int> { 0, 1, 2, 3 };
        var mesh = new Mesh(vertices, new List<List<int>> { face });
        return mesh;
      }
    }

    public static Mesh FlatTriangle
    {
      get
      {
        var ptA = new Point3(0, 0, 0);
        var ptB = new Point3(1, 0, 0);
        var ptC = new Point3(1, 1, 0);
        var vertices = new List<Point3> { ptA, ptB, ptC };
        var face = new List<int> { 0, 1, 2 };
        var mesh = new Mesh(vertices, new List<List<int>> { face });
        return mesh;
      }
    }

    public static Mesh Cube
    {
      get
      {
        var pt0 = new Point3(0, 0, 0);
        var pt1 = new Point3(1, 0, 0);
        var pt2 = new Point3(1, 1, 0);
        var pt3 = new Point3(0, 1, 0);
        var pt4 = new Point3(0, 0, 1);
        var pt5 = new Point3(1, 0, 1);
        var pt6 = new Point3(1, 1, 1);
        var pt7 = new Point3(0, 1, 1);
        var vertices = new List<Point3> { pt0, pt1, pt2, pt3, pt4, pt5, pt6, pt7 };
        var side1 = new List<int> { 0, 1, 5, 4 };
        var side2 = new List<int> { 1, 2, 6, 5 };
        var side3 = new List<int> { 2, 3, 7, 6 };
        var side4 = new List<int> { 3, 0, 4, 7 };
        var bottom = new List<int> { 0, 1, 2, 3 };
        var top = new List<int> { 4, 5, 6, 7 };
        var mesh = new Mesh(vertices, new List<List<int>> { side1, side2, side3, side4, bottom, top });
        return mesh;
      }
    }

    public static Mesh Pyramid
    {
      get
      {
        var pt0 = new Point3(0, 0, 0);
        var pt1 = new Point3(1, 0, 0);
        var pt2 = new Point3(1, 1, 0);
        var pt3 = new Point3(0, 1, 0);
        var pt4 = new Point3(0.5, 0.5, 1);
        var vertices = new List<Point3> { pt0, pt1, pt2, pt3, pt4 };
        var bottom = new List<int> { 0, 1, 2, 3 };
        var left = new List<int> { 0, 1, 4 };
        var front = new List<int> { 1, 2, 4 };
        var right = new List<int> { 2, 3, 4 };
        var back = new List<int> { 3, 0, 4 };
        var mesh = new Mesh(vertices, new List<List<int>> { bottom, left, front, right, back });
        return mesh;
      }
    }
  }
}