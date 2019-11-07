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
    }
}
