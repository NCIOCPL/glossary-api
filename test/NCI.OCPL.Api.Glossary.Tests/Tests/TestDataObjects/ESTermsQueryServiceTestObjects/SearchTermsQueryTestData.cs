namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Base class for test data for the ESTermsQueryService Search method.
    /// </summary>
    public abstract class SearchTermsQueryTestData
    {
        /// <summary>
        /// The data structure the Search method should return when given the test data response from ES.
        /// </summary>
        public abstract GlossaryTermResults ExpectedData { get; }

        /// <summary>
        /// Gets the type of Search test we are performing.  Used for constructing the file name for the test data response from ES.
        /// </summary>
        public abstract string SearchTestType { get; }
    }
}