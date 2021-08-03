using System;
using GShark.Geometry;
using GShark.Geometry.Enum;
using GShark.Operation;

namespace GShark.ExtendedMethods
{
    public static class Surface
    {
        /// <summary>
        /// Splits (divides) the surface into two parts at the specified parameter
        /// </summary>
        /// <param name="surface">The NURBS surface to split.</param>
        /// <param name="parameter">The parameter at which to split the surface, parameter should be between 0 and 1.</param>
        /// <param name="direction">Where to split in the U or V direction of the surface.</param>
        /// <returns>If the surface is split vertically (U direction) the left side is returned as the first surface and the right side is returned as the second surface.<br/>
        /// If the surface is split horizontally (V direction) the bottom side is returned as the first surface and the top side is returned as the second surface.</returns>
        public static NurbsSurface[] Split(this NurbsSurface surface, double parameter, SplitDirection direction)
        {
            if (parameter < 0.0 || parameter > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(parameter), "The parameter is not into the domain 0.0 to 1.0.");
            }

            if (surface == null)
            {
                throw new ArgumentNullException(nameof(surface));
            }

            return Divide.SplitSurface(surface, parameter, direction);
        }
    }
}
