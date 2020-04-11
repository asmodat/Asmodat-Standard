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
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using System.Drawing.Imaging;
using AsmodatStandard.Extensions.Imaging;
using System.Drawing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.ColorSpaces;

namespace AsmodatStandard.Extensions.ImageSharp
{
    public static class ImageEx
    {
        public static string ToDataImage(this SixLabors.ImageSharp.Image img, IImageFormat format = null, PngColorType pngColorType = PngColorType.RgbWithAlpha)
        {
            if (img == null)
                return null;

            format = format ?? PngFormat.Instance;
            var formatName = format.ToDataImageFormatName();

            return img.IsNullOrEmpty() ?
                $"data:image/{formatName};base64,00" :
                $"data:image/{formatName};base64,{img.ToByteArray(format, pngColorType: pngColorType).ToBase64String()}";
        }

        public static bool IsNullOrEmpty(this SixLabors.ImageSharp.Image image) => image == null || image.Width <= 0 || image.Height <= 0;

        public static System.Drawing.Bitmap ToBitmap(this SixLabors.ImageSharp.Image image, System.Drawing.Imaging.ImageFormat format = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                var config = image.GetConfiguration();
                var imageEncoder = config.ImageFormatsManager.FindEncoder(format.TryToImageSharpFormat() ?? PngFormat.Instance);

                image.Save(memoryStream, imageEncoder);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return new System.Drawing.Bitmap(memoryStream);
            }
        }

        public static System.Drawing.Bitmap ToBitmapGDI<TPixel>(this Image<TPixel> image, System.Drawing.Imaging.ImageFormat format = null) where TPixel : struct, IPixel<TPixel>
            => image.ToBitmapGDI(format?.TryToImageSharpFormat());
        
        public static System.Drawing.Bitmap ToBitmapGDI<TPixel>(this Image<TPixel> image, IImageFormat format = null) where TPixel : struct, IPixel<TPixel>
        {
            using (var memoryStream = new MemoryStream())
            {
                var config = image.GetConfiguration();
                var imageEncoder = config.ImageFormatsManager.FindEncoder(format ?? PngFormat.Instance);

                image.Save(memoryStream, imageEncoder);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return new System.Drawing.Bitmap(memoryStream);
            }
        }

        public static byte[] GetColorData<TPixel>(this Image<TPixel> image) where TPixel : struct, IPixel<TPixel>
        {
            if (image == null)
                return null;

            if (image.Width <= 0 || image.Height <= 0)
                return new byte[0];

            return MemoryMarshal.AsBytes(image.GetPixelSpan()).ToArray();
        }

        public static byte[] GetColorData(this SixLabors.ImageSharp.Image image)
        {
            if (image == null)
                return null;

            if (image.Width <= 0 || image.Height <= 0)
                return new byte[0];
        
            if (image.PixelType.BitsPerPixel == 8 * 3)
                return MemoryMarshal.AsBytes(((Image<Rgb24>)image).GetPixelSpan()).ToArray();
            else if (image.PixelType.BitsPerPixel == 8 * 4)
                return MemoryMarshal.AsBytes(((Image<Rgba32>)image).GetPixelSpan()).ToArray();
            else if (image.PixelType.BitsPerPixel == 8 * 6)
                return MemoryMarshal.AsBytes(((Image<Rgb48>)image).GetPixelSpan()).ToArray();
            else if (image.PixelType.BitsPerPixel == 8 * 8)
                return MemoryMarshal.AsBytes(((Image<Rgba64>)image).GetPixelSpan()).ToArray();
            else
                throw new NotSupportedException($"Unsupported image pixel format bpp: {image.PixelType.BitsPerPixel}");
        }


        public static Image<Rgb24> ToImageSharpImage(this System.Drawing.Bitmap bitmap)
        {
            if (bitmap.IsNullOrEmpty())
                return null;

            Bitmap bmp24Bit;
            try
            {
                if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                    bmp24Bit = bitmap.Convert(PixelFormat.Format24bppRgb);
                else
                    bmp24Bit = bitmap;
            }
            catch
            {
                bmp24Bit = bitmap;
            }

            var pixels = bmp24Bit.GetColorData24Bit();
            var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(pixels, bitmap.Width, bitmap.Height);
            return image;
        }

        public static SixLabors.ImageSharp.Image ToImageSharpImageGDI(this System.Drawing.Bitmap bitmap)
        {
            if (bitmap.IsNullOrEmpty())
                return null;

            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return SixLabors.ImageSharp.Image.Load(memoryStream);
            }
        }

        public static System.Drawing.Bitmap ImageSharpConvert(this System.Drawing.Bitmap bitmap, ImageFormat format, int quality = 100, int compressionLevel = 1)
        {
            var newFormat = format.TryToImageSharpFormat().TryToImageFormat();

            if (newFormat == null)
                throw new Exception($"Converting to '{format?.ToString() ?? "undefined"}' is not supported, try gif, png, jpg or bmp.");

            var data = bitmap.ToImageSharpByteArray(
                format: newFormat, 
                quality: quality, 
                compressionLevel: compressionLevel);
            using (var ms = new MemoryStream(data))
            {
                ms.Seek(0, SeekOrigin.Begin);
                var img = SixLabors.ImageSharp.Image.Load(ms);
                var bmp = img.ToBitmap(format);
                return bmp;
            }
        }

        public static SixLabors.ImageSharp.Image Convert(this SixLabors.ImageSharp.Image img, string format, 
            int quality = 100,
            int compressionLevel = 1,
            PngColorType pngColorType = PngColorType.Rgb,
            BmpBitsPerPixel bmpBitsPerPixel = BmpBitsPerPixel.Pixel32,
            PngBitDepth? pngBitDepth = null)
        {
            var isFormat = format.DataImageToImageSharpFormat();
            return img.Convert(isFormat, 
                quality: quality, 
                compressionLevel: compressionLevel, 
                pngColorType: pngColorType, 
                bmpBitsPerPixel: bmpBitsPerPixel,
                pngBitDepth: pngBitDepth);
        }

        public static SixLabors.ImageSharp.Image Convert(this SixLabors.ImageSharp.Image img, IImageFormat format, 
            int quality = 100, 
            int compressionLevel = 1,
            PngColorType pngColorType = PngColorType.Rgb,
            BmpBitsPerPixel bmpBitsPerPixel = BmpBitsPerPixel.Pixel32,
            PngBitDepth? pngBitDepth = null)
        {
            if (format == null)
                throw new Exception($"Converting to '{format?.ToString() ?? "undefined"}' is not supported, try gif, png, jpg or bmp.");

            var arr = img.ToByteArray(format, 
                quality: quality, 
                compressionLevel: compressionLevel, 
                pngColorType: pngColorType, 
                bmpBitsPerPixel: bmpBitsPerPixel,
                pngBitDepth: pngBitDepth);
            SixLabors.ImageSharp.Image newImg = null;

            if (format.Name == BmpFormat.Instance.Name)
            {
                if (bmpBitsPerPixel == BmpBitsPerPixel.Pixel24)
                    newImg = SixLabors.ImageSharp.Image.Load<Rgb24>(arr);
                else if (bmpBitsPerPixel == BmpBitsPerPixel.Pixel32)
                    newImg = SixLabors.ImageSharp.Image.Load<Rgba32>(arr);
                if (bmpBitsPerPixel == BmpBitsPerPixel.Pixel16)
                    newImg = SixLabors.ImageSharp.Image.Load<Gray16>(arr);
                if (bmpBitsPerPixel == BmpBitsPerPixel.Pixel8)
                    newImg = SixLabors.ImageSharp.Image.Load<Gray8>(arr);
            }
            else if (format.Name == PngFormat.Instance.Name)
            {
                if (pngColorType == PngColorType.Rgb)
                    newImg = SixLabors.ImageSharp.Image.Load<Rgb24>(arr);
                else if (pngColorType == PngColorType.RgbWithAlpha)
                    newImg = SixLabors.ImageSharp.Image.Load<Rgba32>(arr);
                if (pngColorType == PngColorType.GrayscaleWithAlpha)
                    newImg = SixLabors.ImageSharp.Image.Load<Gray16>(arr);
                if (pngColorType == PngColorType.GrayscaleWithAlpha)
                    newImg = SixLabors.ImageSharp.Image.Load<Gray8>(arr);
            }

            newImg = newImg ?? SixLabors.ImageSharp.Image.Load(arr);
            return newImg;
        }

        public static byte[] ToByteArray(this SixLabors.ImageSharp.Image img, string format,
            int quality = 100,
            int compressionLevel = 1,
            PngColorType pngColorType = PngColorType.Rgb,
            BmpBitsPerPixel bmpBitsPerPixel = BmpBitsPerPixel.Pixel32,
            PngBitDepth? pngBitDepth = null)
        {
            var isFormat = format.DataImageToImageSharpFormat();
            return img.ToByteArray(isFormat, 
                quality: quality, 
                compressionLevel: compressionLevel, 
                pngColorType: pngColorType,
                bmpBitsPerPixel: bmpBitsPerPixel,
                pngBitDepth: pngBitDepth);
        }

        public static byte[] ToByteArray(
            this SixLabors.ImageSharp.Image img, 
            IImageFormat format, 
            int quality = 100, 
            int compressionLevel = 1,
            PngColorType pngColorType = PngColorType.Rgb,
            BmpBitsPerPixel bmpBitsPerPixel = BmpBitsPerPixel.Pixel32,
            PngBitDepth? pngBitDepth = null)
        {
            if (img.IsNullOrEmpty())
                return null;

            using (var ms = new MemoryStream())
            {
                if (format.Name == GifFormat.Instance.Name)
                    img.Save(ms, new GifEncoder()
                    {
                        
                    });
                else if (format.Name == JpegFormat.Instance.Name)
                    img.Save(ms, new JpegEncoder()
                    {
                        Quality = quality,
                        
                    });
                else if (format.Name == PngFormat.Instance.Name)
                    img.Save(ms, new PngEncoder()
                    {
                        CompressionLevel = compressionLevel,
                        ColorType = pngColorType,
                        BitDepth = pngBitDepth,
                        
                    });
                else if (format.Name == BmpFormat.Instance.Name)
                    img.Save(ms, new BmpEncoder()
                    {
                        BitsPerPixel = bmpBitsPerPixel
                    });
                else
                    throw new NotSupportedException($"Converting to '{format.Name?.ToString() ?? "undefined"}' is not supported, try gif, png, jpg or bmp.");

                ms.Seek(0, SeekOrigin.Begin);
                var arr = ms.ToArray();
                return arr;
            }
        }
    }
}
