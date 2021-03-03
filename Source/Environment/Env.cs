using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source.Environment
{
    public static class Env
    {
        public static string Get(string name) => System.Environment.GetEnvironmentVariable(name);

        public static readonly bool IsDebug = Debugger.IsAttached;

        public static string OpenApiUri => Get("OPEN_API_URI") ?? throw new Exception("OpenApiUri not set");

        public static string RfiPublicKey => Get("RFI_KEY_PUBLIC") ?? throw new Exception("RfiPublicKey not set");

        public static string PartnerPrivateKey => Get("PARTNER_KEY_PRIVATE") ?? throw new Exception("PartnerPrivateKey not set");

        public static string CertificateFilePath => Get("CERTIFICATE_FILE");
    }
}
