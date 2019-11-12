using Microsoft.AspNetCore.Mvc;
using System;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Glossary.Interfaces;

namespace NCI.OCPL.Api.Glossary.Controllers
{
    /// <summary>
    /// The Term Enpoint Controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TermController : Controller
    {
        private readonly ITermQueryService _termQueryService;

        /// <summary>
        /// Creates a new instance of a TermController with one argument.
        /// </summary>
        public TermController(ITermQueryService termQueryService){
            this._termQueryService = termQueryService;
        }

        /// <summary>
        /// Get the Glossary Term based on Id.
        /// </summary>
        /// <returns>GlossaryTerm object</returns>
        [HttpGet("{dictionary}/{audience}/{language}/{id}")]
        public GlossaryTerm GetById(string dictionary, AudienceType audience, string language, long id, [FromQuery] string[] requestedFields){

             if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || id <= 0){
                throw new APIErrorException(400, "You must supply a valid dictionary, audience, language and id");
             }

             return _termQueryService.GetById(dictionary,audience,language,id, requestedFields);
        }
    }

}