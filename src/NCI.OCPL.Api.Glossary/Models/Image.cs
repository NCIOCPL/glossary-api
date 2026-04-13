using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes an Image content item.
    /// </summary>
    public class Image : IMedia
    {
        /// <summary>
        /// Type of media this class will represent.
        /// </summary>
        /// <value>Always MediaType.Image</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaType Type { get; set; }

        /// <summary>
        /// A collection of image source files.
        /// </summary>
        /// <value></value>
        public ImageSource[] ImageSources { get; set; }

        /// <summary>
        /// The CDR ID of the referenced image.
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// The image's alternate text version, suitable for displaying in an HTML alt= attribute.
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// String containing the image's caption.
        /// </summary>
        public string Caption { get; set; }
    }
}
