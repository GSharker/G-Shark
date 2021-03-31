using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometrySharp.Geometry;

namespace GeometrySharp.Optimization
{
    public interface IObjectiveFunction
    {
        double Value(Vector3 v);
        Vector3 Gradient(Vector3 v);
    }
}
