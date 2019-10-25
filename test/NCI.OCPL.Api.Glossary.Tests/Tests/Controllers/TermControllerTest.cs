using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using NCI.OCPL.Api.Glossary.Controllers;
using Newtonsoft.Json;


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
            TermController controller = new TermController();
            GlossaryTerm gsTerm = controller.GetById("Dictionary", AudienceType.Patient, "EN", 10L);
            string actualJsonValue = JsonConvert.SerializeObject(gsTerm);
            string expectedJsonValue = File.ReadAllText("Tests\\TestData\\TestData.json");
            Assert.Equal(expectedJsonValue, actualJsonValue);
        }

        [Fact]
        public void Say_Hello_World()
        {
            TermController controller = new TermController();
            string actualValue = controller.SayHelloWorld();
            string expectedValue = "Hello New World";
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void Say_Hello_World_Error()
        {
            TermController controller = new TermController();
            string actualValue = controller.SayHelloWorld();
            string expectedValue = "Hello";
            Assert.NotSame(expectedValue, actualValue);
        }

        private void GetTestData()
        {
            string jsonstr = File.ReadAllText("C:\\Projects\\NCI\\nciocpl\\glossary-api\\test\\NCI.OCPL.Api.Glossary.Tests\\Tests\\TestData\\TestData.json"); 
            var ser = JsonConvert.DeserializeObject<GlossaryTerm>(jsonstr);
        }
    }
}