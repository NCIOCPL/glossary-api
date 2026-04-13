using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for TermsQueryService GetAll method when there are no results to return.
    /// </summary>
    public class GetAll_NoResults : GetAllTermsQueryTestData
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
        public override string GetAllTestType => "noresults";
    }
}
