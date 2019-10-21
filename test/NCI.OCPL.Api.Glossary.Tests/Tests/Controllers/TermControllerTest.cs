using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using NCI.OCPL.Api.Glossary.Controllers;


namespace NCI.OCPL.Api.Glossary.Tests
{
    public class TermControllerTests
    {
        [Fact]
        public void Say_Hello_World()
        {
            TermController controller = new TermController();
            string actualValue = controller.SayHelloWorld();
            string expectedValue = "Hello New World";
            Assert.Equal(expectedValue,actualValue);
        }

        [Fact]
        public void Say_Hello_World_Error()
        {
            TermController controller = new TermController();
            string actualValue = controller.SayHelloWorld();
            string expectedValue = "Hello";
            Assert.NotSame(expectedValue,actualValue);
        }        
    }
}