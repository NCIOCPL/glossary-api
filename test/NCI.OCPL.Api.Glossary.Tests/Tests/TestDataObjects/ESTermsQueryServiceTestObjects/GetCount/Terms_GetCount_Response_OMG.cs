
namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for the TermsQueryService GetCount method, representing a response with an extremely high count of results.
    /// </summary>
    /// <remarks>
    /// This test data is meant to represent an extreme case where the count of results exceeds any
    /// rational value. This is not a realistic scenario for our application,
    /// if it ever does come back with such a count, something big is probably wrong. 😉
    /// </remarks>
    public class Terms_GetCount_Response_OMG : BaseTermsCountResponseData
    {
        /// <inheritdoc />
        public override string TestFilename => "omg.json";

        /// <inheritdoc />
        public override long ExpectedCount => 2147483647;

    }
}
