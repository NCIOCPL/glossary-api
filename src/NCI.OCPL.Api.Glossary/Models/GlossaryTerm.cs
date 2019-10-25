namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// The GlossaryTerm class
    /// </summary>
    public class GlossaryTerm
    {

        /// <summary>
        /// Gets or sets the Id for the Glosary Term
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Language for the Glosary Term
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the Dictionary for the Glosary Term
        /// </summary>
        public string Dictionary { get; set; }

        /// <summary>
        /// Gets or sets the AudienceType for the Glosary Term
        /// </summary>
        public AudienceType Audience { get; set; }

        /// <summary>
        /// Gets or sets the TermName for the Glosary Term
        /// </summary>
        public string TermName { get; set; }

        /// <summary>
        /// Gets or sets the prettyUrlName for the Glosary Term
        /// </summary>  
        public string  PrettyUrlName { get; set; }

        /// <summary>
        /// Gets or sets the pronounciation for the Glosary Term
        /// </summary>  
        public Pronounciation Pronounciation  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary> 
        public Definition Definition  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary> 
        public RelatedResourceType[] RelatedResourceType  { get; set; }

        /// <summary>
        /// Gets or sets the Definition for the Glosary Term
        /// </summary> 
        public IMedia[] Media  { get; set; }

        /// <summary>
        /// no arg constructor
        /// </summary> 
        public GlossaryTerm() {}                
    }
}
