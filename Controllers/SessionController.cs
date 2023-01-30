using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CMDImageProxy.Controllers.CMDSession;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CMDImageProxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        // GET: api/<SessionController>
        [HttpGet]
        public string Get()
        {
            Task<String> _id = CMDSession.getCMDSesionId();
            root obj = JsonConvert.DeserializeObject<root>(_id.Result);
            //HttpContext.Session.SetString("cmdid", obj.data._id);
            //HttpContext.Session.SetString("lasttokentime", DateTime.Now.ToString());
            return obj.data._id;
        }

        // GET api/<SessionController>/5
        [HttpGet("{id}")]
        public IEnumerable<string> Get(string  id)
        {
            return new string[] { "value1", "value2" };
            // product = await GetProductAsync(url.PathAndQuery);
        }

    }
}
