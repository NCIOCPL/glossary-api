using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using NCI.OCPL.Api.Glossary;
using NCI.OCPL.Api.Common;
using System.Collections.Generic;

namespace NCI.OCPL.Api.Glossary.Controllers
{

    /// <summary>
    /// Controller for routes used when searching for or retrieving
    /// multiple Terms.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class TermsController : Controller
    {
        private readonly ITermsQueryService _termsQueryService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TermsController(ITermsQueryService service)
        {
            _termsQueryService = service;
        }

        /// <summary>
        /// Retrieves a portion of the overall set of glossary terms for a given combination of dictionary, audience, and language.
        /// </summary>
        /// <param name="dictionary">The specific dictionary to retrieve from.</param>
        /// <param name="audience">The target audience.</param>
        /// <param name="language">Language (English - en; Spanish - es).</param>
        /// <param name="size">The number of records to retrieve.</param>
        /// <param name="from">The offset into the overall set to use for the first record.</param>
        /// <param name="requestedFields">The fields to retrieve.  If not specified, defaults to TermName, Pronunciation, and Definition.</param>
        /// <returns>A GlossaryTermResults object containing the desired records.</returns>
        [HttpGet("/glossary/v1/terms/{dictionary}/{audience}/{language}")]
        public async Task<GlossaryTermResults> getAll(string dictionary, string audience, string language, int size = 10, int from = 0, string[] requestedFields = null)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language))
                throw new APIErrorException(400, "You must specify a dictionary, audience, and language.");

            if (language.ToLower() != "en" && language.ToLower() != "es")
                throw new APIErrorException(404, "Unsupported Language. Please try either 'en' or 'es'");

            AudienceType audienceType;
            if(!Enum.TryParse(audience,true,out audienceType))
                    throw new APIErrorException(400, "'AudienceType' can  be 'Patient' or 'HealthProfessional' only");

            if (size <= 0)
                size = 20;

            if (from < 0)
                from = 0;

            if( requestedFields == null || requestedFields.Length == 0 )
                requestedFields = new string[] {"TermName", "Pronunciation", "Definition" };

            GlossaryTermResults res = await _termsQueryService.getAll(dictionary, audienceType, language, size, from, requestedFields);

            return res;
        }

        /// <summary>
        /// Search for Terms based on search criteria
        /// </summary>
        /// <returns>An array GlossaryTerm objects</returns>   
        [HttpGet("search/{dictionary}/{audience}/{language}/{query}")]     
        public async Task<GlossaryTerm[]> Search(string dictionary, string audience, string language, string query,
            [FromQuery] string matchType, [FromQuery] int size, [FromQuery] int from, [FromQuery] string[] requestedFields)
        {
            if (String.IsNullOrWhiteSpace(dictionary) || String.IsNullOrWhiteSpace(language) || String.IsNullOrWhiteSpace(audience))
            {
                throw new APIErrorException(400, "You must supply a valid dictionary, audience and language");
            }
            AudienceType audienceType;
            if(!Enum.TryParse(audience,true,out audienceType)){
                    throw new APIErrorException(400, "'AudienceType' can  be 'Patient' or 'HealthProfessional' only");
            }
            if(null == matchType || ! ( matchType.Equals("begins",StringComparison.InvariantCultureIgnoreCase) || matchType.Equals("contains" ,StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new APIErrorException(400, "'matchType' can only be 'begins' or 'contains'");
            }
            if(size <=0)
            {
                size = 100;
            }
            if(from < 0 )
            {
                from = 0;
            }     
            // if requestedFields is empty populate it with default values 
            if(null == requestedFields || requestedFields.Length == 0)
            {
                requestedFields =  new string[]{"TermName","Pronunciation","Definition"};
            }

            List<GlossaryTerm> glossaryTermList = await _termsQueryService.Search(dictionary, audienceType, language, query, matchType, size, from, requestedFields);
            return glossaryTermList.ToArray();
        }            
    }
}
