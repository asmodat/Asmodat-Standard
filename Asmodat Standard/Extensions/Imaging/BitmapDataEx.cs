using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing;

namespace AsmodatStandard.Extensions.Imaging
{
    public static class BitmapDataEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this BitmapData bmd)
        {
            if (bmd == null || bmd.Width <= 0 || bmd.Height <= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Copies one line at a time, to support diffrent kinds of memory allocation
        /// </summary>
        /// <param name="bmd"></param>
        /// <returns></returns>
        public static byte[] ToByteArrayScan(this BitmapData bmd)
        {
            if (bmd.IsNullOrEmpty())
                return null;

            byte[] result = new byte[bmd.Stride * bmd.Height];

            int offset = 0, i = 0, w = bmd.Width, h = bmd.Height, bpp = bmd.PixelFormat.GetBitsPerPixel();
            long ptr = bmd.Scan0.ToInt64();

            int wtBpp = w * (bpp / 8);
            int htBpp = h * (bpp / 8);

            for (; i < h; i++)
            {
                Marshal.Copy(new IntPtr(ptr), result, offset, wtBpp);
                offset += wtBpp;
                ptr += bmd.Stride;
            }

            return result;
        }


        public static byte[] ToByteArray(this BitmapData bmd)
        {
            if (bmd.IsNullOrEmpty())
                return null;

            byte[] result = new byte[bmd.Stride * bmd.Height];
            Marshal.Copy(bmd.Scan0, result, 0, result.Length);
            return result;
        }
        
        /// <summary>
        /// Warning ! this array returns [y, x, z] format -> [width, height, pixels]
        /// </summary>
        /// <param name="bmd"></param>
        /// <returns></returns>
        public static byte[,,] ToByteArray3D_YXZ(this BitmapData bmd)
        {
            if (bmd.IsNullOrEmpty())
                return null;

            byte[] data = bmd.ToByteArrayScan();

            if (data.IsNullOrEmpty())
                return null;

            int d = bmd.PixelFormat.GetBitsPerPixel() / 8, w = bmd.Width, h = bmd.Height, l = data.Length * sizeof(byte);

            byte[,,] result = new byte[h, w, d];


            Buffer.BlockCopy(data, 0, result, 0, l);

            return result;
        }

         public static Bitmap BitmapDataByteArray3DToBitmap(this byte[,,] data, PixelFormat format)
         {
             if (data.IsAnyDimentionNullOrEmpty())
                 return null;

             int width = data.Height(), height = data.Width(), depth = data.Depth();

             byte[] result = new byte[width * height * depth];
             Buffer.BlockCopy(data, 0, result, 0, result.Length);

             Bitmap bmp = new Bitmap(width, height, format);
             BitmapData bmd = bmp.LockBitsRW();
             Marshal.Copy(result, 0, bmd.Scan0, data.Length);
             bmp.UnlockBits(bmd);

             return bmp;
         }

         public static Bitmap BitmapDataByteArrayToBitmap(this byte[] data, int width, int height, PixelFormat format)
         {
             if (data.IsNullOrEmpty())
                 return null;

             Bitmap bmp = new Bitmap(width, height, format);
             BitmapData bmd = bmp.LockBitsRW();
             Marshal.Copy(data, 0, bmd.Scan0, data.Length);
             bmp.UnlockBits(bmd);

             return bmp;
         }
    }
}
