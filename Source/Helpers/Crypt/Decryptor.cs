﻿using OpenApiAdapter.Source.Environment;
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
            string des = RSAHelper.Decrypt(response.Des, Env.PartnerPrivateKey);

            if (!RSAHelper.Verify(response.Des, response.Signature, Env.RfiPublicKey))
                throw new Exception("Signature not valid");

            return TripleDESHelper.Decrypt(response.Data, response.Des);
        }

        public static string DecriptRespose(ApiResponse response)
        {
            string des = RSAHelper.Decrypt(response.Des, Env.PartnerPrivateKey);

            if (!RSAHelper.Verify(des, response.Signature, Env.RfiPublicKey))
                throw new Exception("Signature not valid");

            return TripleDESHelper.Decrypt(response.Data, des);
        }
    }
}
