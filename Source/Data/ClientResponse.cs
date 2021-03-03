using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source
{
    public class ClientResponse
    {
        [JsonPropertyName("status")]
        public ApiResponseStatus Status { get; set; }

        [JsonPropertyName("data")]
        public dynamic Data { get; set; }
    }
}
