namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// MediaType enum
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum MediaType
    {
        /// <summary>
        /// Marks a media item as being an image.
        /// </summary>
        Image,

        /// <summary>
        /// Marks a media item as being a video.
        /// </summary>
        Video
    }
}