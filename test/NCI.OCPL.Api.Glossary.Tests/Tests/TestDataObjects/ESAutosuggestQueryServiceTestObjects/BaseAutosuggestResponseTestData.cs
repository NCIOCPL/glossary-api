using Xunit.Abstractions;

namespace NCI.OCPL.Api.Glossary.Tests.ESAutosuggestQueryTestData
{
    /// <summary>
    /// Base class for test data objects in mock Elasticsearch responses.
    /// </summary>
    public abstract class BaseAutosuggestTestData : IXunitSerializable
    {
        /// <summary>
        /// Contains the name of the JSON data file to use in
        /// </summary>
        public abstract string TestFilename { get; }

        /// <summary>
        /// Gets the expected response data object.
        /// </summary>
        public abstract Suggestion[] ExpectedData { get; }

        // All data is baked into hardcoded property return values on each concrete subclass,
        // so there is no instance state to serialize or deserialize.
        public void Deserialize(IXunitSerializationInfo info) { }
        public void Serialize(IXunitSerializationInfo info) { }

    }
}