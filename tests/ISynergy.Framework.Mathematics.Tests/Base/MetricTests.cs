﻿using Xunit;

namespace ISynergy.Framework.Mathematics.Base.Tests
{
    /// <summary>
    /// Class MetricTests.
    /// </summary>
    public class MetricTests
    {
        /// <summary>
        /// Defines the test method Surface100Test.
        /// </summary>
        [Fact]
        public void Surface100Test()
        {
            var result = Metric.Surface(10, 10);
            Assert.Equal(100, result);
        }

        /// <summary>
        /// Defines the test method SurfaceDecimalTest.
        /// </summary>
        [Fact]
        public void SurfaceDecimalTest()
        {
            var result = Metric.Surface(2.5m, 7.3m);
            Assert.Equal(18.25m, result);
        }

        /// <summary>
        /// Defines the test method Volume1000Test.
        /// </summary>
        [Fact]
        public void Volume1000Test()
        {
            var result = Metric.Volume(10, 10, 10);
            Assert.Equal(1000, result);
        }

        /// <summary>
        /// Defines the test method VolumeDecimalTest.
        /// </summary>
        [Fact]
        public void VolumeDecimalTest()
        {
            var result = Metric.Volume(2.5m, 7.3m, 5.7m);
            Assert.Equal(104.025m, result);
        }

        /// <summary>
        /// Defines the test method DensityTest.
        /// </summary>
        [Fact]
        public void DensityTest()
        {
            var result = Metric.Density(1000m, 100m);
            Assert.Equal(0.1m, result);
        }
    }
}
