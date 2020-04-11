using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.Drawing.Imaging;

namespace AsmodatStandard.Extensions.Imaging
{
    public static class PixelFormatEx
    {
        public static int GetBitsPerPixel(this PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format16bppArgb1555: return 16;
                case PixelFormat.Format16bppGrayScale: return 16;
                case PixelFormat.Format16bppRgb555: return 16;
                case PixelFormat.Format16bppRgb565: return 16;
                case PixelFormat.Format1bppIndexed: return 1;
                case PixelFormat.Format24bppRgb: return 24;
                case PixelFormat.Format32bppArgb: return 32;
                case PixelFormat.Format32bppPArgb: return 32;
                case PixelFormat.Format32bppRgb: return 32;
                case PixelFormat.Format4bppIndexed: return 4;
                case PixelFormat.Format64bppArgb: return 64;
                case PixelFormat.Format64bppPArgb: return 64;
                case PixelFormat.Format8bppIndexed: return 8;
                default: throw new Exception("GetBitsPerPixel not supported format");

            }
        }
    }
}
