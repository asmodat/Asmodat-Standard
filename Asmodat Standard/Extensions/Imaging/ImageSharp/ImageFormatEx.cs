using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.Drawing.Imaging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Extensions.ImageSharp
{
    public static class ImageFormatEx
    {
        public static string ToDataImageFormatName(this IImageFormat format)
        {
            var name = format?.Name;
            if (name.IsNullOrEmpty())
                return null;

            if (name == SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance.Name)
                return "gif";
            else if (name == SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance.Name)
                return "jpg";
            else if (name == SixLabors.ImageSharp.Formats.Png.PngFormat.Instance.Name)
                return "png";
            else if (name == SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance.Name)
                return "bmp";

            throw new Exception($"Unknown image sharp format '{name}'.");
        }

        public static IImageFormat DataImageToImageSharpFormat(this string img)
        {
            img = (img?.Split('/').SecondOrDefault()?.Split(';')?.FirstOrDefault() ?? img)
                .ToLower()?.Trim(' ', ',', '.');

            if (img == null)
                return null;

            if (img == "gif")
                return SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance;
            else if (img == "jpg")
                return SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance;
            else if (img == "png")
                return SixLabors.ImageSharp.Formats.Png.PngFormat.Instance;
            else if (img == "bpm")
                return SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance;

            throw new Exception($"Unknown image sharp format '{img}'.");
        }

        public static SixLabors.ImageSharp.Formats.IImageFormat
            TryToImageSharpFormat(this System.Drawing.Imaging.ImageFormat format)
        {
            try
            {
                return ToImageSharpFormat(format);
            }
            catch
            {
                return null;
            }
        }

        public static SixLabors.ImageSharp.Formats.IImageFormat
            ToImageSharpFormat(this System.Drawing.Imaging.ImageFormat format)
        {
            if (format == System.Drawing.Imaging.ImageFormat.Jpeg)
                return SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance;
            else if (format == System.Drawing.Imaging.ImageFormat.Gif)
                return SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance;
            else if (format == System.Drawing.Imaging.ImageFormat.Png)
                return SixLabors.ImageSharp.Formats.Png.PngFormat.Instance;
            else if (format == System.Drawing.Imaging.ImageFormat.Bmp)
                return SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance;

            throw new Exception($"Image format {format.ToString()} was not found.");
        }

        public static System.Drawing.Imaging.ImageFormat
            TryToImageFormat(this SixLabors.ImageSharp.Formats.IImageFormat format)
        {
            try
            {
                return ToImageFormat(format);
            }
            catch
            {
                return null;
            }
        }

        public static System.Drawing.Imaging.ImageFormat 
            ToImageFormat(this SixLabors.ImageSharp.Formats.IImageFormat format)
        {
            if (format.Name == SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance.Name)
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            else if (format.Name == SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance.Name)
                return System.Drawing.Imaging.ImageFormat.Gif;
            else if (format.Name == SixLabors.ImageSharp.Formats.Png.PngFormat.Instance.Name)
                return System.Drawing.Imaging.ImageFormat.Png;
            else if (format.Name == SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance.Name)
                return System.Drawing.Imaging.ImageFormat.Bmp;

            throw new Exception($"Image format {format.Name.ToString()} was not found.");
        }
    }
}
