using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GeometrySharp.Core
{
    public class Transform : List<IList<double>>
    {
        /// <summary>
        /// Initializes a 4 x 4 transformation matrix.
        /// All the values are set to zero.
        /// </summary>
        public Transform()
        {
            this.AddRange(Matrix.Construct(4,4));
        }

        /// <summary>
        /// A new identity transformation matrix. An identity matrix defines no transformation.
        /// The diagonal is (1,1,1,1)
        /// </summary>
        /// <returns>Gets the identity transformation matrix.</returns>
        public static Transform Identity()
        {
            var transform = new Transform
            {
                [0] = { [0] = 1 },
                [1] = { [1] = 1 },
                [2] = { [2] = 1 },
                [3] = { [3] = 1 },
            };
            return transform;
        }
    }
}
