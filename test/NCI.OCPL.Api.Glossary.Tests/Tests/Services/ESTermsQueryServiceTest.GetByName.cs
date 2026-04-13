using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Xunit;

using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;
using NCI.OCPL.Api.Common;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    ///  Tests for ESTermsQueryServiceTest::GetByName.
    /// </summary>
    public partial class ESTermsQueryServiceTest
    {

        public static IEnumerable<object[]> GetByNameData => new[] {
            new object[] { new GetByName_s_1() },
            new object[] { new GetByName_s_phase_fraction() }
        };

        /// <summary>
        /// Test failure to connect to Elasticsearch for GetByName.
        /// </summary>
        [Theory]
        [InlineData(401)]
        [InlineData(403)]
        [InlineData(500)]
        [InlineData(502)]
        [InlineData(503)]
        public async Task GetByName_TestAPIConnectionFailure(int returnStatus)
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("An error message"),
                statusCode: returnStatus,
                contentType: "text/plain"
            );

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(conn);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            await Assert.ThrowsAsync<APIInternalException>(
                () => termsClient.GetByName("cancer.gov", AudienceType.Patient, "en", "s-1")
            );
        }

        /// <summary>
        /// Test receiving an invalid response from ES in GetByName.
        /// </summary>
        [Fact]
        public async Task GetByName_TestInvalidResponse()
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("Not the server you were looking for"),
                statusCode: 200,
                contentType: "text/plain"
            );

            var  connectionSettings = TestingElasticsearchClientSettingsFactory.Create(conn);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIInternalException ex = await Assert.ThrowsAsync<APIInternalException>(
                () => termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1")
            );
        }

        /// <summary>
        /// Test that GetByName Request for Elasticsearch is set up correctly.
        /// </summary>
        [Fact]
        public async Task GetByName_TestRequestSetup()
        {
            string actualRequest = null;
            JsonNode expectedRequest = JsonNode.Parse(@"
                {
                    ""sort"": {
                        ""term_name"": {}
                    },
                    ""query"": {
                        ""bool"": {
                            ""must"": [
                                {
                                    ""term"": {
                                        ""language"": {
                                            ""value"": ""en""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""audience"": {
                                            ""value"": ""Patient""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""dictionary"": {
                                            ""value"": ""Cancer.gov""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""pretty_url_name"": {
                                            ""value"": ""s-1""
                                        }
                                    }
                                }
                            ]
                        }
                    }
                }"
            );

            Uri actualESURI = null;

            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetByName/s-1.json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                response,
                200,
                callDetails =>
                {
                    actualRequest = Encoding.UTF8.GetString(callDetails.RequestBodyInBytes);
                    actualESURI = callDetails.Uri;
                });
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            // Don't actually care that this returns anything, only that the connection set up the request correctly.
            await termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1");

            JsonNode actualRequestJson = JsonNode.Parse(actualRequest);
            Assert.True(JsonNode.DeepEquals(expectedRequest, actualRequestJson));
            Assert.Equal("/glossaryv1/_search", actualESURI.AbsolutePath);
        }

        /// <summary>
        /// Test that GetByName throws exception for multiple results.
        /// </summary>
        [Theory]
        [InlineData("getbyname_multiplehits", "errors occurred")]
        public async Task GetByName_MultipleResults(string file, string expectedMessage)
        {
            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetByName/" + file + ".json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(response, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIInternalException ex = await Assert.ThrowsAsync<APIInternalException>(() => termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1"));
            Assert.Equal(expectedMessage, ex.Message);
        }

        /// <summary>
        /// Verify that GetByName returns in the expected manner when Elasticsearch reports that the
        /// term doesn't exist.
        /// </summary>
        [Fact]
        public async Task GetByName_TermNotFound()
        {
            string response =
                  @"{
                        ""took"": 3,
                        ""timed_out"": false,
                        ""_shards"": {
                            ""total"": 1,
                            ""successful"": 1,
                            ""skipped"": 0,
                            ""failed"": 0
                        },
                        ""hits"": {
                            ""total"": {
                                ""value"": 0,
                                ""relation"": ""eq""
                            },
                            ""max_score"": null,
                            ""hits"": []
                        }
                    }";
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(response, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm result = await termsClient.GetByName("cancer.gov", AudienceType.Patient, "en", "s-1");

            Assert.Null(result);
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetByNameData))]
        public async Task GetByName_DataLoading(GetByNameTermsQueryTestData data)
        {
            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetByName/" + data.PrettyUrlName + ".json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(response, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm glossaryTerm = await termsClient.GetByName("Cancer.gov", AudienceType.Patient, "en", "s-1");

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

    }
}