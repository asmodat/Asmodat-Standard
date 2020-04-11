using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace AsmodatStandard.Extensions.Imaging
{
    public static class ImageFormatEx
    {
        public static ImageFormat GetImageFormat(this Image img)
        {
            if (img == null)
                return null;

            if (img.RawFormat.Equals(ImageFormat.Gif))
                return ImageFormat.Gif;
            else if (img.RawFormat.Equals(ImageFormat.Jpeg))
                return ImageFormat.Jpeg;
            else if (img.RawFormat.Equals(ImageFormat.Png))
                return ImageFormat.Png;
            else if (img.RawFormat.Equals(ImageFormat.Bmp))
                return ImageFormat.Bmp;
            else if (img.RawFormat.Equals(ImageFormat.Tiff))
                return ImageFormat.Tiff;
            else if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
                return ImageFormat.MemoryBmp;
            else if (img.RawFormat.Equals(ImageFormat.Emf))
                return ImageFormat.Emf;
            else if (img.RawFormat.Equals(ImageFormat.Exif))
                return ImageFormat.Exif;
            else if (img.RawFormat.Equals(ImageFormat.Icon))
                return ImageFormat.Icon;
            else if (img.RawFormat.Equals(ImageFormat.Wmf))
                return ImageFormat.Wmf;

            throw new Exception("Unknown image format");
        }
    }
}
