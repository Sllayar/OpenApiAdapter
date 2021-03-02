using Microsoft.AspNetCore.Http;
using RFI;
using RFI.Helpers.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source.Integration
{
    public static class OpenApiClient
    {
        public static HttpResponseMessage GetResponse(ApiRequestData request, IHeaderDictionary headers, HttpMethod httpMethod)
        {
            ApiSafeData CriptoSafeData = new ApiSafeData()
            {
                Data = TripleDESHelper.Encrypt(JsonSerializer.Serialize(request.Data), out string desParameters),
                Des = RSAHelper.Encrypt(desParameters, OpenApiConfig.RfiBankKeyPublic),
                Signature = RSAHelper.Sign(desParameters, OpenApiConfig.PartnerKeyPrivate)
            };

            if (String.IsNullOrEmpty(OpenApiConfig.CertificateFileName))
            {
                using (var clientHandler = new HttpClientHandler()
                { 
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; } 
                })
                {
                    return Resend(request, headers, httpMethod, clientHandler, CriptoSafeData);
                }    
            }
            else
            {
                using (var clientHandler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12,
                    ClientCertificates = { new X509Certificate2(OpenApiConfig.CertificateFileName) }
                })
                {
                    return Resend(request, headers, httpMethod, clientHandler, CriptoSafeData);
                }
            }
        }

        private static HttpResponseMessage Resend(ApiRequestData request, IHeaderDictionary headers, 
            HttpMethod httpMethod, HttpClientHandler clientHandler, ApiSafeData CriptoSafeData)
        {
            using (HttpClient httpClient = new HttpClient(clientHandler))
            {
                using (var content = new StringContent(JsonSerializer.Serialize(CriptoSafeData), Encoding.UTF8, "application/json"))
                {
                    using (var httpRequestMessage = new HttpRequestMessage(httpMethod, request.Uri))
                    {
                        foreach (var header in headers)
                            if (!cannotModifiedHeaders.Contains(header.Key))
                                foreach (var value in header.Value)
                                    httpRequestMessage.Headers.Add(header.Key, value);

                        httpRequestMessage.Content = content;

                        return httpClient.SendAsync(httpRequestMessage).Result;
                    }
                }
            }
        }

        private static List<string> cannotModifiedHeaders = new List<string>()
        {
            "Accept", "Connection", "Content-Length", "Content-Type", "Date",
            "Expect", "Host", "If-Modified-Since", "Range", "Referer",
            "Transfer-Encoding", "User-Agent", "Proxy-Connection"
        };

    }
}
