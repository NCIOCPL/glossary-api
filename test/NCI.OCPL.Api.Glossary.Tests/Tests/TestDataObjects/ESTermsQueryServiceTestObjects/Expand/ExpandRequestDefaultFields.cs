using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Test data for TermsQueryService Expand method with only default fields in the response.
    /// </summary>
    public class ExpandRequestDefaultFields : ExpandRequestBase
    {
        /// <inheritdoc />
        public override string Dictionary => "Cancer.gov";

        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.Patient;

        /// <inheritdoc />
        public override string LanguageCode => "en";

        /// <inheritdoc />
        public override string ExpandCharacter => "s";

        /// <inheritdoc />
        public override int Size => 5;

        /// <inheritdoc />
        public override int From => 0;

        /// <inheritdoc />
        public override bool IncludeAdditionalInfo => false;

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
                            ""other_languages""
                        ]
                    },
                    ""sort"": { ""term_name"": {} },
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
                                            ""value"": ""Cancer.gov""
                                        }
                                    }
                                },
                                {
                                    ""term"": {
                                        ""first_letter"": {
                                            ""value"": ""s""
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
