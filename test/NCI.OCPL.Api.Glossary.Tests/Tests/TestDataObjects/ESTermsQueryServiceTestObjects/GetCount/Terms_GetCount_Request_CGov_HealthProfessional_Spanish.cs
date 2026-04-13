using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for ESTermsQueryService GetCount method for Cancer.gov, Health Professional, Spanish terms.
    /// </summary>
    public class Terms_GetCount_Request_CGov_HealthProfessional_Spanish : BaseTermsQueryCountTestData
    {
        /// <inheritdoc />
        public override string DictionaryName => "Cancer.Gov";

        /// <inheritdoc />
        public override string Language => "es";

        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.HealthProfessional;

        /// <inheritdoc />
        public override JsonNode ExpectedData => JsonNode.Parse(@"
{
    ""query"": {
        ""bool"": {
            ""must"": [
                {
                    ""term"": {
                        ""language"": {
                            ""value"": ""es""
                        }
                    }
                },
                {
                    ""term"": {
                        ""audience"": {
                            ""value"": ""HealthProfessional""
                        }
                    }
                },
                {
                    ""term"": {
                        ""dictionary"": {
                            ""value"": ""Cancer.Gov""
                        }
                    }
                }
            ]
        }
    }
}
        ");

    }
}
