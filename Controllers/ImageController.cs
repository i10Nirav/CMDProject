using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using static CMDImageProxy.Controllers.CMDSession;
namespace CMDImageProxy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        CMDSession da = new CMDSession();
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

    // GET api/values/5
        [Route("[action]/{classNameParam}/{cardIdParam}/{shapeIdParam}")]
        [HttpGet]
        public ActionResult<string> GetImageList(string classNameParam,string cardIdParam, string shapeIdParam)
        {
          try
          {
              Task<String> _id = da.getMainImage(classNameParam, cardIdParam, shapeIdParam);
              return _id.Result;

          }
          catch (Exception ex)
          {
            return ex.Message;
          }
         }

    // GET api/values/5
        [Route("[action]/{classNameParam}/{cardIdParam}/{shapeIdParam}/{imageId}")]
        [HttpGet]
        public ActionResult<HttpResponseMessage> GetImageContent(string classNameParam, string cardIdParam, string shapeIdParam,string imageId)
        {
            try
            {
                Task<HttpResponseMessage> _id = da.getImageContent(classNameParam, cardIdParam, shapeIdParam, imageId);
                HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

                  return _id.Result;
            }
            catch(Exception ex)
            {
                HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                 
                 return Response;
            }
          
        }
  }

}
