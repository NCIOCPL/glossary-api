using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Subclass of IMedia catering to Images
    /// </summary>
    public class Image : IMedia
    {
        /// <summary>
        /// Type of media this class will represent.
        /// It will be Image in this case
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// no arg constructor
        /// </summary>
        public Image(){

        }

        /// <summary>
        /// Gets or sets the reference
        /// </summary>
         [Keyword(Name = "ref")]
        public string Ref { get; set; }

        /// <summary>
        /// Gets or sets the alternate name
        /// </summary>
        [Keyword(Name = "alt")]
        public string Alt { get; set; }

        /// <summary>
        /// Gets or sets the caption
        /// </summary>
        [Keyword(Name = "caption")]
        public string[] Caption { get; set; }
        
        /// <summary>
        /// Gets or sets the template
        /// </summary>
        [Keyword(Name = "template")]
        public string Template { get; set; }
    }
}