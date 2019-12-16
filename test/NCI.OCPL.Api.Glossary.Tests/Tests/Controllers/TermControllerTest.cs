using System;
using System.IO;
using System.Collections.Generic;
using Moq;
using Xunit;
using NCI.OCPL.Api.Glossary.Controllers;
using Newtonsoft.Json;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Common.Testing;
using Nest;
using NCI.OCPL.Api.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCI.OCPL.Api.Glossary.Models;

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
        public async void GetById_ErrorMessage_Dictionary(){
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            TermController controller = new TermController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("", AudienceType.Patient, "EN", 10L, new string[]{}));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Languate(){
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            TermController controller = new TermController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "", 10L, new string[]{}));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById_ErrorMessage_Id(){
            Mock<ITermQueryService> termQueryService = new Mock<ITermQueryService>();
            TermController controller = new TermController(termQueryService.Object);
            var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.GetById("Dictionary", AudienceType.Patient, "EN", 0L, new string[]{}));
            Assert.Equal("You must supply a valid dictionary, audience, language and id", exception.Message);
        }

        [Fact]
        public async void GetById()
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
                RelatedResources = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary},
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
            .Returns(Task.FromResult(glossaryTerm));

            TermController controller = new TermController(termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));

            // Verify that the service layer is called:
            // a) with the expected values.
            // b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public async void GetById_BlankRequiredFields()
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
                RelatedResources = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary},
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
            .Returns(Task.FromResult(glossaryTerm));

            TermController controller = new TermController(termQueryService.Object);
            GlossaryTerm gsTerm = await controller.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, requestedFields);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));

            // Verify that the service layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            termQueryService.Verify(
                svc => svc.GetById("Dictionary", AudienceType.Patient, "EN", 1234L, new string[] {"TermName","Pronunciation","Definition"}),
                Times.Once
            );

            Assert.Equal(expectedJsonValue, actualJsonValue);
        }                                                   
    }
}