using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Dictionary - Cancer.gov
    /// Contains - True
    /// SearchText - ablación
    /// Language - Spanish
    /// Audience - Patient
    /// </summary>
    class Autosuggest_Request_CGov_Contains_Ablacion_Spanish_Patient : BaseAutosuggestRequestTestData
    {
        public   override   string SearchText => "ablación" ;

        public override  MatchType   MatchType => MatchType.Contains;

        public override string DictionaryName => "Cancer.gov";

        public override string Language => "es";

        public override AudienceType Audience => AudienceType.Patient;

        public override int Size => 5;

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
                            },
                            {
                                ""match_phrase"": {
                                    ""term_name._autocomplete"": {
                                        ""query"": ""ablación""
                                    }
                                }
                            }
                        ],
                        ""must_not"": {
                            ""prefix"": {
                                ""term_name"": {
                                    ""value"": ""ablación""
                                }
                            }
                        }
                    }
                },
                ""sort"": { ""term_name"": {} },
                ""_source"": {
                    ""includes"": [
                        ""term_id"",
                        ""term_name""
                    ]
                },
                ""size"": 5
            }
        ");
    }
}