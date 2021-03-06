﻿using System.ComponentModel;
using ISynergy.Framework.Core.Fixtures;
using Xunit;

namespace ISynergy.Framework.Core.Data.Tests
{
    public class ModelTests
    {
        [Fact]
        public void ViewModelFieldNotifyPropertyChangedNotifiesOnNewValueTest()
        {
            var originalValue = "original value";

            var instance = new ModelFixture<string>(originalValue);

            Assert.IsAssignableFrom<INotifyPropertyChanged>(instance);

            var gotEvent = false;

            ((INotifyPropertyChanged)instance).PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                gotEvent = true;
                Assert.True(e.PropertyName.Equals("Value") | e.PropertyName.Equals("IsValid"), "PropertyName was wrong.");
            };

            var newValue = "new value";
            instance.Value = newValue;

            Assert.True(newValue.Equals(instance.Value), "Value didn't change.");
            Assert.True(gotEvent, "Didn't get the PropertyChanged event.");
        }

        [Fact]
        public void ViewModelFieldNotifyPropertyChangedDoesNotNotifyOnSameValueTest()
        {
            var originalValue = "original value";

            var instance = new ModelFixture<string>(originalValue);

            Assert.IsAssignableFrom<INotifyPropertyChanged>(instance);

            var gotEvent = false;

            ((INotifyPropertyChanged)instance).PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                gotEvent = true;
                Assert.True(gotEvent, "Should not get any PropertyChanged events.");
            };

            instance.Value = originalValue;

            Assert.False(gotEvent, "Should not have gotten the PropertyChanged event.");
        }

        [Fact]
        public void ViewModelFieldConstructorEmptyTest()
        {
            var instance = new ModelFixture<string>();

            Assert.True(instance.Value is null, "Value should be null.");
        }

        [Fact]
        public void ViewModelFieldConstructorWithValueTest()
        {
            var expectedValue = "expected value";

            var instance = new ModelFixture<string>(expectedValue);

            Assert.True(expectedValue.Equals(instance.Value), "Value was wrong.");
        }

        [Fact]
        public void ViewModelFieldToStringReturnsEmptyStringWhenValueIsNullTest()
        {
            var instance = new ModelFixture<string>();

            Assert.True(instance.Value is null, "Value should be null.");
            Assert.True(instance.ToString().Equals(string.Empty), "ToString() should return empty.");
        }

        [Fact]
        public void ViewModelFieldToStringTest()
        {
            var expectedValue = "expected value";

            var instance = new ModelFixture<string>(expectedValue);

            Assert.True(expectedValue.Equals(instance.Value), "Value was wrong.");
            Assert.True(expectedValue.Equals(instance.ToString()), "ToString() was wrong.");
        }
    }
}
