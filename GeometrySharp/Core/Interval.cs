namespace GeometrySharp.Core
{
    // Note originally interval took T as type. If necessary re-introduce as a T.
    // ToDo add checks for this class.
    // ToDo add comments to the properties.
    // ToDo make the test for this class.
    /// <summary>
    /// A simple parametric data representing an "interval" between two numbers.
    /// </summary>
    public class Interval
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double ParameterAt(double normalizedParameter)
        {
            return !GeoSharpMath.IsValidDouble(normalizedParameter)
                ? -1.23432101234321E+308
                : (1.0 - normalizedParameter) * this.Min + normalizedParameter * this.Max;
        }
    }
}
