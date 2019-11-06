using NCI.OCPL.Api.Glossary.Interfaces;
using Nest;

namespace NCI.OCPL.Api.Glossary.Services
{

    /// <summary>
    /// Concrete implementation of a service for working with
    /// multiple Terms.
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

    }
}
