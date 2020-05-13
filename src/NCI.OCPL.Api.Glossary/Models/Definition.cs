using Nest;

namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// AudienceType enum
    /// </summary>    
    public class Definition
    {
        /// <summary>
        /// Gets or sets the html value for the definition
        /// </summary>
        [Keyword(Name = "html")]
        public string Html { get; set; }

        /// <summary>
        /// Gets or sets the text for the definition
        /// </summary>
        [Keyword(Name = "text")]
        public string Text { get; set; }
        /// TODO Convert string to URL class

        /// <summary>
        /// No Arg Constructor
        /// </summary>
        public Definition() { }

        /// <summary>
        /// 2 Arg Constructor
        /// </summary>
        public Definition(string Html, string Text) {
            this.Html = Html;
            this.Text = Text;
         }


    }
}