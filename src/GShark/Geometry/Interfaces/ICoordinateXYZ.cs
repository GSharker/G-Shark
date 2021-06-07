using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GShark.Geometry.Interfaces
{
    /// <summary>
    /// This interface defines the basic coordinates used to describes a geometry in a 3-dimensional space.
    /// </summary>
    public interface ICoordinateXYZ
    {
        /// <summary>
        /// Gets the X coordinate of the Euclidean 3d geometry.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate of the Euclidean 3d geometry.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the Z coordinate of the Euclidean 3d geometry.
        /// </summary>
        public double Z { get; }
    }
}
