using Microsoft.AspNetCore.Mvc;
using System;

using NCI.OCPL.Api.Glossary.Interfaces;

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
    }
}
