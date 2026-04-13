using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Moq;
using Xunit;

using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Models;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Tests for the Autosuggest Query Service, examining responses.
    /// </summary>
    public class ESAutosuggestQueryServiceResponseTest
    {
        /// <summary>
        /// Test data objects for testing "Begins with" suggestions (Contains = False.)
        /// </summary>
        public static IEnumerable<object[]> ResponseData => new[]
        {
            new object[] { new AutosuggestScenario_BeginAciSpanishPatient() },
            new object[] { new AutosuggestScenario_BeginDarEnglishPatient() },
            new object[] { new AutosuggestScenario_ContainCatEnglishPatient()},
            new object[] { new AutosuggestScenario_ContainCutaneoSpanishPatient() }
        };

        /// <summary>
        /// Test that the request to Elasticsearch goes to the correct URI
        /// and structures the request body as expected.
        /// </summary>
        [Theory, MemberData(nameof(ResponseData))]
        public async Task GetSuggestions_TestBeginsRequestSetup(BaseAutosuggestTestData data)
        {
            Uri esURI = null;
            HttpMethod esMethod = HttpMethod.DELETE; // Initialize to something other than the expected value.

            string requestBody = null;

            string responseBody = TestingTools.ReadTestFile("ESAutosuggestQueryResponse/" + data.TestFilename);

            var connectionSettings = TestingElasticsearchClientSettingsFactory.Create(
                responseBody,
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

            // We don't really care about the inputs. What matters is the return, which is controlled by the mock ES connection.
            Suggestion[] result = await query.GetSuggestions("Cancer.gov", AudienceType.Patient, "es", "chicken", MatchType.Contains, 200);

            Assert.Equal(data.ExpectedData, result, new ArrayComparer<Suggestion, SuggestionComparer>());
            Assert.Equal("/glossaryv1/_search", esURI.AbsolutePath);
            Assert.Equal(HttpMethod.POST, esMethod);
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