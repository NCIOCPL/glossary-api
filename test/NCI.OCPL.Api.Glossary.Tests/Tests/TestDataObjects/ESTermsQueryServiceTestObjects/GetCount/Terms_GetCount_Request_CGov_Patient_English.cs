using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for ESTermsQueryService GetCount method for Cancer.gov, Patient, English terms.
    /// </summary>
    public class Terms_GetCount_Request_CGov_Patient_English : BaseTermsQueryCountTestData
    {
        /// <inheritdoc />
        public override string DictionaryName => "Cancer.Gov";

        /// <inheritdoc />
        public override string Language => "en";

        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.Patient;

        /// <inheritdoc />
        public override JsonNode ExpectedData => JsonNode.Parse(@"
{
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
