using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionAppTest
{
    public static class Function2
    {
        [FunctionName("FunctionTest")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            string orderId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "orderId", true) == 0)
                .Value;

            string messageId = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "messageId", true) == 0)
                .Value;

            if (name == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.name;
            }
            if (orderId == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.orderId;
            }
            if (messageId == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.messageId;
            }


            //string message = string.Format("Orden Número {0} - Monto ${1} - Estado: Aprobada", orderId, orderId == "1" ? "55000" : "11000");

            await Task.Delay(8000);

            await SendOrderMessage(messageId, orderId);
       
            

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "");
        }

        public static async Task<string> SendOrderMessage(string messageId,string orderId)
        {
            using (var client = new HttpClient())
            {

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("ordenId", orderId);
                dictionary.Add("messsageId", messageId);
                dictionary.Add("total", (int.Parse(orderId)*1000).ToString() );
                dictionary.Add("estado", "Pendiente");

                string json = JsonConvert.SerializeObject(dictionary);
                var requestData = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "http://localhost:3978/events/123";
                //var url = "https://mggWebApp.azurewebsites.net/api/messages/events/123";

                var response = await client.PostAsync(String.Format(url), requestData);

                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }
    }
}
