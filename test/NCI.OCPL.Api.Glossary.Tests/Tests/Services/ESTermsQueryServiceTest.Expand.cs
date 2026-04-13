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
    /// Tests for ESTermsQueryServiceTest::Expand.
    /// </summary>
    public partial class ESTermsQueryServiceTest
    {

        public static IEnumerable<object[]> ExpandData => new[] {
            new object[] { new Expand_S() },
            new object[] { new Expand_NoResults() }
        };

        /// <summary>
        /// Test failure to connect to and retrieve response from API in GetById
        /// </summary>
        [Fact]
        public async Task Expand_TestAPIConnectionFailure()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, 500);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "a", 10, 0, false));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test failure to connect to ES or receiving an invalid response from ES in GetById.
        /// </summary>
        [Fact]
        public async Task Expand_TestInvalidResponse()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, 0);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "a", 10, 0, true));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        public static IEnumerable<object[]> ExpandRequestData => new[] {
            new object[] { new ExpandRequestDefaultFields() },
            new object[] { new ExpandRequestAdditionalInfo() }
        };

        /// <summary>
        /// Test that Expand Request for Elasticsearch is set up correctly.
        /// </summary>
        [Theory, MemberData(nameof(ExpandRequestData))]
        public async Task Expand_TestRequestSetup(ExpandRequestBase data)
        {
            string requestBody = null;

            string response = TestingTools.ReadTestFile("ESTermsQueryData/Expand/expand_response_results.json");

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                response,
                200,
                callDetails =>
                {
                    if (callDetails.RequestBodyInBytes != null)
                    {
                        requestBody = Encoding.UTF8.GetString(callDetails.RequestBodyInBytes);
                    }
                }
            );

            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            var results = await termsClient.Expand(data.Dictionary, data.Audience, data.LanguageCode, data.ExpandCharacter, data.Size, data.From, data.IncludeAdditionalInfo);

            var actualJson = JsonNode.Parse(requestBody);
            Assert.True(JsonNode.DeepEquals(data.ExpectedRequest, actualJson));
        }

        /// <summary>
        /// Tests the correct deserializing of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(ExpandData))]
        public async Task Expand_DataLoading(ExpandTermsQueryTestData data)
        {
            ElasticsearchClient client = Expand_GetElasticClientWithData(data.ExpandTestType);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTermResults glossaryTermResults = await termsClient.Expand("Cancer.gov", AudienceType.Patient, "en", "s", 5, 0, false);

            Assert.Equal(data.ExpectedData.Results, glossaryTermResults.Results, new GlossaryTermComparer());
            Assert.Equal(data.ExpectedData.Meta.TotalResults, glossaryTermResults.Meta.TotalResults);
            Assert.Equal(data.ExpectedData.Meta.From, glossaryTermResults.Meta.From);
        }


        ///<summary>
        ///A private method to enrich data from file for GetById
        ///</summary>
        private ElasticsearchClient Expand_GetElasticClientWithData(string expandTestType)
        {
            string response = TestingTools.ReadTestFile("ESTermsQueryData/Expand/expand_response_" + expandTestType + ".json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                response,
                200
            );
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            return client;
        }

    }
}