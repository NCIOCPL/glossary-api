namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Media Interface
    /// </summary>
    public interface IMedia
    {
        /// <summary>
        /// type of media to be used
        /// </summary>
        MediaType Type { get; }
    }
}