using System;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;

namespace VerbNurbsSharpConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = new Line(new Point() { 0, 0, 0 }, new Point() { 1, 1, 1 });
            var json = Line.ToJson(line);

            var angle = Vec.AngleBetween(new Vector() { 1, 0, 0 }, new Vector() { 0, 1, 0 });

            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }
    }
}
