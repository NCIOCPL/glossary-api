using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Base class for test data for TermsQueryService::GetAll.
    /// </summary>
    public abstract class GetAll_Request_Base
    {
        /// <summary>
        /// The dictionary to query against.
        /// </summary>
        public abstract string Dictionary { get; }

        /// <summary>
        /// The audience to query against.
        /// </summary>
        public abstract AudienceType Audience { get; }

        /// <summary>
        /// The language code to query against.
        /// </summary>
        public abstract string LangCode { get; }

        /// <summary>
        /// The number of results to return.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// The starting index for the results.
        /// </summary>
        public abstract int From { get; }

        /// <summary>
        /// Whether to include additional information in the results.
        /// </summary>
        public abstract bool IncludeAdditionalInfo { get; }

        /// <summary>
        /// The expected JSON request body.
        /// </summary>
        public abstract JsonNode ExpectedRequest { get; }
    }
}
