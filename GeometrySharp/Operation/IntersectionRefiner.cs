using GeometrySharp.Core.IntersectionResults;
using GeometrySharp.Geometry;
using GeometrySharp.Optimization;

namespace GeometrySharp.Operation
{
    internal static class IntersectionRefiner
    {
        /// <summary>
        /// Refine an intersection pair for two curves given an initial guess. This is an unconstrained minimization,
        /// so the caller is responsible for providing a very good initial guess.
        /// </summary>
        /// <param name="crv0">The first curve.</param>
        /// <param name="crv1">The second curve.</param>
        /// <param name="firstGuess">The first guess parameter.</param>
        /// <param name="secondGuess">The second guess parameter.</param>
        /// <param name="tolerance">The value tolerance for the intersection.</param>
        /// <returns>The results collected into the object <see cref="CurvesIntersectionResult"/>.</returns>
        internal static CurvesIntersectionResult CurvesWithEstimation(NurbsCurve crv0, NurbsCurve crv1,
            double firstGuess, double secondGuess, double tolerance)
        {
            IObjectiveFunction objectiveFunctions = new CurvesIntersectionObjectives(crv0, crv1);
            Minimizer min = new Minimizer(objectiveFunctions);
            MinimizationResult solution = min.UnconstrainedMinimizer(new Vector3 { firstGuess, secondGuess }, tolerance * tolerance);

            Vector3 pt1 = Evaluation.CurvePointAt(crv0, solution.SolutionPoint[0]);
            Vector3 pt2 = Evaluation.CurvePointAt(crv1, solution.SolutionPoint[1]);

            return new CurvesIntersectionResult(pt1, pt2, solution.SolutionPoint[0], solution.SolutionPoint[1]);
        }

        /// <summary>
        /// Refine an intersection between a curve and a plane given an initial guess. This is an unconstrained minimization,
        /// so the caller is responsible for providing a very good initial guess.
        /// </summary>
        /// <param name="crv">The curve to intersect.</param>
        /// <param name="plane">The plane to intersect with the curve.</param>
        /// <param name="firstGuess">The first guess parameter.</param>
        /// <param name="secondGuess">The second guess parameter.</param>
        /// <param name="tolerance">The value tolerance for the intersection.</param>
        /// <returns>The results collected into the object <see cref="CurvePlaneIntersectionResult"/>.</returns>
        internal static CurvePlaneIntersectionResult CurvePlaneWithEstimation(NurbsCurve crv, Plane plane,
            double firstGuess, double secondGuess, double tolerance)
        {
            IObjectiveFunction objectiveFunctions = new CurvePlaneIntersectionObjectives(crv, plane);
            Minimizer min = new Minimizer(objectiveFunctions);
            MinimizationResult solution = min.UnconstrainedMinimizer(new Vector3 { firstGuess, secondGuess }, tolerance * tolerance);

            Vector3 pt = crv.PointAt(solution.SolutionPoint[0]);
            (double u, double v) parameters = plane.ClosestParameters(pt);
            Vector3 uv = new Vector3{ parameters.u, parameters.v, 0.0};

            return new CurvePlaneIntersectionResult(pt, solution.SolutionPoint[0], uv);
        }
    }
}
