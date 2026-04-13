using System;
using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Describes an Image file source.
    /// </summary>
    public class ImageSource
    {
        /// <summary>
        /// The logical size.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// The image's source's URI.
        /// </summary>
        public Uri Src { get; set; }
    }
}
