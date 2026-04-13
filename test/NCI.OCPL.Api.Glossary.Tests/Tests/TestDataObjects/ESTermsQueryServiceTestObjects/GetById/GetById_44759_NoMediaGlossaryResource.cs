using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Test data for TermsQueryService GetById method with no media items and Glossary related resource.
    /// </summary>
    public class GetById_44759_NoMediaGlossaryResource : BaseTermsQueryTestData
    {
        /// <inheritdoc />
        public override string DictionaryName => "cancer.gov";
        /// <inheritdoc />
        public override AudienceType Audience => AudienceType.Patient;
        /// <inheritdoc />
        public override long TermID => 44759L;

        /// <inheritdoc />
        public override string ESTermID => "44759_cancer.gov_en_patient";
        /// <inheritdoc />
        public override string Language => "en";

        /// <inheritdoc />
        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
            TermId = 44759L,
            Language = "en",
            Dictionary = "Cancer.gov",
            Audience = AudienceType.Patient,
            TermName = "fatty breast tissue",
            FirstLetter = "f",
            PrettyUrlName = "fatty-breast-tissue",
            Definition = new Definition()
            {
                Text = "A term used to describe breast tissue that is made up of almost all fatty tissue. Fatty breast tissue does not look dense on a mammogram, which may make it easier to find tumors or other changes in the breast. Fatty breast tissue is more common in older women than in younger women. Fatty breast tissue is one of four categories used to describe a level of breast density seen on a mammogram.",
                Html = "A term used to describe breast tissue that is made up of almost all fatty tissue. Fatty breast tissue does not look dense on a mammogram, which may make it easier to find tumors or other changes in the breast. Fatty breast tissue is more common in older women than in younger women. Fatty breast tissue is one of four categories used to describe a level of breast density seen on a mammogram."
            },
            Pronunciation = new Pronunciation()
            {
                Key = "(FA-tee brest TIH-shoo)",
                Audio = "https://nci-media-dev.cancer.gov/audio/pdq/705927.mp3"
            },
            RelatedResources = new IRelatedResource[] {
                new GlossaryResource {
                    Type = RelatedResourceType.GlossaryTerm,
                    TermId = 335487L,
                    Text = "breast density",
                    Audience = AudienceType.Patient,
                    PrettyUrlName = "breast-density"
                }
            }
        };
    }
}
