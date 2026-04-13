namespace NCI.OCPL.Api.Glossary.Tests.ESTermsQueryTestData
{
    /// <summary>
    /// Base class for test objects to use when testing  ESTermsQueryService::GetById.
    /// </summary>
    public abstract class BaseTermsQueryTestData
    {
        /// <summary>
        /// The item's Term ID as it would appear in Elasticsearch.
        /// </summary>
        /// <returns></returns>
        public abstract string ESTermID { get; }

        /// <summary>
        /// The GlossaryTerm expected to be returned from the GetById call.
        /// </summary>
        /// <returns></returns>
        public abstract GlossaryTerm ExpectedData { get; }

        /// <summary>
        /// The ID for the term being requested.
        /// </summary>
        /// <value></value>
        public abstract long TermID { get; }

        /// <summary>
        /// The name of the dictionary for the term request
        /// </summary>
        /// <value></value>
        public abstract string DictionaryName { get; }

        /// <summary>
        /// The language of the term request
        /// </summary>
        /// <value></value>
        public abstract string Language { get; }

        /// <summary>
        /// The audience type for the term request
        /// </summary>
        /// <value></value>
        public abstract AudienceType Audience { get; }
    }
}