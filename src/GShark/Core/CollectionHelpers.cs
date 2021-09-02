using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.Core
{
    /// <summary>
    /// Provides utility functions to create and manipulate sets of numbers or points.<br/>
    /// Example: range, numerical linear subdivisions and boolean operations.
    /// </summary>
    public static class CollectionHelpers
    {
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
        public static List<List<T>> Transpose2DArray<T>(List<List<T>> data)
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
