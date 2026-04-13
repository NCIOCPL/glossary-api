namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Base class for test data for TermsQueryService::GetAll.
    /// </summary>
    public abstract class GetAllTermsQueryTestData
    {
        /// <summary>
        /// The object expected to be returned from GetAll.
        /// </summary>
        public abstract GlossaryTermResults ExpectedData { get; }

        /// <summary>
        /// The type of GetAll test we are performing.
        /// </summary>
        public abstract string GetAllTestType { get; }
    }
}