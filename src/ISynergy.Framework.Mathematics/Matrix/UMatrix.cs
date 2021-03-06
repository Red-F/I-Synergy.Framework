﻿using ISynergy.Framework.Mathematics.Base;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Class UMatrix.
    /// Implements the <see cref="BaseMatrix" />
    /// </summary>
    /// <seealso cref="BaseMatrix" />
    public class UMatrix : BaseMatrix
    {
        // rows of the semimatrix
        /// <summary>
        /// The rows
        /// </summary>
        private readonly Hash[] rows;

        /// <summary>
        /// Constructor of matrix
        /// </summary>
        /// <param name="n">order of the matrix</param>
        public UMatrix(int n)
            : base(n)
        {
            rows = new Hash[n];

            for (var i = 0; i < n; i++)
            {
                rows[i] = new Hash();
            }
        }

        /// <summary>
        /// Override method for index in matrix
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns>return value for row and col</returns>
        public override double this[int row, int col]
        {
            get
            {
                return rows[row].GetValue(col);
            }
            set
            {
                rows[row].SetValue(col, value);
            }
        }
    }
}
