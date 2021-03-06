﻿using System;

namespace ISynergy.Framework.Mathematics.Generator
{
    /// <summary>
    /// Class MatrixGenerator.
    /// </summary>
    public static class MatrixGenerator
    {
        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random random;

        /// <summary>
        /// Initializes static members of the <see cref="MatrixGenerator" /> class.
        /// </summary>
        static MatrixGenerator()
        {
            random = new Random();
        }

        /// <summary>
        /// Generates the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="n">The n.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>Matrix.</returns>
        public static Matrix Generate(int m, int n, int minValue = -9, int maxValue = 9)
        {
            var matrix = new Matrix(m, n);

            for (var row = 0; row < matrix.Rows; row++)
                for (var col = 0; col < matrix.Columns; col++)
                    matrix[row, col] = random.Next(minValue, maxValue + 1);

            return matrix;
        }

        /// <summary>
        /// Generates the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>Matrix.</returns>
        public static Matrix Generate(int m, int minValue = -9, int maxValue = 9)
        {
            return Generate(m, m, minValue, maxValue);
        }

        /// <summary>
        /// Identities the matrix.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns>Matrix.</returns>
        public static Matrix IdentityMatrix(int m)
        {
            var matrix = new Matrix(m);

            for (var i = 0; i < m; i++)
                matrix[i, i] = 1;

            return matrix;
        }
    }
}
