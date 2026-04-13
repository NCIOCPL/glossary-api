using System.Text.Json.Nodes;

namespace NCI.OCPL.Api.Glossary.Tests
{
    /// <summary>
    /// Test data for TermsQueryService GetAll method when additional fields should be returned.
    /// </summary>
    public class GetAll_Request_AdditionalFields : GetAll_Request_Base
    {
        /// <inheritdoc />
        public override string Dictionary => "Genetics";

        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.HealthProfessional;

        /// <inheritdoc />
        public override string LangCode => "en";

        /// <inheritdoc />
        public override int Size => 5;

        /// <inheritdoc />
        public override int From => 200;

        /// <inheritdoc />
        public override bool IncludeAdditionalInfo => true;

        /// <inheritdoc />
        public override JsonNode ExpectedRequest => JsonNode.Parse(@"
                {
                    ""from"": 200,
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
                                            ""value"": ""Genetics""
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
