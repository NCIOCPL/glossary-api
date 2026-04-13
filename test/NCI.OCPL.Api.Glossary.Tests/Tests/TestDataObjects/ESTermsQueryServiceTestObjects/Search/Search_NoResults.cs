using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for EsTermsQueryService::Search when no results are returned from ES.
    /// </summary>
    public class Search_NoResults : SearchTermsQueryTestData
    {
        /// <inheritdoc />
        public override GlossaryTermResults ExpectedData => new GlossaryTermResults() {
            Results = new GlossaryTerm[] {},
            Meta = new ResultsMetadata() {
                TotalResults = 0,
                From = 0
            }
        };

        /// <inheritdoc />
        public override string SearchTestType => "noresults";
    }
}
