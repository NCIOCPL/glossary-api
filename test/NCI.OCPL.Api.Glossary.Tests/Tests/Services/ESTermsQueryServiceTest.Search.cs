using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Xunit;

using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;
using NCI.OCPL.Api.Common;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Tests for ESTermsQueryServiceTest::Search.
    /// </summary>
    public partial class ESTermsQueryServiceTest
    {

        public static IEnumerable<object[]> SearchData => new[] {
            new object[] { new Search_S() },
            new object[] { new Search_NoResults() }
        };

        /// <summary>
        /// Test failure to connect to and retrieve response from API in Search
        /// </summary>
        [Fact]
        public async Task Search_TestAPIConnectionFailure()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, 500);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, false));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test failure to connect to ES or receiving an invalid response from ES in Search.
        /// </summary>
        [Fact]
        public async Task Search_TestInvalidResponse()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, 0);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 10, 0, false));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        public static IEnumerable<object[]> SearchRequestData => new[]
        {
            new object[]{ new Terms_Search_Request_Begins() },
            new object[]{ new Terms_Search_Request_Contains() },
            new object[]{ new Terms_Search_Request_Exact() }
        };

        /// <summary>
        /// Test that Search Requests for Elasticsearch are structured correctly.
        /// </summary>
        [Theory, MemberData(nameof(SearchRequestData))]
        public async Task Search_TestRequestSetup(Terms_Search_Request_Base data)
        {
            string actualRequest = null;

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                ElasticsearchTestingTools.MockEmptyResponseString,
                200,
                callDetails =>
                {
                    actualRequest = Encoding.UTF8.GetString(callDetails.RequestBodyInBytes);
                }
            );
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            var results = await termsClient.Search(data.Dictionary, data.Audience, data.LangCode, data.SearchTerm, data.MatchType, data.Size, data.From, data.IncludeAdditionalInfo);

            JsonNode actualRequestJson = JsonNode.Parse(actualRequest);
            Assert.True(JsonNode.DeepEquals(data.ExpectedRequest, actualRequestJson));
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(SearchData))]
        public async Task Search_DataLoading(SearchTermsQueryTestData data)
        {
            string response = TestingTools.ReadTestFile("ESTermsQueryData/Search/search_response_" + data.SearchTestType + ".json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(response, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTermResults glossaryTermResults = await termsClient.Search("Cancer.gov", AudienceType.Patient, "en", "chicken", MatchType.Begins, 5, 0, false);

            Assert.Equal(data.ExpectedData.Results, glossaryTermResults.Results, new GlossaryTermComparer());
            Assert.Equal(data.ExpectedData.Meta.TotalResults, glossaryTermResults.Meta.TotalResults);
            Assert.Equal(data.ExpectedData.Meta.From, glossaryTermResults.Meta.From);
        }

    }
}