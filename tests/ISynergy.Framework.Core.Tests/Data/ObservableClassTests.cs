﻿using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Xunit;

namespace ISynergy.Framework.Core.Data.Tests
{
    public class ObservableClassTests
    {
        // Check when object is initialized that it's clean.
        [Fact]
        public void CheckIfObjectAfterInitializationIsClean_1()
        {
            var product = new Product
            {
                Name = "Test1"
            };
            product.MarkAsClean();

            Assert.False(product.IsDirty);
        }

        [Fact]
        public void CheckIfObjectAfterInitializationIsClean_2()
        {
            var product = new Product
            {
                Name = "Test2"
            };
            product.MarkAsClean();

            Assert.False(product.IsDirty);
        }

        [Fact]
        public void CheckIfObjectAfterInitializationIsClean_3()
        {
            var product = new Product(
                Guid.NewGuid(),
                "Test3",
                1,
                100);

            Assert.False(product.IsDirty);
        }

        // Check when object is initialized that it's not dirty
        [Fact]
        public void CheckIfObjectAfterInitializationIsDirty_1()
        {
            var product = new Product
            {
                Name = "Test1"
            };

            Assert.True(product.IsDirty);
        }

        [Fact]
        public void CheckIfObjectAfterInitializationIsDirty_2()
        {
            var product = new Product { Name = "Test2" };
            product.Date = DateTimeOffset.Now;

            Assert.True(product.IsDirty);
        }

        [Fact]
        public void CheckIfObjectAfterInitializationIsDirty_3()
        {
            var product = new Product(
                Guid.NewGuid(),
                "Test3",
                1,
                100)
            {
                Date = null
            };

            Assert.True(product.IsDirty);
        }

        // Check when object is initialized that all properties are added to Property dictionary
    }
}
