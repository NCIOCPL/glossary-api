
namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for the TermsQueryService GetCount method with a reasonable expected count of 8458.
    /// </summary>
    public class Terms_GetCount_Response_Realistic : BaseTermsCountResponseData
    {
        /// <inheritdoc />
        public override string TestFilename => "realistic.json";

        /// <inheritdoc />
        public override long ExpectedCount => 8458;

    }
}
