using GShark.Geometry;
using GShark.Optimization;

namespace GShark.Intersection
{
    internal static class IntersectionRefiner
    {
        /// <summary>
        /// Refines an intersection pair for two curves given an initial guess.<br/>
        /// This is an unconstrained minimization, so the caller is responsible for providing a very good initial guess.
        /// </summary>
        /// <param name="crv0">The first curve.</param>
        /// <param name="crv1">The second curve.</param>
        /// <param name="firstGuess">The first guess parameter.</param>
        /// <param name="secondGuess">The second guess parameter.</param>
        /// <param name="tolerance">The value tolerance for the intersection.</param>
        /// <returns>The results collected into the object <see cref="CurvesIntersectionResult"/>.</returns>
        internal static CurvesIntersectionResult CurvesWithEstimation(NurbsBase crv0, NurbsBase crv1,
            double firstGuess, double secondGuess, double tolerance)
        {
            IObjectiveFunction objectiveFunctions = new CurvesIntersectionObjectives(crv0, crv1);
            Minimizer min = new Minimizer(objectiveFunctions);
            MinimizationResult solution = min.UnconstrainedMinimizer(new Vector { firstGuess, secondGuess }, tolerance * tolerance);

            // These are not the same points, also are used to filter where the intersection is not happening.
            Point3 pt1 = crv0.PointAt(solution.SolutionPoint[0]);
            Point3 pt2 = crv1.PointAt(solution.SolutionPoint[1]);

            return new CurvesIntersectionResult(pt1, pt2, solution.SolutionPoint[0], solution.SolutionPoint[1]);
        }

        /// <summary>
        /// Refines an intersection between a curve and a plane given an initial guess.<br/>
        /// This is an unconstrained minimization, so the caller is responsible for providing a very good initial guess.
        /// </summary>
        /// <param name="crv">The curve to intersect.</param>
        /// <param name="plane">The plane to intersect with the curve.</param>
        /// <param name="firstGuess">The first guess parameter.</param>
        /// <param name="secondGuess">The second guess parameter.</param>
        /// <param name="tolerance">The value tolerance for the intersection.</param>
        /// <returns>The results collected into the object <see cref="CurvePlaneIntersectionResult"/>.</returns>
        internal static CurvePlaneIntersectionResult CurvePlaneWithEstimation(NurbsBase crv, Plane plane,
            double firstGuess, double secondGuess, double tolerance)
        {
            IObjectiveFunction objectiveFunctions = new CurvePlaneIntersectionObjectives(crv, plane);
            Minimizer min = new Minimizer(objectiveFunctions);
            MinimizationResult solution = min.UnconstrainedMinimizer(new Vector { firstGuess, secondGuess }, tolerance * tolerance);

            Point3 pt = crv.PointAt(solution.SolutionPoint[0]);
            (double u, double v) parameters = plane.ClosestParameters(pt);
            Vector uv = new Vector { parameters.u, parameters.v, 0.0 };

            return new CurvePlaneIntersectionResult(pt, solution.SolutionPoint[0], uv);
        }
    }
}
