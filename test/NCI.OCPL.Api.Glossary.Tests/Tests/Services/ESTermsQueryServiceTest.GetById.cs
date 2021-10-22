using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
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

        public static IEnumerable<object[]> GetByIdData => new[] {
            new object[] { new GetById_43966_NoMediaNoResources() },
            new object[] { new GetById_44058_VideoExernalLink() },
            new object[] { new GetById_44759_NoMediaGlossaryResource() },
            new object[] { new GetById_44178_NoMediaSummaryLink() },
            new object[] { new GetById_44386_NoMediaDrugSummary() },
            new object[] { new GetById_339337_HealthProfessional() },
            new object[] { new GetById_445043_ImageAndExternalLink() },
        };

        /// <summary>
        /// Test failure to connect to Elasticsearch for GetById.
        /// </summary>
        [Theory]
        [InlineData(401)]
        [InlineData(403)]
        [InlineData(500)]
        [InlineData(502)]
        [InlineData(503)]
        public async void GetById_TestAPIConnectionFailure(int returnStatus)
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("An error message"),
                statusCode: returnStatus,
                exception: null,
                contentType: "text/plain"
            );

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            await Assert.ThrowsAsync<APIInternalException>(
                () => termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L)
            );
        }

        /// <summary>
        /// Test receiving an invalid response from ES in GetById.
        /// </summary>
        [Fact]
        public async void GetById_TestInvalidResponse()
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("Not the server you were looking for"),
                statusCode: 200,
                exception: null,
                contentType: "text/plain"
            );

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            APIInternalException ex = await Assert.ThrowsAsync<APIInternalException>(
                () => termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L)
            );
        }

        /// <summary>
        /// Test that GetById URI for Elasticsearch is set up correctly.
        /// </summary>
        [Theory, MemberData(nameof(GetByIdData))]
        public async void GetById_TestUriSetup(BaseTermsQueryTestData data)
        {
            Uri esURI = null;

            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetById/" + data.ESTermID + ".json");
                res.StatusCode = 200;
                esURI = req.Uri;
            });

            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            // We don't actually care that this returns anything - only that the intercepting connection
            // sets up the request URI correctly.
            GlossaryTerm actDisplay = await termsClient.GetById(
                data.DictionaryName,
                data.Audience,
                data.Language,
                data.TermID
            );

            Assert.Equal( $"/glossaryv1/_doc/{data.ESTermID}", esURI.AbsolutePath);
        }

        /// <summary>
        /// Verify that GetById returns in the expected manner when Elasticsearch reports that the
        /// term doesn't exist.
        /// </summary>
        [Fact]
        public async void GetById_TermNotFound()
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes("{\"_index\":\"glossaryv1\",\"_type\":\"_doc\",\"_id\":\"1_cancer.gov_en_patient\",\"found\":false}"),
                statusCode: 404,
                exception: null,
                contentType: "text/plain"
            );
            // While this has a URI, it does not matter, an InMemoryConnection never requests
            // from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm result = await termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 1);

            Assert.Null(result);
        }

        /// <summary>
        /// Tests the correct loading of various data files for GetById.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Theory, MemberData(nameof(GetByIdData))]
        public async void GetById_DataLoading(BaseTermsQueryTestData data)
        {
            IElasticClient client = GetById_GetElasticClientWithData(data);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm glossaryTerm = await termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L);

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

        ///<summary>
        ///A private method to enrich data from file for GetById
        ///</summary>
        private IElasticClient GetById_GetElasticClientWithData(BaseTermsQueryTestData data)
        {
            ElasticsearchInterceptingConnection conn = new ElasticsearchInterceptingConnection();
            conn.RegisterRequestHandlerForType<Nest.GetResponse<GlossaryTerm>>((req, res) =>
            {
                //Get the file name for this round
                res.Stream = TestingTools.GetTestFileAsStream("ESTermsQueryData/GetById/" + data.ESTermID + ".json");

                res.StatusCode = 200;
            });

            //While this has a URI, it does not matter, an InMemoryConnection never requests
            //from the server.
            var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));

            var connectionSettings = new ConnectionSettings(pool, conn, sourceSerializer: JsonNetSerializer.Default);
            IElasticClient client = new ElasticClient(connectionSettings);

            return client;
        }

    }
}