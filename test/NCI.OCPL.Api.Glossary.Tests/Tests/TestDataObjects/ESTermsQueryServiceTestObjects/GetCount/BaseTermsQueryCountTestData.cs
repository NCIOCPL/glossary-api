using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Base class for test data for TermsQueryService GetCount method when verifying
    /// the generated request body.
    /// </summary>
    public abstract class BaseTermsQueryCountTestData
    {
        /// <summary>
        /// The expected request body.
        /// </summary>
        public abstract JsonNode ExpectedData { get; }

        /// <summary>
        /// The name of the dictionary for the suggestion request
        /// </summary>
        public abstract string DictionaryName { get; }

        /// <summary>
        /// The language of the suggestion request
        /// </summary>
        public abstract string Language { get; }

        /// <summary>
        /// The audience type for the suggestion request
        /// </summary>
        public abstract AudienceType Audience { get; }
    }
}