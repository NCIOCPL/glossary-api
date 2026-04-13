
using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Test data for calling ESTermsQueryService::Search with MatchType.Contains.
    /// </summary>
    class Terms_Search_Request_Contains : Terms_Search_Request_Base
    {
        /// <inheritdoc />
        public override string Dictionary => "Cancer.gov";

        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.Patient;

        /// <inheritdoc />
        public override string LangCode => "es";

        /// <inheritdoc />
        public override string SearchTerm => "pollo";

        /// <inheritdoc />
        public override MatchType MatchType => MatchType.Contains;

        /// <inheritdoc />
        public override int Size => 5;

        /// <inheritdoc />
        public override int From => 0;

        /// <inheritdoc />
        public override bool IncludeAdditionalInfo => true;

        /// <inheritdoc />
        public override JsonNode ExpectedRequest => JsonNode.Parse(@"
                {
                    ""from"": 0,
                    ""size"": 5,
                    ""_source"": {
                        ""includes"": [
                            ""term_id"",
                            ""language"",
                            ""dictionary"",
                            ""audience"",
                            ""term_name"",
                            ""first_letter"",
                            ""pretty_url_name"",
                            ""pronunciation"",
                            ""definition"",
                            ""other_languages"",
                            ""related_resources"",
                            ""media""
                        ]
                    },
                    ""sort"": { ""term_name"": {} },
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
                                    ""match"": {
                                        ""term_name._contain"": {
                                            ""query"": ""pollo""
                                        }
                                    }
                                }
                            ]
                        }
                    }
                }"
            );
    }
}
