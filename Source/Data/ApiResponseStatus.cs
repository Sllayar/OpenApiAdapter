using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source
{
    public class ApiResponseStatus
    {
        [JsonPropertyName("code")]
        public int Code { get; set; } = -1;

        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }
}
