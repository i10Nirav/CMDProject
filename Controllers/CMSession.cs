using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace CMDImageProxy.Controllers
{
    public class CMDSession
    {

        private static MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
       
        public async static Task<String> getCMDSesionId()
        {
          var cacheKey = "user";
          
          if (!memoryCache.TryGetValue(cacheKey, out root user))
          {
            
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            String cmdurl = "http://amanaggarwal.com:8080/cmdbuild32/services/rest/v2/sessions/";

            HttpResponseMessage response = await client.PostAsJsonAsync(cmdurl, new Login());
            response.EnsureSuccessStatusCode();

            Task<string> d = response.Content.ReadAsStringAsync();
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
              Priority = CacheItemPriority.High,
              SlidingExpiration = TimeSpan.FromMinutes(20)
            };
            root obj = JsonConvert.DeserializeObject<root>(d.Result);
            memoryCache.Set(cacheKey, cacheExpiryOptions);
            memoryCache.Set(cacheKey, obj, cacheExpiryOptions);
            return d.Result;
          }
          else
          {
              var user1 = memoryCache.Get(cacheKey);
              var data = JsonConvert.SerializeObject(user1);
              return data;
          }
          
      
          
        }

        public async Task<string> getMainImage(String classNameParam, String cardIdParam, String shapeIdParam)
        {
            var cacheKey = "ImageList";

            if (!memoryCache.TryGetValue(cacheKey, out root ImageList))
            {
                String mainImageName = "";
                IList imageList = await getImageList(classNameParam, cardIdParam, shapeIdParam);

                var data = JsonConvert.SerializeObject(imageList);
                IList obj = JsonConvert.DeserializeObject<IList>(data);
                
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                  Priority = CacheItemPriority.High,
                  SlidingExpiration = TimeSpan.FromMinutes(20)
                };
                memoryCache.Set(cacheKey, cacheExpiryOptions);
                memoryCache.Set(cacheKey, obj, cacheExpiryOptions);
                return data;
            }
            else
            {
              var data1 = memoryCache.Get(cacheKey);
              var data2 = JsonConvert.SerializeObject(data1);
              return data2;
            }
          
        }

        public async Task<IList> getImageList(String classNameParam, String cardIdParam, String shapeIdParam)
        {
            Task<String> sessionId = getCMDSesionId();
            root obj = JsonConvert.DeserializeObject<root>(sessionId.Result);
            
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            String urlToOpen = "http://amanaggarwal.com:8080/cmdbuild32/services/rest/v3/classes/" + classNameParam + "/cards/" + cardIdParam + "/attachments/?CMDBuild-Authorization=" + obj.data._id;

            HttpResponseMessage response = await client.GetAsync(urlToOpen);
            response.EnsureSuccessStatusCode();
            Task<string> d = response.Content.ReadAsStringAsync();
            
            ImageRoot myDeserializedClass = JsonConvert.DeserializeObject<ImageRoot>(d.Result);
            List<Imagelist> lstImageList = new List<Imagelist>();
            ArrayList imgList = new ArrayList();
            for (int i = 0; i < myDeserializedClass.data.Count; i++)
            {
                Imagelist img = new Imagelist();
                img._id = myDeserializedClass.data[i]._id;
                img.name= myDeserializedClass.data[i].name;
                var imgname = myDeserializedClass.data[i].name;
                var imgindex = imgname.IndexOf(".");
                var str1 = imgname.Substring(imgindex + 1, 3);
                var imgindex1 = str1.IndexOf(".");
                string res = "";
                if (imgindex1 != -1)
                  res = str1.Substring(imgindex1 + 1, imgname.Length);
                else
                  res = str1;
                //var res = str1.Substring(imgindex1 + 1, imgname.Length);
                if (res == "xlsx" || res == "doc" || res == "pdf")
                {
                  continue;
                }
              lstImageList.Add(img);
           }
           return lstImageList;
        }

        public async Task<HttpResponseMessage> getImageContent(String classNameParam, String cardIdParam, String imageName, String imgid)
        {
            Task<String> sessionId = getCMDSesionId();
            root obj = JsonConvert.DeserializeObject<root>(sessionId.Result);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            String urlToOpen = "http://amanaggarwal.com:8080/cmdbuild32/services/rest/v3/classes/" + classNameParam + "/cards/" + cardIdParam + "/attachments/" + imgid + "/" + imageName + "?CMDBuild-Authorization=" + obj.data._id;

            ////This will result in a content stream
            //String urlToOpen = "/services/rest/v3/classes/" + classNameParam + "/cards/" + cardIdParam + "/attachments/" + imgid + "/" + imageName + "?CMDBuild-Authorization=" + sessionId + "";
             HttpResponseMessage response = await client.GetAsync(urlToOpen);
             response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            
             return response;

          }

        private static HttpClient GetHttpClient()
        {
            String baseURL = "http://amanaggarwal.com:8080/cmdbuild32";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseURL);
            client.DefaultRequestHeaders.Accept.Clear();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        class Login
        {
            public String username = "imager"; /*CMDBuild Username*/
            public String password = "satishheap"; /*CMDBuild Password*/
        }
        public class LoginResponse
        {
          public string _id { get; set; }
          public string username { get; set; }
          public string role { get; set; }
          public List<string> availableRoles { get; set; }
        }
        public class Meta
        {
         
        }
        public class root
        {
          public LoginResponse data { get; set; }
          public Meta meta { get; set; }

        }
       
        public class Imagelist
        {
          public string _id { get; set; }
          public string name { get; set; }
          public object category { get; set; }
          public string description { get; set; }
          public string version { get; set; }
          public string author { get; set; }
          public DateTime created { get; set; }
          public DateTime modified { get; set; }
        }

        public class ImageMeta
        {
          public int total { get; set; }
        }

        public class ImageRoot
        {
          public bool success { get; set; }
          public List<Imagelist> data { get; set; }
          public ImageMeta meta { get; set; }
        }



  }
}
