using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    public class Terms_GetCount_Request_CGov_HealthProfessional_English : BaseTermsQueryCountTestData
    {
        public override string DictionaryName => "Cancer.Gov";

        public override string Language => "en";

        public override AudienceType Audience => AudienceType.HealthProfessional;

        public override JObject ExpectedData => JObject.Parse(@"
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