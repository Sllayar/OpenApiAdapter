using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source
{
    public class ApiResponse : ApiSafeData
    {
        [JsonPropertyName("status")]
        public ApiResponseStatus Status { get; } = new ApiResponseStatus();

        public void SetStatus(int code, string description)
        {
            Status.Code = code;
            Status.Detail = description;
        }

        public void SetSafeData(ApiSafeData safeData)
        {
            Data = safeData.Data;
            Signature = safeData.Signature;
            Des = safeData.Des;
        }
    }
}
