using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Common
{
    // This is a workaround until .Net adds support for polymorphic serialization.
    // That's (currently) planned for .Net 7 https://github.com/dotnet/runtime/issues/30083#issuecomment-861529307

    // Based on code from this article: https://khalidabuhakmeh.com/serialize-interface-instances-system-text-json

    /// <summary>
    /// Serializes an object when only an interface is known.
    /// (Does not deserialize.)
    /// </summary>
    /// <typeparam name="T">The specific interface being serialized.</typeparam>

    public class InterfaceJsonConverter<T> : JsonConverter<T> where T : class
    {
        /// <summary>
        /// NOT IMPLEMENTED. DO NOT USE!
        /// Reads and converts the JSON to type T.
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns></returns>
        public override T Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes an object as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(
            Utf8JsonWriter writer,
            T value,
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (T)null, options);
                    break;
                default:
                    {
                        var type = value.GetType();
                        JsonSerializer.Serialize(writer, value, type, options);
                        break;
                    }
            }
        }
    }
}