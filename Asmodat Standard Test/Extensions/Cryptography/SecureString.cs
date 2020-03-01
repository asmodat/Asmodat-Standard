using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;
using Asmodat.Cryptography.Bitcoin.Extension;

namespace Asmodat.Extensions.Cryptography
{
    public static class SecureStringEx
    {
        /// <summary>
        /// Converts a string into a secure string.
        /// </summary>
        /// <param name="value">the string to be converted.</param>
        /// <returns>The secure string converted from the input string </returns>
        public static SecureString ConvertToSecureString(this string value)
        {
            Guard.Require(value.IsNotNull());
            var secureString = new System.Security.SecureString();
            value.ToCharArray().ForEach(secureString.AppendChar);
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Converts the secure string to a string.
        /// </summary>
        /// <param name="secureString">the secure string to be converted.</param> 
        /// <returns>The string converted from a secure string </returns>
        public static string ConvertToString(this SecureString secureString)
        {
            Guard.Require(secureString.IsNotNull());

            var stringPointer = IntPtr.Zero;
            try
            {
                stringPointer = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringBSTR(stringPointer);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(stringPointer);
            }
        }
    }
}
