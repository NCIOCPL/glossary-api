using System;
using System.IO;
using Moq;
using Xunit;
using NCI.OCPL.Api.Glossary.Controllers;
using Newtonsoft.Json;
using NCI.OCPL.Api.Glossary.Interfaces;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Common.Testing;
using Nest;
using NCI.OCPL.Api.Common;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public class TermControllerTests
    {
        [JsonProperty("GlossaryTerm")]
        private GlossaryTerm glossaryTerm;

        private GlossaryTerm GetGlossaryTerm()
        {
            return glossaryTerm;
        }

        private void SetGlossaryTerm(GlossaryTerm value)
        {
            glossaryTerm = value;
        }

        [Fact]
        public void GetById_ErrorMessage_Dictionary(){
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            TermController controller = new TermController(termQueryService.Object);
            var exception = Assert.Throws<APIErrorException>(() => controller.GetById("", AudienceType.Patient, "EN", 10L, new string[]{}));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public void GetById_ErrorMessage_Languate(){
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            TermController controller = new TermController(termQueryService.Object);
            var exception = Assert.Throws<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "", 10L, new string[]{}));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        } 

        [Fact]
        public void GetById_ErrorMessage_Id(){
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            TermController controller = new TermController(termQueryService.Object);
            var exception = Assert.Throws<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "EN", 0L, new string[]{}));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }       

        [Fact]
        public void GetById()
        {
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            string[] requestedFields = {"TermName","Pronunciation","Definition"};
            Pronounciation pronounciation = new Pronounciation("Pronounciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            GlossaryTerm glossaryTerm = new GlossaryTerm
            {
                Id = 1234L,
                Language = "EN",
                Dictionary = "Dictionary",
                Audience = AudienceType.Patient,
                TermName = "TermName",
                PrettyUrlName = "www.glossary-api.com",
                Pronounciation = pronounciation,
                Definition = definition,
                RelatedResourceType = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary},
            };
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.IsAny<String>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(glossaryTerm);

            TermController controller = new TermController(termQueryService.Object);
            GlossaryTerm gsTerm = controller.GetById("Dictionary", AudienceType.Patient, "EN", 10L, requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 10L, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }   

        [Fact]
        public void GetById_BlankRequiredFields()
        {
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            string[] requestedFields = new string[]{};
            Pronounciation pronounciation = new Pronounciation("Pronounciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            GlossaryTerm glossaryTerm = new GlossaryTerm
            {
                Id = 1234L,
                Language = "EN",
                Dictionary = "Dictionary",
                Audience = AudienceType.Patient,
                TermName = "TermName",
                PrettyUrlName = "www.glossary-api.com",
                Pronounciation = pronounciation,
                Definition = definition,
                RelatedResourceType = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary},
            };
            termQueryService.Setup(
                termQSvc => termQSvc.GetById(
                    It.IsAny<String>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<long>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(glossaryTerm);

            TermController controller = new TermController(termQueryService.Object);
            GlossaryTerm gsTerm = controller.GetById("Dictionary", AudienceType.Patient, "EN", 10L, requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 10L, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }                          
    }
}