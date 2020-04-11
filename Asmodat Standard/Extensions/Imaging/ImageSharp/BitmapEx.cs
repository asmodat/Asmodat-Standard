using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using System.Drawing.Imaging;
using AsmodatStandard.Extensions.Imaging;

namespace AsmodatStandard.Extensions.ImageSharp
{
    public static class BitmapEx
    {
        public static byte[] ToImageSharpByteArray(this System.Drawing.Bitmap bitmap, ImageFormat format = null, int quality = 100, int compressionLevel = 1)
        {
            if (format == null)
                format = bitmap.GetImageFormat();

            var newFormat = format.TryToImageSharpFormat().TryToImageFormat();

            if (newFormat == null)
                throw new Exception($"Format '{format?.ToString() ?? "undefined"}' is not supported, gif, png, jpg or bmp supported only.");

            var isImage = bitmap.ToImageSharpImage();
            using (var ms = new MemoryStream())
            {
                if (newFormat == ImageFormat.Gif)
                    isImage.Save(ms, new SixLabors.ImageSharp.Formats.Gif.GifEncoder()
                    {

                    });
                else if (newFormat == ImageFormat.Jpeg)
                    isImage.Save(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder()
                    {
                        Quality = quality
                    });
                else if (newFormat == ImageFormat.Png)
                    isImage.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder()
                    {
                        CompressionLevel = compressionLevel
                    });
                else if (newFormat == ImageFormat.Bmp)
                    isImage.Save(ms, new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder()
                    {

                    });
                else
                    throw new NotSupportedException($"Converting to byte array from '{newFormat?.ToString() ?? "undefined"}' is not supported, try gif, png, jpg or bmp.");

                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }
    }
}
