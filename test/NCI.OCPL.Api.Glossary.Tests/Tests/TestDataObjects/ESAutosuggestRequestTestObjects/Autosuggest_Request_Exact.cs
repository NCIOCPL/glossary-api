using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Dictionary - Genetics
    /// Contains - False
    /// SearchText - dar
    /// Language - English
    /// Audience - HealthProfessional
    /// </summary>
    public class Autosuggest_Request_Exact : BaseAutosuggestRequestTestData
    {
        /// <inheritdoc />
        public override string SearchText => "Are you kidding?";

        /// <inheritdoc />
        public override MatchType MatchType => MatchType.Exact;

        /// <inheritdoc />
        public override string DictionaryName => "Cancer.gov";

        /// <inheritdoc />
        public override string Language => "en";

        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.HealthProfessional;

        /// <inheritdoc />
        public override int Size => 1;

        /// <inheritdoc />
        public override JsonNode ExpectedData => JsonNode.Parse(@"
            {
                ""query"": {
                    ""bool"": {
                        ""must"": [{
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
                                        ""value"": ""Cancer.gov""
                                    }
                                }
                            },
                            {
                                ""term"": {
                                    ""term_name"": {
                                        ""value"": ""Are you kidding?""
                                    }
                                }
                            }
                        ]
                    }
                },
                ""size"": 1,
                ""_source"": {
                    ""includes"": [
                        ""term_id"",
                        ""term_name""
                    ]
                },
                ""sort"": { ""term_name"": {} }
            }
        ");
    }
}