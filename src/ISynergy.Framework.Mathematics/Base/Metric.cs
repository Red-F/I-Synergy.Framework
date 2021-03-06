﻿namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    /// Class Metric.
    /// </summary>
    public static class Metric
    {
        /// <summary>
        /// Calculates surface.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="width">The width.</param>
        /// <returns>Decimal</returns>
        public static decimal Surface(decimal length, decimal width)
        {
            return length * width;
        }

        /// <summary>
        /// Calculates volume.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Decimal</returns>
        public static decimal Volume(decimal length, decimal width, decimal height)
        {
            return length * width * height;
        }

        /// <summary>
        /// Calculates density.
        /// </summary>
        /// <param name="volume">The volume.</param>
        /// <param name="weight">The weight.</param>
        /// <returns>Decimal</returns>
        public static decimal Density(decimal volume, decimal weight)
        {
            return weight / volume;
        }
    }
}
