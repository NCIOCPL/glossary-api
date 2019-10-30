namespace NCI.OCPL.Api.Glossary.Interfaces
{
    /// <summary>
    /// Interface for a Term Query service.
    /// </summary>
    public interface ITermQueryService
    {
        /// <summary>
        /// Get Term deatils based on the input values
        /// <param name="dictionary">The value for dictionary.</param>
        /// <param name="audience">Patient or Healthcare provider</param>
        /// <param name="language">The language in which the details needs to be fetched</param>
        /// <param name="id">The Id for the term</param>
        /// <returns>An object of GlossaryTerm</returns>        
        /// </summary>
        GlossaryTerm GetById(string dictionary, AudienceType audience, string language, long id);
    }
}