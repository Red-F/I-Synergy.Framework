﻿using System;
using System.Reflection;
using ISynergy.Framework.UI.Behaviors.Base;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Framework.UI.Behaviors
{
    /// <summary>
    /// Class MultiBindingBehavior.
    /// Implements the <see cref="BehaviorBase{FrameworkElement}" />
    /// </summary>
    /// <seealso cref="BehaviorBase{FrameworkElement}" />
    [ContentProperty(Name = nameof(Items))]
    [TypeConstraint(typeof(FrameworkElement))]
    public class MultiBindingBehavior : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Gets the <see cref="MultiBindingItem" /> collection within this <see cref="MultiBindingBehavior" /> instance.
        /// </summary>
        /// <value>One or more <see cref="MultiBindingItem" /> objects.</value>
        public MultiBindingItemCollection Items
        {
            get { return (MultiBindingItemCollection)GetValue(ItemsProperty); }
            private set { SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="Items" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(MultiBindingItemCollection), typeof(MultiBindingBehavior), null);

        /// <summary>
        /// Gets or sets the path to the binding source property.
        /// </summary>
        /// <value>The path to the binding source property.</value>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="PropertyName" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(MultiBindingBehavior), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the converter to use to convert the source values to or from the target value.
        /// </summary>
        /// <value>A resource reference to a class that implements the <see cref="IValueConverter" /> interface, which includes implementations of the <see cref="IValueConverter.Convert" /> and <see cref="IValueConverter.ConvertBack" /> methods.</value>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="Converter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register(nameof(Converter), typeof(IValueConverter), typeof(MultiBindingBehavior), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// Gets or sets an optional parameter to pass to the converter as additional information.
        /// </summary>
        /// <value>A value of the type expected by the converter, which might be an object element or a string depending on the definition and XAML capabilities both of the property type being used and of the implementation of the converter.</value>
        public object ConverterParameter
        {
            get { return GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="ConverterParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register(nameof(ConverterParameter), typeof(object), typeof(MultiBindingBehavior), new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// </summary>
        /// <value>A value that indicates the direction of the data flow in the binding.</value>
        public BindingMode Mode
        {
            get { return (BindingMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Identifier for the <see cref="Mode" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(BindingMode), typeof(MultiBindingBehavior), new PropertyMetadata(BindingMode.OneWay, OnPropertyChanged));

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiBindingBehavior = (MultiBindingBehavior)d;

            multiBindingBehavior.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiBindingBehavior" /> class.
        /// </summary>
        public MultiBindingBehavior()
        {
            Items = new MultiBindingItemCollection();
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" /></remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            Update();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (AssociatedObject is null || string.IsNullOrEmpty(PropertyName))
            {
                return;
            }

            var targetProperty = PropertyName;
            Type targetType;

            if (targetProperty.Contains("."))
            {
                var propertyNameParts = targetProperty.Split('.');

                targetType = Type.GetType(string.Format("Windows.UI.Xaml.Controls.{0}, Windows",
                    propertyNameParts[0]));

                targetProperty = propertyNameParts[1];
            }
            else
            {
                targetType = AssociatedObject.GetType();
            }

            PropertyInfo targetDependencyPropertyField = null;

            while (targetDependencyPropertyField is null && targetType != null)
            {
                var targetTypeInfo = targetType.GetTypeInfo();

                targetDependencyPropertyField = targetTypeInfo.GetDeclaredProperty(targetProperty + "Property");

                targetType = targetTypeInfo.BaseType;
            }

            var targetDependencyProperty = (DependencyProperty)targetDependencyPropertyField.GetValue(null);

            var binding = new Binding()
            {
                Path = new PropertyPath(nameof(MultiBindingItem.Value)),
                Source = Items,
                Converter = Converter,
                ConverterParameter = ConverterParameter,
                Mode = Mode
            };

            BindingOperations.SetBinding(AssociatedObject, targetDependencyProperty, binding);
        }
    }
}
