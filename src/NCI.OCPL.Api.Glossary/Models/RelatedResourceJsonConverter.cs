using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Glossary
{
  /// <summary>
  /// A JsonConverter for IRelatedResource items.
  /// </summary>
  public class RelatedResourceJsonConverter : JsonConverter<IRelatedResource[]>
  {

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The Utf8JsonWriter to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, IRelatedResource[] value, JsonSerializerOptions options)
    {
      // For legacy reasons, force PascalCase property names.  This prevents the converter from
      // being used in a round-trip manner, but that's not a concern for our use case.
      var pascalCaseOptions = new JsonSerializerOptions(options)
      {
        // Remove the provided naming policy to force PascalCase.
        // See the README for more information.
        PropertyNamingPolicy = null
      };

      writer.WriteStartArray();
      if(value != null)
      {
        foreach (var item in value)
        {
          // If the item is null, skip it to prevent null entries in the JSON array.
          if (item is null)
            continue;
          JsonSerializer.Serialize(writer, item, item.GetType(), pascalCaseOptions);
        }
      }
      writer.WriteEndArray();
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader to read from.</param>
    /// <param name="typeToConvert">Type of the object.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>An array of IRelatedResource items.</returns>
    public override IRelatedResource[] Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options
    )
    {
      if (reader.TokenType == JsonTokenType.Null)
        return Array.Empty<IRelatedResource>();

      if (reader.TokenType != JsonTokenType.StartArray)
        throw new JsonException("Expected start of array.");

      var resourceList = new System.Collections.Generic.List<IRelatedResource>();

      while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
      {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
          using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
          {
            var root = doc.RootElement;
            string type = null;
            if (root.TryGetProperty("type", out JsonElement typeElement))
            {
                if (typeElement.ValueKind != JsonValueKind.String)
                  throw new JsonException("Expected 'type' property to be a string.");

              type = typeElement.GetString();
            }
            else
            {
              // If there's no "type" property, skip this entry to prevent errors.
              continue;
            }

            IRelatedResource resource = null;
            string rawJson = root.GetRawText();

            switch (type?.ToLowerInvariant())
            {
              case "drugsummary":
              case "summary":
              case "external":
                resource = JsonSerializer.Deserialize<LinkResource>(rawJson, options);
                break;
              case "glossaryterm":
                resource = JsonSerializer.Deserialize<GlossaryResource>(rawJson, options);
                break;
              default:
                // Don't want to throw an exception for an unknown media type, so skip it instead
                break;
            }

            if (resource != null)
            {
              resourceList.Add(resource);
            }
          }
        }
          else
            throw new JsonException("Expected all RelatedResource entries to be objects.");
      }

      return resourceList.ToArray();
    }


  }
}