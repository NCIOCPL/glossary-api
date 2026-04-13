using System;
using System.Collections.Generic;
using System.Text;
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
    /// Tests for ESTermsQueryServiceTest::GetById.
    /// </summary>
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
        public async Task GetById_TestAPIConnectionFailure(int returnStatus)
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
                () => termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L)
            );
        }

        /// <summary>
        /// Test receiving an invalid response from ES in GetById.
        /// </summary>
        [Fact]
        public async Task GetById_TestInvalidResponse()
        {
            InMemoryRequestInvoker conn = new InMemoryRequestInvoker(
                responseBody: Encoding.UTF8.GetBytes("Not the server you were looking for"),
                statusCode: 200,
                contentType: "text/plain"
            );

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(conn);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

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
        public async Task GetById_TestUriSetup(BaseTermsQueryTestData data)
        {
            Uri esURI = null;

            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetById/" + data.ESTermID + ".json");

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                response,
                200,
                callDetails =>
                {
                    esURI = callDetails.Uri;
                }
            );
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            // We don't actually care that this returns anything - only that the connection
            // assembles the request URI correctly.
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
        public async Task GetById_TermNotFound()
        {
            InMemoryConnection conn = new InMemoryConnection(
                responseBody: Encoding.UTF8.GetBytes(
                    @"{
                        ""_index"": ""glossaryv1"",
                        ""_id"": ""1_cancer.gov_en_patient"",
                        ""found"": false
                    }"
                ),
                statusCode: 404,
                exception: null,
                contentType: "application/json"
            );
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(conn);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

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
        public async Task GetById_DataLoading(BaseTermsQueryTestData data)
        {
            string response = TestingTools.ReadTestFile("ESTermsQueryData/GetById/" + data.ESTermID + ".json");
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(response, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> gTermsClientOptions = GetMockOptions();

            ESTermsQueryService termsClient = new ESTermsQueryService(client, gTermsClientOptions, new NullLogger<ESTermsQueryService>());

            GlossaryTerm glossaryTerm = await termsClient.GetById("cancer.gov", AudienceType.Patient, "en", 43966L);

            Assert.Equal(data.ExpectedData, glossaryTerm, new GlossaryTermComparer());
        }

    }
}