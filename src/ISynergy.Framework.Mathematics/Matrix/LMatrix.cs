﻿using ISynergy.Framework.Mathematics.Base;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Class LMatrix.
    /// Implements the <see cref="BaseMatrix" />
    /// </summary>
    /// <seealso cref="BaseMatrix" />
    public class LMatrix : BaseMatrix
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
        public LMatrix(int n)
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
                if (row == col)
                {
                    return 1;
                }
                else
                {
                    return rows[row].GetValue(col);
                }
            }
            set
            {
                if (row == col)
                {
                    return;
                }
                else
                {
                    rows[row].SetValue(col, value);
                }
            }
        }
    }
}
