
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.Glossary.Tests
{
    class Terms_Search_Request_Exact : Terms_Search_Request_Base
    {
        public override string Dictionary => "Cancer.gov";

        public override AudienceType Audience => AudienceType.Patient;

        public override string LangCode => "en";

        public override string SearchTerm => "s-1";

        public override MatchType MatchType => MatchType.Exact;

        public override int Size => 5;

        public override int From => 0;

        public override bool IncludeAdditionalInfo => false;

        public override JObject ExpectedRequest => JObject.Parse(@"
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
                    ""sort"": [
                        {
                            ""term_name"": {}
                        }
                    ],
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
                                        ""term_name"": {
                                            ""value"": ""s-1""
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