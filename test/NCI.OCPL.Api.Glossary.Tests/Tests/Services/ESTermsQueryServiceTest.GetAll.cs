using System;
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
    /// Tests for ESTermsQueryServiceTest::GetAll.
    /// </summary>
    public partial class ESTermsQueryServiceTest
    {

        public static IEnumerable<object[]> GetAllData => new[] {
            new object[] { new GetAll_S() },
            new object[] { new GetAll_NoResults() }
        };

        /// <summary>
        /// Test failure to connect to and retrieve response from API in GetById
        /// </summary>
        [Fact]
        public async Task GetAll_TestAPIConnectionFailure()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, 500);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 10, 0, false));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        public static IEnumerable<object[]> GetAll_RequestData => new[]
        {
            new object[] { new GetAll_Request_DefaultFields()},
            new object[] { new GetAll_Request_AdditionalFields()}
        };

        /// <summary>
        /// Test that GetAll Request for Elasticsearch is set up correctly.
        /// </summary>
        [Theory, MemberData(nameof(GetAll_RequestData))]
        public async Task GetAll_TestRequestSetup(GetAll_Request_Base data)
        {
            string actualRequest = null;

            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetAll/getall_response_results.json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                response,
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

            var results = await termsClient.GetAll(data.Dictionary, data.Audience, data.LangCode, data.Size, data.From, data.IncludeAdditionalInfo);

            JsonNode actualRequestJson = JsonNode.Parse(actualRequest);
            Assert.True(JsonNode.DeepEquals(data.ExpectedRequest, actualRequestJson));
        }

        /// <summary>
        /// Exercise the ESTermsQueryService GetAll method.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetAllData))]
        public async Task GetAll_DataLoading(GetAllTermsQueryTestData data)
        {
            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetAll/getall_response_" + data.GetAllTestType + ".json");
            var settings = TestingElasticsearchClientSettingsFactory.Create(
                response,
                200
            );
            ElasticsearchClient client = new ElasticsearchClient(settings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTermResults glossaryTermResults = await termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 5, 0, false);

            Assert.Equal(data.ExpectedData.Results, glossaryTermResults.Results, new GlossaryTermComparer());
            Assert.Equal(data.ExpectedData.Meta.TotalResults, glossaryTermResults.Meta.TotalResults);
            Assert.Equal(data.ExpectedData.Meta.From, glossaryTermResults.Meta.From);
        }

    }
}