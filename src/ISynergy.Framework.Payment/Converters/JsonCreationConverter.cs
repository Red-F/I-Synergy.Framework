﻿using System;
using System.Linq;
using System.Reflection;

namespace ISynergy.Framework.Payment.Converters
{
    /// <summary>
    /// Class JsonCreationConverter.
    /// Implements the <see cref="JsonConverter" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="JsonConverter" />
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Creates the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="jObject">The j object.</param>
        /// <returns>T.</returns>
        protected abstract T Create(Type objectType, JObject jObject);

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            //Create a new reader for this jObject, and set all properties to match the original reader.
            var jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            serializer.Populate(jObjectReader, target);

            return target;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        /// <summary>
        /// Fields the exists.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="jObject">The j object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject.Properties().Any(x => string.Equals(x.Name, fieldName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
