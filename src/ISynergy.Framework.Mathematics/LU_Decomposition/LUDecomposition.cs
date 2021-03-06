﻿using ISynergy.Framework.Mathematics.Base;
using ISynergy.Framework.Mathematics.Helpers;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Class LU_Decomposition.
    /// </summary>
    public class LUDecomposition
    {
        /// <summary>
        /// The l
        /// </summary>
        private IBaseMatrix L;
        /// <summary>
        /// The u
        /// </summary>
        private IBaseMatrix U;
        /// <summary>
        /// The r
        /// </summary>
        private IBaseMatrix R;
        /// <summary>
        /// The y
        /// </summary>
        private double[] Y;

        /// <summary>
        /// Calculates the lu decomposition.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="X">The x.</param>
        public void Calculate_LUDecomposition(string source, int inputType, out double[] X)
        {
            IBaseMatrix A;
            double[] vectorB;

            if (inputType == 1)
            {
                IOHelper.ReadFile(source, out A, out vectorB);
            }
            else
            {
                IOHelper.ReadConsole(out A, out vectorB);
            }

            L = new LMatrix(A.N);
            U = new UMatrix(A.N);
            R = new UMatrix(A.N);
            Y = new double[A.N];
            X = new double[A.N];

            Calculate_LU(A, A.N, vectorB, X);
        }

        /// <summary>
        /// Calculates the lu.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="n">The n.</param>
        /// <param name="vectorB">The vector b.</param>
        /// <param name="X">The x.</param>
        private void Calculate_LU(IBaseMatrix A, int n, double[] vectorB, double[] X)
        {
            double item;

            for (var i = 0; i < n; i++)
            {
                U[i, 0] = A[i, 0];
                L[0, i] = A[0, i] / U[0, 0];
                L[i, i] = 1;
            }

            for (var i = 1; i < n; i++)
            {
                for (var j = 1; j < n; j++)
                {
                    double sum;

                    if (i >= j)
                    {
                        sum = 0;
                        for (var k = 0; k < j; k++)
                            sum += U[i, k] * L[k, j];

                        U[i, j] = A[i, j] - sum;
                    }
                    else
                    {
                        sum = 0;
                        for (var k = 0; k < i; k++)
                            sum += U[i, k] * L[k, j];

                        L[i, j] = (A[i, j] - sum) / U[i, i];
                    }
                }
            }

            for (var i = 0; i < n; i++)
            {
                item = 0;

                for (var j = 0; j < i; j++)
                {
                    item += U[i, j] * Y[j];
                }

                Y[i] = (vectorB[i] - item) / U[i, i];
            }

            for (var i = n - 1; i >= 0; i--)
            {
                double d = 0;

                for (var j = n - 1; j >= i + 1; j--)
                {
                    d += L[i, j] * X[j];
                }

                X[i] = Y[i] - d;
            }
        }
    }
}
