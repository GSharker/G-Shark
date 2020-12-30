using System;
using System.Collections.Generic;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;



namespace VerbNurbsSharpConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Matrix i = Mat.Identity(4);
            Matrix a = new Matrix() { new List<double>() {1, 2, 3 }, new List<double>() {4, 5, 6 } };
            Matrix t = Mat.Transpose(a);

            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }
    }
}
