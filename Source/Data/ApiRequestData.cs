using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source
{
    public class ApiRequestData
    {
        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

        [JsonPropertyName("uri")]
        public Uri Uri { get; set; }
    }
}
