using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Types;
using System.Linq;
using AsmodatStandard.Cryptography.Bitcoin;

namespace AsmodatStandard.Extensions.Cryptography
{
    public static class Bech32Ex
    {
        public static bool TryDecode(string encoded, out string hrp, out byte[] bytes)
        {
            try
            {
                bytes = Bech32.Decode(encoded, out var thrp);
                hrp = thrp?.Trim(' ','\n','\r','\t');
                return true;
            }
            catch(Exception ex)
            {
                hrp = null;
                bytes = null;
                return false;
            }
        }

        public static bool CanDecode(string encoded)
        {
            try
            {

                var bytes = Bech32.Decode(encoded, out var thrp);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    
}

