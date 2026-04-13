namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Base class for test data for TermsQueryService::GetByName.
    /// </summary>
    public abstract class GetByNameTermsQueryTestData
    {
        /// <summary>
        /// The object expected to be returned from GetByName.
        /// </summary>
        public abstract GlossaryTerm ExpectedData { get; }

        /// <summary>
        /// The pretty URL name of the object.
        /// </summary>
        public abstract string PrettyUrlName { get; }
    }
}