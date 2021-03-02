using RFI;
using RFI.Helpers.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenApiAdapter.Source.Helpers.Crypt
{
    public static class Decryptor
    {

        public static string Decipher(ApiResponse response)
        {
            string des = RSAHelper.Decrypt(response.Des, OpenApiConfig.PartnerKeyPrivate);

            if (!RSAHelper.Verify(response.Des, response.Signature, OpenApiConfig.RfiBankKeyPublic))
                throw new Exception("Signature not valid");

            return TripleDESHelper.Decrypt(response.Data, response.Des);
        }
    }
}
