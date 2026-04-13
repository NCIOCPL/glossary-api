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
using NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Tests for the Autosuggest Query Service.
    /// </summary>
    public class ESAutosuggestQueryServiceTest
    {
        /// <summary>
        /// Test data objects for testing "Begins with" suggestions (Contains = False.)
        /// </summary>
        public static IEnumerable<object[]> RequestBeginData => new[]
        {
            new object[] { new Autosuggest_Request_Genetics_Begin_Dar_English_HealthProfessional() },
            new object[] { new Autosuggest_Request_CGov_Begin_Aci_Spanish_Patient() },
            new object[] { new Autosuggest_Request_CGov_Contains_Cat_English_Patient() },
            new object[] { new Autosuggest_Request_Genetics_Contains_Gene_English_HealthProfessional() },
            new object[] { new Autosuggest_Request_CGov_Contains_Ablacion_Spanish_Patient() },
            new object[] { new Autosuggest_Request_Exact() }
        };

        /// <summary>
        /// Test that the request to Elasticsearch goes to the correct URI
        /// and structures the request body as expected.
        /// </summary>
        [Theory, MemberData(nameof(RequestBeginData))]
        public async Task GetSuggestions_TestBeginsRequestSetup(BaseAutosuggestRequestTestData data)
        {
            Uri esURI = null;
            HttpMethod esMethod = HttpMethod.DELETE; // Basically, something other than the expected value.

            string requestBody = null;

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                ElasticsearchTestingTools.MockEmptyResponseString,
                200,
                details =>
                {
                  esURI = details.Uri;
                  esMethod = details.HttpMethod;
                  if (details.RequestBodyInBytes != null)
                  {
                      requestBody = Encoding.UTF8.GetString(details.RequestBodyInBytes);
                  }
              });
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't really care that this returns anything (for this test), only that the intercepting connection
            // sets up the request correctly.
            Suggestion[] result = await query.GetSuggestions(data.DictionaryName, data.Audience, data.Language, data.SearchText, data.MatchType, data.Size);

            Assert.Equal("/glossaryv1/_search", esURI.AbsolutePath);
            Assert.Equal(HttpMethod.POST, esMethod);

            // Compare JSON structures for equivalence
            var actualJson = JsonNode.Parse(requestBody);
            Assert.True(JsonNode.DeepEquals(data.ExpectedData, actualJson));
        }


        /// <summary>
        /// Test that the ESAutosuggestQueryService responds correctly when ES returns an empty result set.
        /// </summary>
        [Fact]
        public async Task GetSuggestions_TestEmptyESResults()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create( ElasticsearchTestingTools.MockEmptyResponseString, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't really care about the inputs, only the result.
            Suggestion[] result = await query.GetSuggestions("Cancer.gov", AudienceType.HealthProfessional, "es", "chicken", MatchType.Contains, 200);

            Assert.Empty(result);
        }

        /// <summary>
        /// Verify that ESAutosuggestQueryService responds correctly when Elasticsearch returns an error.
        /// </summary>
        [Fact]
        public async Task GetSuggestions_TestErrorResponse()
        {
            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(string.Empty, 500);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetSuggestions("Cancer.gov", AudienceType.HealthProfessional, "es", "chicken", MatchType.Contains, 200)
            );
            Assert.Equal(500, ex.HttpStatusCode);
        }

        /// <summary>
        /// Verify that ESAutosuggestQueryService responds correctly if Elasticsearch returns a broken
        /// response.
        /// </summary>
        [Fact]
        public async Task GetSuggestions_TestInvalidResponse()
        {
            string partial = @"{
                    ""took"": 223,
                    ""timed_out"": false,
                    ""_shards"": {
                                ""total"": 1,
                        ""successful"": 1,";

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(partial, 200);
            ElasticsearchClient client = new ElasticsearchClient(connectionSettings);

            // Setup the mocked Options
            IOptions<GlossaryAPIOptions> clientOptions = GetMockOptions();

            ESAutosuggestQueryService query = new ESAutosuggestQueryService(client, clientOptions, new NullLogger<ESAutosuggestQueryService>());

            // We don't care about the inputs, only in the error response.
            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(
                () => query.GetSuggestions("Cancer.gov", AudienceType.HealthProfessional, "es", "chicken", MatchType.Contains, 200)
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

    }
}