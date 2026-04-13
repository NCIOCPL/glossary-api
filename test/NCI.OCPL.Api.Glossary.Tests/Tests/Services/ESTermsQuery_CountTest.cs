using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Moq;
using Xunit;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Tests to verify that the requests for the term count
    /// are put together in the expected manner.
    /// </summary>
    public class ESTermsQuery_CountTest
    {
        /// <summary>
        /// Test data objects for use when validating how requests to Elasticsearch are assembled.
        /// </summary>
        public static IEnumerable<object[]> RequestData => new[]
        {
            new object[] {new Terms_GetCount_Request_CGov_HealthProfessional_English() },
            new object[] {new Terms_GetCount_Request_CGov_HealthProfessional_Spanish() },
            new object[] {new Terms_GetCount_Request_CGov_Patient_English() },
            new object[] {new Terms_GetCount_Request_CGov_Patient_Spanish() },
            new object[] {new Terms_GetCount_Request_Genetics_HealthProfessional_English() },
            new object[] {new Terms_GetCount_Request_Genetics_HealthProfessional_Spanish() },
            new object[] {new Terms_GetCount_Request_Genetics_Patient_English() },
            new object[] {new Terms_GetCount_Request_Genetics_Patient_Spanish() }
        };

        /// <summary>
        /// Test data objects for use when validating how Elasticsearch responses are handled.
        /// </summary>
        public static IEnumerable<object[]> ResponseData => new[]
        {
            new object[] { new Terms_GetCount_Response_Zero() },
            new object[] { new Terms_GetCount_Response_One() },
            new object[] { new Terms_GetCount_Response_Realistic() },
            new object[] { new Terms_GetCount_Response_OMG() }
        };

        /// <summary>
        /// Verify the ES request to get the count is put together correctly.
        /// </summary>
        /// <param name="data"></param>
        [Theory, MemberData(nameof(RequestData))]
        public async Task GetCount_Request(BaseTermsQueryCountTestData data)
        {
            Uri esURI = null;
            HttpMethod esMethod = HttpMethod.DELETE; // Initialize to something other than the expected value.

            string requestBody = null;

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                MockCountResponse,
                200,
                details =>
                {
                    esURI = details.Uri;
                    esMethod = details.HttpMethod;
                    if (details.RequestBodyInBytes != null)
                    {
                        requestBody = Encoding.UTF8.GetString(details.RequestBodyInBytes);
                    }
                }
            );

            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't really care that this returns anything (for this test), only that the connection
            // sets up the request correctly.
            long result = await query.GetCount(data.DictionaryName, data.Audience, data.Language);

            Assert.Equal("/glossaryv1/_count", esURI.AbsolutePath);
            Assert.Equal(HttpMethod.POST, esMethod);

            var actualJson = JsonNode.Parse(requestBody);
            Assert.True(JsonNode.DeepEquals(data.ExpectedData, actualJson));
        }


        /// <summary>
        /// Verify the response from ES is processed correctly.
        /// </summary>
        [Theory, MemberData(nameof(ResponseData))]
        public async Task GetCount_Response(BaseTermsCountResponseData data)
        {
            string responseBody = TestingTools.ReadTestFile("ESTermsQueryData/GetCount/" + data.TestFilename);

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(responseBody, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't really care about the inputs. What matters is the return, which is controlled by the mock ES connection.
            long result = await query.GetCount("Cancer.gov", AudienceType.Patient, "es");

            Assert.Equal(data.ExpectedCount, result);
        }

        /// <summary>
        /// Verify that ESTermsQueryService responds correctly when Elasticsearch returns an error.
        /// </summary>
        [Theory]
        [InlineData(400)]
        [InlineData(403)]
        [InlineData(404)]
        [InlineData(500)]
        public async Task GetCount_ErrorResponse(int returnStatusCode)
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, returnStatusCode);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetCount("Cancer.gov", AudienceType.HealthProfessional, "es")
            );
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify that ESTermsQueryService responds correctly if Elasticsearch returns
        /// a broken response.
        /// </summary>
        [Fact]
        public async Task GetCount_InvalidResponse()
        {
            string partial =@"{
                ""count"": 8458,
                ""_shards"": {";

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(partial, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESTermsQueryService query = new ESTermsQueryService(client, clientOptions, new NullLogger<ESTermsQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetCount("Cancer.gov", AudienceType.HealthProfessional, "es")
            );
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Mock Elasticsearch configuration options.
        /// </summary>
        private IOptions<GlossaryAPIOptions> GetMockOptions()
        {
            Mock<IOptions<GlossaryAPIOptions>> clientOptions = new Mock<IOptions<GlossaryAPIOptions>>();
            clientOptions
                .SetupGet(opt => opt.Value)
                .Returns(new GlossaryAPIOptions()
                {
                    AliasName = "glossaryv1"
                }
            );

            return clientOptions.Object;
        }

        /// <summary>
        /// Simulates a count response from Elasticsearch so we
        /// have something for tests where we don't care about the response.
        /// </summary>
        private static string MockCountResponse
        {
          get {
            return @"
{
    ""count"": 42,
    ""_shards"": {
                ""total"": 1,
        ""successful"": 1,
        ""skipped"": 0,
        ""failed"": 0
    }
}";
          }
        }
    }
}