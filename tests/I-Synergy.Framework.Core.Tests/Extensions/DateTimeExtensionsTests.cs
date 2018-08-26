﻿using ISynergy.Library;
using System;
using Xunit;

namespace ISynergy.Extensions.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        [Trait(nameof(DateTimeExtensions), Test.Unit)]
        public void ToStartOfDayTest()
        {
            DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToStartOfDay();
            Assert.Equal(new DateTime(1975, 10, 29, 0, 0, 0), result);
        }

        [Fact]
        [Trait(nameof(DateTimeExtensions), Test.Unit)]
        public void ToEndOfDayTest()
        {
            DateTime result = new DateTime(1975, 10, 29, 14, 43, 35).ToEndOfDay();
            Assert.Equal(new DateTime(1975, 10, 29, 23, 59, 59, 999), result);
        }

        [Fact]
        [Trait(nameof(DateTimeExtensions), Test.Unit)]
        public void ToStartOfYearTest()
        {
            DateTime result = new DateTime().ToStartOfYear(1975);
            Assert.Equal(new DateTime(1975, 1, 1, 0, 0, 0), result);
        }

        [Fact]
        [Trait(nameof(DateTimeExtensions), Test.Unit)]
        public void ToEndOfYearTest()
        {
            DateTime result = new DateTime().ToEndOfYear(1975);
            Assert.Equal(new DateTime(1975, 12, 31, 23, 59, 59, 999), result);
        }
    }
}