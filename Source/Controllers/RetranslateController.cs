using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenApiAdapter.Source.Environment;
using OpenApiAdapter.Source.Integration;
using RFI;
using RFI.Helpers.Crypt;
using System;
using System.Collections.Generic;
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


                var response = OpenApiClient.GetResponse(requestData, Request.Headers, new HttpMethod(Request.Method));

                var res = response.Content?.ReadAsStringAsync().Result;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return new ClientResponse() 
                    {
                        Status = new ApiResponseStatus() 
                        { 
                            Code = -2,
                            Detail = String.Format("Response status code is {0}. Body: {1}", response.StatusCode, res) 
                        }
                    };

                ApiResponse openApirespoinse = JsonSerializer.Deserialize<ApiResponse>(res);

                return new ClientResponse()
                {
                    Status = openApirespoinse.Status,
                    Data = DecriptRespose(openApirespoinse.Data)
                };
            }
            catch (Exception ex)
            {


                return new ClientResponse()
                {
                    Status = new ApiResponseStatus() { Code = -1, Detail = ex.Message}
                };
            }
        }

        public static string DecriptRespose(string response)
        {
            string signature = "";

            string des = RSAHelper.Decrypt(response, Env.PartnerPrivateKey);

            if (!RSAHelper.Verify(des, signature, Env.RfiPublicKey)) 
                throw new Exception("Signature not valid");

                return TripleDESHelper.Decrypt(response, des);
        }
    }
}
