using System;

using Moq;
using Xunit;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Glossary.Controllers;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.Glossary.Tests
{
    public class TermsControllerTests
    {
        [Fact]
        public async void GetAll_Error_DictionaryMissing()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();

            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => controller.getAll("", AudienceType.HealthProfessional, "en"));
        }

        [Fact]
        public async void GetAll_Error_LanguageMissing()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();

            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => controller.getAll("glossary", AudienceType.HealthProfessional, ""));
        }

        [Fact]
        public async void GetAll_Error_LanguageBad()
        {
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();

            TermsController controller = new TermsController(querySvc.Object);

            APIErrorException ex = await Assert.ThrowsAsync<APIErrorException>(() => controller.getAll("glossary", AudienceType.HealthProfessional, "turducken"));
        }

        /// <Summary>
        /// Verify that getAll behaves in the expected manner when size, from, and requestedFields
        /// aren't set in the call.
        /// </Summary>
        [Fact]
        public async void GetAll_Default_Parameters()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.getAll(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.getAll("glossary", AudienceType.HealthProfessional, "en");

            // Verify that the query layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.getAll("glossary", AudienceType.HealthProfessional, "en", 10, 0, new string[] { "TermName", "Pronunciation", "Definition" }),
                Times.Once,
                "ITermsQueryService::getAll() should be called once, with default values for size, from, and requestedFields"
            );
        }

        /// <Summary>
        /// Verify that getAll behaves in the expected manner when size, from, and requestedFields
        /// are set to explicit values
        /// </Summary>
        [Fact]
        public async void GetAll_Specified_Parameters()
        {
            // Create a mock query that always returns the same result.
            Mock<ITermsQueryService> querySvc = new Mock<ITermsQueryService>();
            querySvc.Setup(
                svc => svc.getAll(
                    It.IsAny<string>(),
                    It.IsAny<AudienceType>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.FromResult(new GlossaryTermResults()));

            // Call the controller, we don't care about the actual return value.
            TermsController controller = new TermsController(querySvc.Object);
            await controller.getAll("glossary", AudienceType.HealthProfessional, "es", 200, 2, new string[] {"Field1", "Field2", "Field3"});

            // Verify that the query layer is called:
            //  a) with the expected values.
            //  b) exactly once.
            querySvc.Verify(
                svc => svc.getAll("glossary", AudienceType.HealthProfessional, "es", 200, 2, new string[] { "Field1", "Field2", "Field3" }),
                Times.Once,
                "ITermsQueryService::getAll() should be called once, with the specified values for size, from, and requestedFields"
            );
        }

    }
}