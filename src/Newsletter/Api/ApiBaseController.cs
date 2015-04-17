using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace BVNetwork.EPiSendMail.Api
{
    public class ApiBaseController : ApiController
    {
        protected JObject GetJsonResult<T>(T result)
        {
            return JObject.FromObject(result);
        }

    }
}
