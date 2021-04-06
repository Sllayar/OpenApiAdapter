using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OpenApiAdapter.Source.Environment;
using OpenApiAdapter.Source.Helpers.Crypt;
using OpenApiAdapter.Source.Integration;
using RFI;
using RFI.Helpers.Crypt;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source
{
    [Route("{*url}")]
    [ApiController]
    public class RetranslateController : ControllerBase
    {

        [HttpPost, HttpDelete, HttpDelete, HttpGet, HttpPut]
        public ClientResponse Retranslate([FromBody] dynamic requestData)
        {
            try
            {
                var response = OpenApiClient.GetResponse(requestData, Request.Headers, new HttpMethod(Request.Method), Request.Path);

                var res = response.Content?.ReadAsStringAsync().Result;
                
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return new ClientResponse()
                    {
                        Status = new ApiResponseStatus()
                        {
                            Code = -2,
                            Detail = String.Format("Response status code is {0}.", response.StatusCode)
                        }
                    };

                ApiResponse openApirespoinse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(res);
                dynamic data = openApirespoinse.Data is null ? null : Decryptor.DecriptRespose(openApirespoinse);

                return new ClientResponse()
                {
                    Status = openApirespoinse.Status,
                    Data = data is null ? null : JsonConvert.DeserializeObject<ExpandoObject>(data, new ExpandoObjectConverter())
                };
            }
            catch (Exception ex)
            {
                return new ClientResponse()
                {
                    Status = new ApiResponseStatus() { Code = -1, Detail = ex.Message }
                };
            }
        }
    }
}
