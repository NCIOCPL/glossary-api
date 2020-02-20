using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json.Linq;
using Xunit;

using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;
using NCI.OCPL.Api.Common;


namespace NCI.OCPL.Api.Glossary.Tests
{
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
        public async void GetAll_TestAPIConnectionFailure()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                res.StatusCode = 500;
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 10, 0, new string[] { }));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test failure to connect to ES or receiving an invalid response from ES in GetById.
        /// </summary>
        [Fact]
        public async void GetAll_TestInvalidResponse()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {

            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 10, 0, new string[] { }));
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Test that GetAll Request for Elasticsearch is set up correctly.
        /// </summary>
        [Fact]
        public async void GetAll_TestRequestSetup()
        {
            JObject actualRequest = null;
            JObject expectedRequest = JObject.Parse(@"
                {
                    ""from"": 0,
                    ""size"": 5,
                    ""_source"": {
                        ""includes"": [
                            ""term_id"",
                            ""language"",
                            ""dictionary"",
                            ""audience"",
                            ""term_name"",
                            ""first_letter"",
                            ""pretty_url_name"",
                            ""pronunciation"",
                            ""definition"",
                        ]
                    },
                    ""sort"": [
                        {
                            ""term_name"": {}
                        }
                    ],
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
                                }
                            ]
                        }
                    }
                }"
            );

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetAll/getall_response_results.json");

                res.StatusCode = 200;

                actualRequest = conn.GetRequestPost(req);
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            try
            {
                var results = await termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 5, 0, new string[]{"termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation"});
            }
            catch (Exception) { }

            Assert.Equal(expectedRequest, actualRequest, new JTokenEqualityComparer());
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetAllData))]
        public async void GetAll_DataLoading(GetAllTermsQueryTestData data)
        {
            IElasticClient client = GetAll_GetElasticClientWithData(data.GetAllTestType);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTermResults glossaryTermResults = await termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 5, 0, new string[]{"termId", "language", "dictionary", "audience", "termName", "firstLetter", "prettyUrlName", "definition", "pronunciation"});

            Assert.Equal(data.ExpectedData.Results, glossaryTermResults.Results, new GlossaryTermComparer());
            Assert.Equal(data.ExpectedData.Meta.TotalResults, glossaryTermResults.Meta.TotalResults);
            Assert.Equal(data.ExpectedData.Meta.From, glossaryTermResults.Meta.From);
        }

        /// <summary>
        /// Tests the correct loading of various data files.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Fact]
        public async void GetAll_ValidRequestedFields()
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {

            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            // Test null argument
            await Assert.ThrowsAsync<ArgumentNullException>(async () => {
                GlossaryTermResults glossaryTermResults = await termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 5, 0, null);
            });

            // Test null in an array
            await Assert.ThrowsAsync<ArgumentException>(async () => {
                GlossaryTermResults glossaryTermResults = await termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 5, 0, new string[] { null });
            });

            // Test bad property name
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => {
                GlossaryTermResults glossaryTermResults = await termsClient.GetAll("Cancer.gov", AudienceType.Patient, "en", 5, 0, new string[] { "chicken" });
            });
        }

        ///<summary>
        ///A private method to enrich data from file for GetById
        ///</summary>
        private IElasticClient GetAll_GetElasticClientWithData(string getAllTestType)
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.SearchResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetAll/getall_response_" + getAllTestType + ".json");

                res.StatusCode = 200;
            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn);
            IElasticClient client = new ElasticClient(connectionSettings);

            return client;
        }

    }
}