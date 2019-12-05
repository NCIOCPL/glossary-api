namespace NCI.OCPL.Api.Glossary.Models
{
    /// <summary>
    /// Configuration options for the Glossary Term API.
    /// </summary>
    public class GlossaryAPIOptions
    {
        /// <summary>
        /// Gets or sets the alias name for the Elasticsearch Collection we will use.
        /// </summary>
        /// <value>The name of the alias.</value>
        public string AliasName { get; set; }
    }
}