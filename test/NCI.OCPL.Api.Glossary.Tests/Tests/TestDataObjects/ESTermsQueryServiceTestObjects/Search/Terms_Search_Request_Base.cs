using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Base class for testing requests made by ESTermsQueryService::Search.
    /// </summary>
    public abstract class Terms_Search_Request_Base
    {
        /// <summary>
        /// The dictionary being searched.
        /// </summary>
        public abstract string Dictionary { get; }

        /// <summary>
        /// The audience for the search.
        /// </summary>
        public abstract AudienceType Audience { get; }

        /// <summary>
        /// The language code for the search.
        /// </summary>
        public abstract string LangCode { get; }

        /// <summary>
        /// The term being searched for.
        /// </summary>
        public abstract string SearchTerm { get; }

        /// <summary>
        /// The type of match to perform.
        /// </summary>
        public abstract MatchType MatchType { get; }

        /// <summary>
        /// The number of results to return.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// The starting index into the search results.
        /// </summary>
        public abstract int From { get; }

        /// <summary>
        /// Whether to include additional information in the search results.
        /// </summary>
        public abstract bool IncludeAdditionalInfo { get; }

        /// <summary>
        /// The expected JSON request for the search.
        /// </summary>
        public abstract JsonNode ExpectedRequest { get; }
    }
}