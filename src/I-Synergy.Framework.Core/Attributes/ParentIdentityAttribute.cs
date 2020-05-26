﻿using System;

namespace ISynergy.Framework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParentIdentityAttribute : Attribute
    {
        public Type PropertyType { get; set; }

        public ParentIdentityAttribute(Type propertyType)
        {
            PropertyType = propertyType;
        }
    }
}
