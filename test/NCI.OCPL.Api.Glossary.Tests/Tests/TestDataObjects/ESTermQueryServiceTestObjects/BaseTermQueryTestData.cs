using System;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests.ESTermQueryTestData
{
    public abstract class BaseTermQueryTestData
    {
        /// <summary>
        /// Gets the file name containing the actual test data.
        /// </summary>
        /// <returns></returns>
        public abstract string TestFilePath { get; }

        /// <summary>
        /// Gets an instance of the Expected Data object
        /// </summary>
        /// <returns></returns>
        public abstract GlossaryTerm ExpectedData { get; }
    }
}