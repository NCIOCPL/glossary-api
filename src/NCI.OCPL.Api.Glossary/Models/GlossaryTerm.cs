using System.Text.Json.Serialization;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// The GlossaryTerm class
    /// </summary>
    public class GlossaryTerm
    {

        /// <summary>
        /// Gets or sets the Id for the Glossary Term
        /// </summary>
        public long TermId { get; set; }

        /// <summary>
        /// Gets or sets the Language for the Glossary Term
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the Dictionary for the Glossary Term
        /// </summary>
        public string Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the AudienceType for the Glossary Term
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AudienceType Audience { get; set; }

        /// <summary>
        /// Gets or sets the TermName for the Glossary Term
        /// </summary>
        public string TermName { get; set; }

        /// <summary>
        /// Gets or sets the FirstLetter for the Glossary Term
        /// </summary>
        public string FirstLetter { get; set; }

        /// <summary>
        /// Gets or sets the prettyUrlName for the Glossary Term
        /// </summary>
        public string  PrettyUrlName { get; set; }

        /// <summary>
        /// Gets or sets the pronunciation for the Glossary Term
        /// </summary>
        public Pronunciation Pronunciation  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glossary Term
        /// </summary>
        public Definition Definition  { get; set; }

        /// <summary>
        /// Gets or sets the translations of this term.
        /// </summary>
        public TermOtherLanguage[] OtherLanguages { get; set; } = new TermOtherLanguage[] { };

        /// <summary>
        /// Gets or sets the Definition for the Glossary Term
        /// </summary>
        [JsonConverter(typeof(RelatedResourceJsonConverter))]
        public IRelatedResource[] RelatedResources  { get; set; } = new IRelatedResource[] { };

        /// <summary>
        /// Gets or sets the Definition for the Glossary Term
        /// </summary>
        [JsonConverter(typeof(MediaJsonConverter))]
        public IMedia[] Media  { get; set; } = new IMedia[] { };

        /// <summary>
        /// no arg constructor
        /// </summary>
        public GlossaryTerm() {}
    }
}
