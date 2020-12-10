using GeometryLib.Core;
using Newtonsoft.Json;
using System;

namespace GeometryLibConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Point pt = new Point(0, 0, 0);
            var ptj = Point.ToJson(pt);

            Plane p = new Plane(new Point(0, 0, 0), new Vector(0, 0, 1));
            var s = Plane.ToJson(p);


            Console.WriteLine("Press any key to close...");
            Console.ReadLine();
        }
    }
}
