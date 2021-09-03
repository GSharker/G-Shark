namespace GShark.Enumerations
{
    /// <summary>
    /// Enum defining the direction of a surface for evaluation.
    /// </summary>
    public enum EvaluateSurfaceDirection
    {
        Unknown = 0,

        /// <summary>
        /// The U direction of a surface.
        /// </summary>
        U = SurfaceDirection.U,

        /// <summary>
        /// The V direction of a surface.
        /// </summary>
        V = SurfaceDirection.V,

        /// <summary>
        /// The normal direction of a surface.
        /// </summary>
        Normal
    }
}