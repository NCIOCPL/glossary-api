using NCI.OCPL.Api.Glossary;
using Nest;
using System;
using System.Collections.Generic;

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
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>An object of GlossaryTerm</returns>
        /// </summary>
        public GlossaryTerm GetById(string dictionary, AudienceType audience, string language, long id, string[] requestedFields){
            // TODO
            // Uncomment the below line and replace it with actual call to Elastic search
            // _elasticClient.

            return GenerateSampleTerm(requestedFields);
        }

        /// <summary>
        /// This temporary method will create a GlossaryTerm
        /// object to testing purpose.
        /// </summary>
        /// <returns>The GlossaryTerm</returns>
        private GlossaryTerm GenerateSampleTerm(string[] requestedFields){
            GlossaryTerm _GlossaryTerm = new GlossaryTerm();
            Pronounciation pronounciation = new Pronounciation("Pronounciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            if(null==requestedFields || requestedFields.Length == 0){
                _GlossaryTerm.Id = 10L;
                _GlossaryTerm.Language = "EN";
                _GlossaryTerm.Dictionary = "Dictionary";
                _GlossaryTerm.Audience = AudienceType.Patient;
                _GlossaryTerm.TermName = "TermName";
                _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
                _GlossaryTerm.Pronounciation = pronounciation;
                _GlossaryTerm.Definition = definition;
            }
            foreach (string field in requestedFields)
            {
                if(field.Equals("Id")){
                    _GlossaryTerm.Id = 1234L;
                }else  if(field.Equals("Language",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Language = "EN";
                }else  if(field.Equals("Dictionary",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Dictionary = "Dictionary";
                }else  if(field.Equals("Audience",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Audience = AudienceType.Patient;
                }else  if(field.Equals("TermName",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.TermName = "TermName";
                }else  if(field.Equals("PrettyUrlName",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
                }else  if(field.Equals("Pronounciation",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Pronounciation = pronounciation;
                }else  if(field.Equals("Definition",StringComparison.InvariantCultureIgnoreCase)){
                    _GlossaryTerm.Definition = definition;
                }
            }

            _GlossaryTerm.RelatedResources = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary};
            return _GlossaryTerm;
        }
    }
}