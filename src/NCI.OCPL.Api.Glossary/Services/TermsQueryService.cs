using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace NCI.OCPL.Api.Glossary.Services
{

    /// <summary>
    /// Elasticsearch implementation of the service for retrieveing multiple
    /// GlossaryTerm objects.
    /// </summary>
    public class TermsQueryService : ITermsQueryService
    {

        private IElasticClient _elasticClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TermsQueryService(IElasticClient client)
        {
            _elasticClient = client;
        }

        /// <summary>
        /// Retrieves a portion of the overall set of glossary terms for a given combination of dictionary, audience, and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="requestedFields">The fields to retrieve.  If not specified, defaults to TermName, Pronunciation, and Definition.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        public async Task<GlossaryTermResults> getAll(string dictionary, AudienceType audience, string language, int size, int from, string[] requestedFields)
        {
            // Dummy return for now.
            GlossaryTermResults results = new GlossaryTermResults()
            {
                Meta = new ResultsMetadata()
                {
                    TotalResults = 200,
                    From = 20
                },
                Links = new Metalink()
                {
                    Self = new System.Uri("https://www.cancer.gov")
                },
                Results = new GlossaryTerm[]
                {
                    new GlossaryTerm()
                    {
                        Id =43966,
                        Language = "en",
                        Dictionary = "Cancer.gov",
                        Audience = AudienceType.HealthProfessional,
                        TermName = "stage II cutaneous T-cell lymphoma",
                        PrettyUrlName = "stage-ii-cutaneous-t-cell-lymphoma",
                        Pronounciation = new Pronounciation()
                        {
                            Key = "kyoo-TAY-nee-us T-sel lim-FOH-muh",
                            Audio = "https://www.cancer.gov/PublishedContent/Media/CDR/media/703959.mp3"
                        },
                        Definition = new Definition()
                        {
                            Html = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells.",
                            Text = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells."
                        }
                    },
                    new GlossaryTerm()
                    {
                        Id =43971,
                        Language = "en",
                        Dictionary = "Cancer.gov",
                        Audience = AudienceType.Patient,
                        TermName = "bcl-2 antisense oligodeoxynucleotide G3139",
                        PrettyUrlName = "bcl-2-antisense-oligodeoxynucleotide-g3139",
                        Pronounciation = new Pronounciation()
                        {
                            Key = "AN-tee-sents AH-lih-goh-dee-OK-see-NOO-klee-oh-tide",
                            Audio = "https://www.cancer.gov/PublishedContent/Media/CDR/media/703968mp3"
                        },
                        Definition = new Definition()
                        {
                            Html = "A substance being studied in the treatment of cancer. It may kill cancer cells by blocking the production of a protein that makes cancer cells live longer and by making them more sensitive to anticancer drugs. It is a type of antisense oligodeoxyribonucleotide. Also called augmerosen, Genasense, and oblimersen sodium.",
                            Text = "A substance being studied in the treatment of cancer. It may kill cancer cells by blocking the production of a protein that makes cancer cells live longer and by making them more sensitive to anticancer drugs. It is a type of antisense oligodeoxyribonucleotide. Also called augmerosen, Genasense, and oblimersen sodium."
                        }
                    }
                }
            };

            return results;
        }

        /// <summary>
        /// Search for Terms based on the search criteria.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The search query</param>
        /// <param name="matchType">Defines if the search should begin with or contain the key word</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>A list of GlossaryTerm</returns>        
        /// </summary>
        public async Task<List<GlossaryTerm>> Search(string dictionary, AudienceType audience, string language, string query,string matchType, int size, int from, string[] requestedFields)
        {
            // Temporary Solution till we have Elastic Search
            List<GlossaryTerm> glossaryTermList = new List<GlossaryTerm>();
            glossaryTermList.Add(GenerateSampleTerm(requestedFields));
            glossaryTermList.Add(GenerateSampleTerm(requestedFields));

            return glossaryTermList;
        }    


        /// <summary>
        /// Search for Terms based on the search criteria.
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="query">The search query</param>
        /// <param name="matchType">Defines if the search should begin with or contain the key word</param>
        /// <param name="size">Defines the size of the search</param>
        /// <param name="from">Defines the Offset for search</param>
        /// <param name="requestedFields"> The list of fields that needs to be sent in the response</param>
        /// <returns>A list of GlossaryTerm</returns>        
        /// </summary>
        public async Task<List<GlossaryTerm>> Expand(string dictionary, AudienceType audience, string language, string query,string matchType, int size, int from, string[] requestedFields)
        {
            // Temporary Solution till we have Elastic Search
            List<GlossaryTerm> glossaryTermList = new List<GlossaryTerm>();
            glossaryTermList.Add(GenerateSampleTerm(requestedFields));
            glossaryTermList.Add(GenerateSampleTerm(requestedFields));

            return glossaryTermList;
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
            _GlossaryTerm.Id = 7890L;
            _GlossaryTerm.Language = "EN";
            _GlossaryTerm.Dictionary = "Dictionary";
            _GlossaryTerm.Audience = AudienceType.Patient;
            _GlossaryTerm.TermName = "TermName";
            _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
            _GlossaryTerm.Pronounciation = pronounciation;
            _GlossaryTerm.Definition = definition;
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
