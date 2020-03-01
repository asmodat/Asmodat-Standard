using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AsmodatStandard.Extensions.Security
{
    public static class SecureStringEx
    {
        public static String Release(this SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static SecureString ToSecureString(this string s)
        {
            var ss = new SecureString();
            foreach (char c in s)
                ss.AppendChar(c);

            return ss;
        }
    }
}
