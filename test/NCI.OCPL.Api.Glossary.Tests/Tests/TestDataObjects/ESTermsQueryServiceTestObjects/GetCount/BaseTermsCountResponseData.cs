namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Base class for TermsQueryService GetCount test data when
    /// mocking Elasticsearch responses.
    /// </summary>
    public abstract class BaseTermsCountResponseData
    {
        /// <summary>
        /// Contains the name of the JSON data file to use as a mock Elasticsearch response.
        /// </summary>
        public abstract string TestFilename { get; }

        /// <summary>
        /// The expected result count.
        /// </summary>
        public abstract long ExpectedCount { get; }
    }
}