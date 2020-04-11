using System.Drawing;
using System.IO;
using System;
using AsmodatStandard.Extensions.Collections;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing.Imaging;

namespace AsmodatStandard.Extensions.Imaging
{
    public static partial class StringEx
    {
        public static ImageFormat DataImageToImageFormat(this string img)
        {
            img = img?.ToLower()?.Trim(' ',',','.');
            if (img == null)
                return null;

            if (img == "gif")
                return ImageFormat.Gif;
            else if (img == "jpg")
                return ImageFormat.Jpeg;
            else if (img == "png")
                return ImageFormat.Png;
            else if (img == "bpm")
                return ImageFormat.Bmp;
            else if (img == "tiff")
                return ImageFormat.Tiff;
            else if (img == "memorybmp" || img == "raw")
                return ImageFormat.MemoryBmp;
            else if (img == "emf")
                return ImageFormat.Emf;
            else if (img == "exif")
                return ImageFormat.Exif;
            else if (img == "icon")
                return ImageFormat.Icon;
            else if (img == "wmf")
                return ImageFormat.Wmf;

            throw new Exception($"Unknown image format '{img}'.");
        }

        public static Bitmap DataImageToBitmap(this string data) => (Bitmap)data.DataImageToImage();

        public static System.Drawing.Image DataImageToImage(this string data)
        {
            if (data.IsNullOrEmpty() || !data.Contains("data:image"))
                return null;

            var base64Data = data.Split(',').SecondOrDefault();
            var imgData = base64Data.TryFromBase64();

            if (imgData.IsNullOrEmpty() || imgData.Length < 32)
                return null;

            System.Drawing.Image img = null;
            using (var ms = new MemoryStream(imgData))
            {
                ms.Position = 0;
                img = System.Drawing.Image.FromStream(ms);
            };

            return img;
        }
    }
}
