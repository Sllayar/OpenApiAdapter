using Microsoft.AspNetCore.Http;
using OpenApiAdapter.Source.Environment;
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
        public static HttpResponseMessage GetResponse(dynamic request, IHeaderDictionary headers, HttpMethod httpMethod)
        {
            ApiSafeData CriptoSafeData = new ApiSafeData()
            {
                Data = TripleDESHelper.Encrypt(JsonSerializer.Serialize(TryGetValueFromRequest(request)), out string desParameters),
                Des = RSAHelper.Encrypt(desParameters, Env.RfiPublicKey),
                Signature = RSAHelper.Sign(desParameters, Env.PartnerPrivateKey)
            };

            if (String.IsNullOrEmpty(Env.CertificateFilePath))
            {
                using (var clientHandler = new HttpClientHandler()
                { 
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; } 
                })
                {
                    return Resend(headers, httpMethod, clientHandler, CriptoSafeData);
                }    
            }
            else
            {
                using (var clientHandler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12,
                    ClientCertificates = { new X509Certificate2(Env.CertificateFilePath) }
                })
                {
                    return Resend(headers, httpMethod, clientHandler, CriptoSafeData);
                }
            }
        }

        private static HttpResponseMessage Resend(IHeaderDictionary headers, HttpMethod httpMethod, 
            HttpClientHandler clientHandler, ApiSafeData CriptoSafeData)
        {
            using (HttpClient httpClient = new HttpClient(clientHandler))
            {
                using (var content = new StringContent(JsonSerializer.Serialize(CriptoSafeData), Encoding.UTF8, "application/json"))
                {
                    using (var httpRequestMessage = new HttpRequestMessage(httpMethod, Env.OpenApiUri))
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

        private static Object TryGetValueFromRequest(dynamic request)
        {
            try
            { 
                return request.Data; 
            }
            catch(Exception)
            { 
                return request; 
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
