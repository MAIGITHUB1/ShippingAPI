using Microsoft.AspNetCore.Mvc;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Services.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/<HomeController>
        [HttpGet, Route("GetKey")]
        public string Get()
        {
            return AuthenticationConfig.GenerateJSONWebToken();
        }
    }
}
