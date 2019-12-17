using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermQueryTestData
{
    public class ESTermQueryServiceTestData : BaseTermQueryTestData
    {
        public override string TestFilePath => "TermQuery.json";
        public override GlossaryTerm ExpectedData => new GlossaryTerm()
        {
                Id = 0L,
                Language = "en",
                Dictionary = "cancer.gov",
                Audience = AudienceType.Patient,
                TermName = "stage II cutaneous T-cell lymphoma",
                Definition = new Definition()
                {
                    Text = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells.",
                    Html = "Stage II cutaneous T-cell lymphoma may be either of the following: (1) stage IIA, in which the skin has red, dry, scaly patches but no tumors, and lymph nodes are enlarged but do not contain cancer cells; (2) stage IIB, in which tumors are found on the skin, and lymph nodes are enlarged but do not contain cancer cells."
                },
                Pronounciation = new Pronounciation()
                {
                    Key = "(... kyoo-TAY-nee-us T-sel lim-FOH-muh)",
                    Audio = "703959.mp3"
                }
        };
    }
}