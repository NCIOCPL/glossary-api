using System;
using NCI.OCPL.Api.Glossary;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for TermsQueryService GetByName method with a specific term.
    /// </summary>
    public class GetByName_s_1 : GetByNameTermsQueryTestData
    {
        /// <inheritdoc />
        public override GlossaryTerm ExpectedData => new GlossaryTerm() {
            TermId = 46716,
            Language = "en",
            Dictionary = "Cancer.gov",
            Audience = AudienceType.Patient,
            TermName = "S-1",
            FirstLetter = "s",
            PrettyUrlName = "s-1",
            Definition = new Definition()
            {
                Text = "A drug that is being studied for its ability to enhance the effectiveness of fluorouracil and prevent gastrointestinal side effects caused by fluorouracil. It belongs to the family of drugs called antimetabolites.",
                Html = "A drug that is being studied for its ability to enhance the effectiveness of fluorouracil and prevent gastrointestinal side effects caused by fluorouracil. It belongs to the family of drugs called antimetabolites."
            },
            Pronunciation = null,
            Media = new IMedia[] {},
            RelatedResources = new IRelatedResource[] { }
        };

        /// <inheritdoc />
        public override string PrettyUrlName => "s-1";
    }
}
