using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source
{
    public class ApiResponse
    {
        [JsonPropertyName("status")]
        public ApiResponseStatus Status { get; set; } = new ApiResponseStatus();

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        [JsonPropertyName("des")]
        public string Des { get; set; }
    }
}
