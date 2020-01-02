using Nest;
namespace NCI.OCPL.Api.Glossary
{
    /// <summary>
    /// Class representing the details required for Pronounciation
    /// </summary>    
    public class Pronounciation
    {
        /// <summary>
        /// Gets or sets the Key for Pronounciation
        /// </summary>
        [Keyword(Name = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value for Audio for Pronounciation
        /// </summary>
        [Keyword(Name = "audio")]
        public string Audio { get; set; }
        /// TODO Convert string to URL class

        /// <summary>
        /// No Arg Constructor
        /// </summary>
        public Pronounciation() { }

        /// <summary>
        /// 2 Arg Constructor
        /// </summary>
        public Pronounciation(string Key, string Audio) {
            this.Key = Key;
            this.Audio = Audio;
        }
    }
}