using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks; 

namespace Services.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        DBLayer.DBLayer dblayer = new DBLayer.DBLayer();

        //User Login Validation Controller
        // POST api/<LoginController>
        [HttpPost, Route("PostLogin")]
        public JsonResult ValidateUser(LoginRequestModels LoginReq)
        {
            var data = dblayer.User_Validation(LoginReq);
            return new JsonResult(data);
        }
    }
}
