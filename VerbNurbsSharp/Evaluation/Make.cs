using VerbNurbsSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerbNurbsSharp.Evaluation
{
    //ToDO initialized the class Make
    public class Make
    {
        public Make()
        {

        }

        public static NurbsCurve Polyline (List<Point> points)
        {
            KnotArray knots = new KnotArray() { 0.0, 0.0 };
            double lsum = 0.0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                lsum += Constants.DistanceTo(points[i], points[i + 1]);
                knots.Add(lsum);
            }
            knots.Add(lsum);
            var weights = points.Select(x => 1.0).ToList();
            points.ForEach(x => weights.Add(1.0));
            return new NurbsCurve(1, knots, Eval.Homogenize1d(points, weights));
        }
    }
}
