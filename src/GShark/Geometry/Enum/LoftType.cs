namespace GShark.Geometry.Enum
{
    /// <summary>
    /// Enum defining the type of lofted surface.
    /// </summary>
    public enum LoftType
    {
        /// <summary>
        /// Uses chord-length parameterization in the loft direction.
        /// </summary>
        Normal,

        /// <summary>
        /// The surface control points are created at the same locations as the control points of the loft input curves
        /// </summary>
        Loose,

        /// <summary>
        /// Uses square root of chord-length parameterization in the loft direction.
        /// </summary>
        Tight,

        /// <summary>
        /// The sections between the curves are straight
        /// </summary>
        Straight,

        /// <summary>
        /// The object knot vectors will be uniform.
        /// </summary>
        Uniform
    }
}
