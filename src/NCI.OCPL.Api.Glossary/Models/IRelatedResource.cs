namespace NCI.OCPL.Api.Glossary
{
    
    /// <summary>
    /// RelatedResources Interface
    /// </summary>
    public interface IRelatedResource
    {    
        /// <summary>
        /// Gets the Type of the related resource
        /// </summary>    
        RelatedResourceType Type {get ;}

        /// <summary>
        /// Gets/Sets the text for the resource
        /// </summary>    
        string Text {get; set; }
    }
}