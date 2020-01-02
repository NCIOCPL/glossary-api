using System.Security.Policy;
using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// LinkResource
    /// </summary>
    public class LinkResource : IRelatedResource
    {  
        /// <summary>
        /// Holds the url for the related resource
        /// </summary>
        [Keyword(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets and Sets the text for the related resource
        /// </summary>
        [Keyword(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Gets and Sets the Type for the related resource
        /// </summary>
        public RelatedResourceType Type  { get; set; }
    }
}