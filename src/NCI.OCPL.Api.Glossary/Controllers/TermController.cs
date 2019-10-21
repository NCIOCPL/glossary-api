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
    [Route("api/v1")]
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
    }

}