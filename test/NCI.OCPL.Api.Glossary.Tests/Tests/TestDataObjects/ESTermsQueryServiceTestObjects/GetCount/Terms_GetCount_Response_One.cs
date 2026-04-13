
namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for the TermsQueryService GetCount method with an expected count of 1.
    /// </summary>
    public class Terms_GetCount_Response_One : BaseTermsCountResponseData
    {
        /// <inheritdoc />
        public override string TestFilename => "one.json";

        /// <inheritdoc />
        public override long ExpectedCount => 1;

    }
}
