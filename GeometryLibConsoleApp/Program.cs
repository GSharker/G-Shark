using GeometryLib.Core;
using Newtonsoft.Json;
using System;

namespace GeometryLibConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Point p = new Point(0, 1, 0);
            var j = Point.ToJson(p);
            var p1 = Point.FromJson(j);


            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }
    }
}
