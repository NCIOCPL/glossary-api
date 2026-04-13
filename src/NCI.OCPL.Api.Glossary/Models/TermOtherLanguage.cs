namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Represents a glossary term in another language
    /// </summary>
    public class TermOtherLanguage
    {
        /// <summary>
        /// Gets or sets the Language for the Glossary Term
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the TermName for the translation
        /// </summary>
        public string TermName { get; set; }

        /// <summary>
        /// If available, the translation's human readable name, rendered in a URL-friendly format.
        /// </summary>
        /// <value>Empty string if no human-readable name is available.</value>
        public string PrettyUrlName{ get; set; }
    }
}