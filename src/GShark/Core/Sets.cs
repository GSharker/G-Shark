using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Core
{
    /// <summary>
    /// Provides utility functions to create and manipulate sets of numbers or points.<br/>
    /// Example: range, numerical linear subdivisions and boolean operations.
    /// </summary>
    public static class Sets
    {
        /// <summary>
        /// The range dimension of a set of number, or the distance between the smallest value to the biggest in the collection.<br/>
        /// https://www.statisticshowto.com/probability-and-statistics/statistics-definitions/range-statistics/
        /// </summary>
        /// <param name="a">Set of numbers.</param>
        /// <returns>The range dimension.</returns>
        public static double RangeDimension(IList<double> a)
        {
            IOrderedEnumerable<double> sortedSet = a.OrderBy(x => x);
            return sortedSet.Last() - sortedSet.First();
        }

        /// <summary>
        /// Creates a range of numbers. 
        /// </summary>
        /// <param name="domain">Numeric domain.</param>
        /// <param name="step">Number of steps</param>
        /// <returns>A range of numbers.</returns>
        public static IList<double> Range(Interval domain, int step)
        {
            if (step <= 0)
            {
                return new List<double>() { domain.T0, domain.T1 };
            }

            List<double> l = new List<double>();
            double f = 0.0;

            while (f <= step)
            {
                double normalizedParam = f / step;
                double number = domain.ParameterAt(normalizedParam);
                l.Add(number);
                ++f;
            }

            return l;
        }

        /// <summary>
        /// Computes a collection of evenly spaced numbers over a specified domain.
        /// </summary>
        /// <param name="domain">Numeric domain.</param>
        /// <param name="step">Number of steps.</param>
        /// <returns>A collection of equally spaced numbers.</returns>
        public static IList<double> LinearSpace(Interval domain, int step)
        {
            if (Math.Abs(domain.T0 - domain.T1) <= GeoSharkMath.Epsilon)
            {
                return new List<double>() { domain.T0 };
            }

            List<double> linearSpace = new List<double>();

            if (step <= 1)
            {
                linearSpace.Add(domain.T0);
                return linearSpace;
            }

            int div = step - 1;
            double delta = domain.T1 - domain.T0;
            for (int i = 0; i < step; i++)
            {
                linearSpace.Add(domain.T0 + (i * delta / div));
            }

            return linearSpace;
        }

        /// <summary>
        /// Creates a range of positive numbers, incrementing of one step and starting from 0.
        /// </summary>
        /// <param name="maxValue">The dimension of the range.</param>
        /// <returns>A range of positive numbers.</returns>
        public static IList<double> Range(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new Exception("T1 value range can not be negative or zero.");
            }

            List<double> l = new List<double>();
            double f = 0.0;

            while (f <= maxValue)
            {
                l.Add(f);
                ++f;
            }

            return l;
        }

        /// <summary>
        /// Creates a series of numbers. 
        /// </summary>
        /// <param name="start">First number in the series.</param>
        /// <param name="step">Step size for each successive number.</param>
        /// <param name="count">Number of values in the series.</param>
        /// <returns>Series of numbers.</returns>
        public static IList<double> Span(double start, double step, int count)
        {
            if (count <= 0)
            {
                throw new Exception("Count can not be negative or zero.");
            }

            List<double> l = new List<double>();
            double counter = 0.0;
            double number1 = start;

            while (counter <= count)
            {
                l.Add(number1);
                number1 += step;
                ++counter;
            }

            return l;
        }

        /// <summary>
        /// The set union of two sequences of numbers.
        /// </summary>
        /// <param name="a">First set.</param>
        /// <param name="b">Second set.</param>
        /// <returns>The set union.</returns>
        public static List<double> SetUnion(IList<double> a, IList<double> b)
        {
            if (a.Count == 0)
            {
                return b.ToList();
            }

            return b.Count == 0 ? a.ToList() : a.Union(b).ToList();
        }

        /// <summary>
        /// The set difference from two sequences of numbers.
        /// </summary>
        /// <param name="a">First set.</param>
        /// <param name="b">Second set.</param>
        /// <returns>The set difference.</returns>
        public static List<double> SetDifference(IList<double> a, IList<double> b)
        {
            if (a.Count == 0)
            {
                throw new Exception("Set difference can't be computed, the first set is empty.");
            }

            return a.Except(b).ToList();
        }

        /// <summary>
        /// Repeats data until it reaches the defined length.
        /// </summary>
        /// <param name="data">Data to repeat.</param>
        /// <param name="length">Length of the final set.</param>
        /// <returns>Set of repeated data.</returns>
        public static List<T> RepeatData<T>(T data, int length)
        {
            if (length < 0)
            {
                throw new Exception("Length can not be negative.");
            }

            List<T> list = new List<T>();
            for (int i = 0; i < length; i++)
            {
                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// Reverses a bi-dimensional collection of T data.<br/>
        /// </summary>
        /// <param name="data">The bi-dimensional collection of data.</param>
        /// <returns>The bi-dimensional collection reversed.</returns>
        public static List<List<T>> Reverse2DMatrixData<T>(List<List<T>> data)
        {
            List<List<T>> reverseData = new List<List<T>>();
            //Reverse the points matrix
            if (data.Count == 0)
            {
                return null;
            }

            int rows = data.Count;
            int columns = data[0].Count;
            for (int c = 0; c < columns; c++)
            {
                List<T> rr = new List<T>();
                for (int r = 0; r < rows; r++)
                {
                    rr.Add(data[r][c]);
                }
                reverseData.Add(rr);
            }

            return reverseData;
        }
    }
}
