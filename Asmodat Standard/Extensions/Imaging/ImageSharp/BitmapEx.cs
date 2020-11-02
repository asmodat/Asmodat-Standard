using System;
using System.IO;
using SixLabors.ImageSharp.Formats.Png;
using System.Drawing.Imaging;
using AsmodatStandard.Extensions.Imaging;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;

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
                    isImage.Save(ms, new GifEncoder()
                    {

                    });
                else if (newFormat == ImageFormat.Jpeg)
                    isImage.Save(ms, new JpegEncoder()
                    {
                        Quality = quality
                    });
                else if (newFormat == ImageFormat.Png)
                    isImage.Save(ms, new PngEncoder()
                    {
                        CompressionLevel = compressionLevel
                    });
                else if (newFormat == ImageFormat.Bmp)
                    isImage.Save(ms, new BmpEncoder()
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
