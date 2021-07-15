using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// RelatedResourceType enum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum RelatedResourceType
    {
        /// <summary>
        /// The resource is a PDQ Drug Information Summary.
        /// </summary>
        DrugSummary,

        /// <summary>
        /// The resource is a PDQ Cancer Information Summary.
        /// </summary>
        Summary,

        /// <summary>
        /// The resource is an external link.
        /// </summary>
        External,

        /// <summary>
        /// The resource is a PDQ GlossaryTerm.
        /// </summary>
        GlossaryTerm
    }
}