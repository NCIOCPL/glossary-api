using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NCI.OCPL.Api.Glossary.Controllers
{
    /// <summary>
    /// The Term Enpoint Controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TermController : ControllerBase
    {

        /// <summary>
        /// Creates a new instance of a TermController
        /// </summary>
        public TermController(){

        }

        /// <summary>
        /// A temporary method added to check the health of the controller.
        /// </summary>
        [HttpGet("hello")]
        public string SayHelloWorld()
        {
            return "Hello New World";
        }

        /// <summary>
        /// Get the Glossary Term based on Id.
        /// </summary>
        /// <returns>GlossaryTerm object</returns>
        [HttpGet("{dictionary}/{audience}/{language}/{id}")]
        public GlossaryTerm GetById(string dictionary, AudienceType audience, string language, long Id){

             if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || Id <= 0){
                throw new APIErrorException(400, "You must supply a valid dictionary, audience, language and id");
             }

            return GenerateSampleTerm();
        }

        /// <summary>
        /// This temporary method will create a GlossaryTerm
        /// object to testing purpose.
        /// </summary>
        /// <returns>The GlossaryTerm</returns>
        private GlossaryTerm GenerateSampleTerm(){
            GlossaryTerm _GlossaryTerm = new GlossaryTerm();
            Pronounciation pronounciation = new Pronounciation("Pronounciation Key", "pronunciation");
            Definition definition = new Definition("<html><h1>Definition</h1></html>", "Sample definition");
            _GlossaryTerm.Id = 10L;
            _GlossaryTerm.Language = "EN";
            _GlossaryTerm.Dictionary = "Dictionary";
            _GlossaryTerm.Audience = AudienceType.Patient;
            _GlossaryTerm.TermName = "TermName";
            _GlossaryTerm.PrettyUrlName = "www.glossary-api.com";
            _GlossaryTerm.Pronounciation = pronounciation;
            _GlossaryTerm.Definition = definition;
            _GlossaryTerm.RelatedResourceType = new RelatedResourceType [] {RelatedResourceType.Summary , RelatedResourceType.DrugSummary};

            return _GlossaryTerm;
        }

    }

}