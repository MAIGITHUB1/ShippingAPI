using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.Models;

namespace Services.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        //DB Connection
        DBLayer.DBLayer dblayer = new DBLayer.DBLayer();

        //Client Dropdown Controller
        // GET: api/<ClientsController>
        [HttpGet, Route("GetClients")]
        public JsonResult GetClientsList()
        {
            var cdata = dblayer.Get_Client_List();
            return new JsonResult(cdata);
        }
        
        //Shipping Report Controller
        [HttpPost, Route("ShippingList")]
        public JsonResult ShippingList(ShippingRequestModels ShipReq)
        {
            var sdata = dblayer.Get_Ship_List(ShipReq);
            return new JsonResult(sdata);
        }
    }
}
