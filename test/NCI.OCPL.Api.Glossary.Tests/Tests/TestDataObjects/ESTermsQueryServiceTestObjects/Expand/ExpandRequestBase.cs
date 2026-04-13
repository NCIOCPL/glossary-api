using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Base class for TermsQueryService Expand test data.
    /// </summary>
    public abstract class ExpandRequestBase
    {
        /// <summary>
        /// The Dictionary to query against.
        /// </summary>
        public abstract string Dictionary { get; }

        /// <summary>
        /// The Audience to query against.
        /// </summary>
        public abstract AudienceType Audience { get; }

        /// <summary>
        /// The language code to query against.
        /// </summary>
        public abstract string LanguageCode { get; }

        /// <summary>
        /// The character to expand.
        /// </summary>
        public abstract string ExpandCharacter { get; }

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