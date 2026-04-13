
namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for the TermsQueryService GetCount method with an expected count of 0.
    /// </summary>
    public class Terms_GetCount_Response_Zero : BaseTermsCountResponseData
    {
        /// <inheritdoc />
        public override string TestFilename => "zero.json";

        /// <inheritdoc />
        public override long ExpectedCount => 0;

    }
}
