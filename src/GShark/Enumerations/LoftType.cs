namespace GShark.Enumerations
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
        /// The surface control points are created at the same locations as the control points of the loft input curves.
        /// </summary>
        Loose,
    }
}
