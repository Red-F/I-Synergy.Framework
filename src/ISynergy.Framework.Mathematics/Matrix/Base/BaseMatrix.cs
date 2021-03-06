﻿namespace ISynergy.Framework.Mathematics.Base
{
    /// <summary>
    /// Class BaseMatrix.
    /// Implements the <see cref="IBaseMatrix" />
    /// </summary>
    /// <seealso cref="IBaseMatrix" />
    public abstract class BaseMatrix : IBaseMatrix
    {
        // Order of matrix
        /// <summary>
        /// Gets the n.
        /// </summary>
        /// <value>The n.</value>
        public int N { get; }

        /// <summary>
        /// Gets or sets the <see cref="System.Double" /> with the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns>System.Double.</returns>
        public abstract double this[int row, int col] { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMatrix" /> class.
        /// </summary>
        /// <param name="n">The n.</param>
        protected BaseMatrix(int n)
        {
            N = n;
        }
    }
}
