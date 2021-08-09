using GShark.Core;
using GShark.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.ExtendedMethods
{
    public static class ExtendedEnumerable
    {
        /// <summary>
        /// Transforms a set of double into a <see cref="Vector"/>.
        /// </summary>
        /// <param name="enumerable">Sets of values.</param>
        /// <returns>A <see cref="Vector"/></returns>
        public static Vector ToVector(this IEnumerable<double> enumerable)
        {
            return new Vector(enumerable.ToList());
        }

        /// <summary>
        /// Transforms a set of double into a <see cref="KnotVector"/>.
        /// </summary>
        /// <param name="enumerable">Sets of values.</param>
        /// <returns>A <see cref="KnotVector"/></returns>
        public static KnotVector ToKnot(this IEnumerable<double> enumerable)
        {
            return new KnotVector(enumerable.ToList());
        }

        /// <summary>
        /// Obtains the unique set of elements in an array.<br/>
        /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-add-custom-methods-for-linq-queries
        /// </summary>
        /// <param name="enumerable">A collection of things.</param>
        /// <param name="comparisonFunc">Function comparing two elements of the collection. Returning true if the comparison is confirmed.</param>
        /// <returns>Collection of unique elements.</returns>
        public static List<T> Unique<T>(this IEnumerable<T> enumerable, Func<T, T, bool> comparisonFunc)
        {
            List<T> tempCollection = enumerable as List<T> ?? enumerable.ToList();
            if (tempCollection.Count == 0)
            {
                throw new InvalidOperationException("Cannot compute unique for a empty set.");
            }

            List<T> uniques = new List<T> { tempCollection[tempCollection.Count - 1] };
            tempCollection.RemoveAt(tempCollection.Count - 1);

            while (tempCollection.Count > 0)
            {
                bool isUnique = true;
                T element = tempCollection[tempCollection.Count - 1];
                tempCollection.RemoveAt(tempCollection.Count - 1);

                foreach (T unique in uniques)
                {
                    if (!comparisonFunc(element, unique))
                    {
                        continue;
                    }

                    isUnique = false;
                    break;
                }

                if (isUnique)
                {
                    uniques.Add(element);
                }
            }

            return uniques;
        }
    }
}
