using System.Drawing;
using System.IO;
using System;
using AsmodatStandard.Extensions.Collections;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.ImageSharp
{
    public static partial class StringEx
    {

        public static Bitmap DataImageToBitmapWithImageSharp(this string data) => (Bitmap)data.DataImageToImageWithImageSharp();

        public static System.Drawing.Image DataImageToImageWithImageSharp(this string data)
        {
            if (data.IsNullOrEmpty() || !data.Contains("data:image"))
                return null;

            var base64Data = data.Split(',').SecondOrDefault();
            var imgData = base64Data.TryFromBase64();

            if (imgData.IsNullOrEmpty() || imgData.Length < 32)
                return null;

            var format = data.DataImageToImageSharpFormat();

            if (format == null)
                return null;

            using (var img = SixLabors.ImageSharp.Image.Load(imgData))
            {
                Console.WriteLine("4-DataImageToImageWithImageSharp");

                var bmp = img.ToBitmapGDI(format: format);

                Console.WriteLine("5-DataImageToImageWithImageSharp");
                return bmp;
            }
        }

        public static SixLabors.ImageSharp.Image DataImageToImageSharpImage(this string data)
        {
            if (data.IsNullOrEmpty() || !data.Contains("data:image"))
                return null;

            var base64Data = data.Split(',').SecondOrDefault();
            var imgData = base64Data.TryFromBase64();

            if (imgData.IsNullOrEmpty() || imgData.Length < 32)
                return null;

            var dataFormat = data.Split('/').SecondOrDefault()?.Split(';')?.FirstOrDefault();
            var format = dataFormat.DataImageToImageSharpFormat();

            if (dataFormat.IsNullOrEmpty() || format == null)
                return null;

            var img = SixLabors.ImageSharp.Image.Load(imgData);
            return img;
        }

        public static async Task<SixLabors.ImageSharp.Image> DataImageToImageSharpImageAsync(this string data)
        {
            if (data.IsNullOrEmpty() || !data.Contains("data:image"))
                return null;

            await Task.Delay(10);
            var base64Data = data.Split(',').SecondOrDefault();
            await Task.Delay(10);
            var imgData = base64Data.TryFromBase64();

            if (imgData.IsNullOrEmpty() || imgData.Length < 32)
                return null;

            await Task.Delay(10);
            var dataFormat = data.Split('/').SecondOrDefault()?.Split(';')?.FirstOrDefault();
            await Task.Delay(10);
            var format = dataFormat.DataImageToImageSharpFormat();

            if (dataFormat.IsNullOrEmpty() || format == null)
                return null;

            await Task.Delay(10);
            return SixLabors.ImageSharp.Image.Load(imgData);
        }
    }
}
