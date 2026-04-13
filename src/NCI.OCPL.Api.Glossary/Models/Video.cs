using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Represents a video media item.
    /// </summary>
    public class Video : IMedia
    {
        /// <summary>
        /// Notes the media type.
        /// </summary>
        /// <value>Always MediaType.Video</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaType Type { get; set; }

        /// <summary>
        /// Where is the video hosted?
        /// </summary>
        /// <value>Always HostingTypes.youtube</value>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HostingTypes Hosting { get; set; }

        /// <summary>
        /// The CDR ID of the referenced video.
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// The video's unique identifier.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// String naming the template to use when rendering the video.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// The video's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The video's caption.
        /// </summary>
        public string Caption { get; set; }
    }
}