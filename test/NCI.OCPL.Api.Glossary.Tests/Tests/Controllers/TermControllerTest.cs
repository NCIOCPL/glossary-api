using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using NCI.OCPL.Api.Glossary.Controllers;
using Newtonsoft.Json;
using NCI.OCPL.Api.Glossary.Interfaces;
using NCI.OCPL.Api.Glossary.Services;
using NCI.OCPL.Api.Common.Testing;
using Nest;

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
        public void GetById()
        {
            IElasticClient elasticClient = new ElasticClient();
            ITermQueryService termQueryService = new TermQueryService(elasticClient);
            TermController controller = new TermController(termQueryService);
            GlossaryTerm gsTerm = controller.GetById("Dictionary", AudienceType.Patient, "EN", 10L);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText(TestingTools.GetPathToTestFile("TestData.json"));
            Assert.Equal(expectedJsonValue, actualJsonValue);
        }
    }
}