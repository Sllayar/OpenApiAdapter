using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ClientResponse Retranslate([FromBody] ApiRequestData safeData)
        {
            try
            {
                ApiResponse openApirespoinse = new ApiResponse();

                var response = OpenApiClient.GetResponse(safeData, Request.Headers, new HttpMethod(Request.Method));

                var res = response.Content?.ReadAsStringAsync().Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    openApirespoinse = JsonSerializer.Deserialize<ApiResponse>(res);
                else
                    throw new Exception(String.Format("Response status code is {0}. Body: {1}", response.StatusCode, res));

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

            string des = RSAHelper.Decrypt(response, OpenApiConfig.PartnerKeyPrivate);

            if (!RSAHelper.Verify(des, signature, OpenApiConfig.RfiBankKeyPublic)) 
                throw new Exception("Signature not valid");

                return TripleDESHelper.Decrypt(response, des);
        }
    }
}
