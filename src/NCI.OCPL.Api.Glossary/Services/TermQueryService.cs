using NCI.OCPL.Api.Glossary.Interfaces;
using Nest;

namespace NCI.OCPL.Api.Glossary.Services
{
    /// <summary>
    /// Concrete Implementation class for a Term Query service.
    /// </summary>    
    public class TermQueryService : ITermQueryService
    {
        private IElasticClient _elasticClient;

        /// <summary>
        /// One arg constructor
        /// </summary> 
        public TermQueryService(IElasticClient client){
            _elasticClient = client;
        }

        /// <summary>
        /// Get Term deatils based on the input values
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="id">The Id for the term</param>
        /// <returns>An object of GlossaryTerm</returns>        
        /// </summary>        
        public GlossaryTerm GetById(string dictionary, AudienceType audience, string language, long id){
            // TODO
            // Uncomment the below line and replace it with actual call to Elastic search
            // _elasticClient.

            return GenerateSampleTerm();
        }

       /// <summary>
        /// This temporary method will create a GlossaryTerm
        /// object to testing purpose.
        /// </summary>
        /// <returns>The GlossaryTerm</returns>
        private GlossaryTerm GenerateSampleTerm(){
            GlossaryTerm _GlossaryTerm = new GlossaryTerm();
            Pronounciation pronounciation = new Pronounciation("Pronounciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            _GlossaryTerm.Id = 10L;
            _GlossaryTerm.Language = "EN";
            _GlossaryTerm.Dictionary = "Dictionary";
            _GlossaryTerm.Audience = AudienceType.Patient;
            _GlossaryTerm.TermName = "TermName";
            _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
            _GlossaryTerm.Pronounciation = pronounciation;
            _GlossaryTerm.Definition = definition;
            _GlossaryTerm.RelatedResourceType = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary};
            return _GlossaryTerm;
        }        
    }
}