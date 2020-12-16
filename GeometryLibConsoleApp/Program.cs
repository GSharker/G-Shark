using GeometryLib.Core;
using GeometryLib.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeometryLibConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = new Line(new Point() { 0, 0, 0 }, new Point() { 1, 1, 1 });
            var json = Line.ToJson(line);

            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }
    }
}
