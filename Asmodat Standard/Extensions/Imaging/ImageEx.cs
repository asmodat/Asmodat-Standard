using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using SixLabors.ImageSharp.Formats.Bmp;

namespace AsmodatStandard.Extensions.Imaging
{
    public static class ImageEx
    {

        public static Bitmap Resize(this Image image, int width, int height)
        {
            if (image.IsNullOrEmpty())
                return null;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image Convert(this Image img, ImageFormat format, long quality = 100L)
        {
            if (img.IsNullOrEmpty())
                return null;

            ImageCodecInfo formatEncoder = GetEncoder(format);
            var qualityEncoder = System.Drawing.Imaging.Encoder.Quality;

            var eParams = new EncoderParameters(1);
            eParams.Param[0] = new EncoderParameter(qualityEncoder, quality);

            using (var ms = new MemoryStream())
            {
                img.Save(ms, formatEncoder, eParams);

                ms.Position = 0;
                return Image.FromStream(ms);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this Image img) => (img == null || img.Width <= 0 || img.Height <= 0);

        public static byte[] ToByteArray(this Image img, ImageFormat format = null)
        {
            if (img.IsNullOrEmpty())
                return null;

            if (format == null)
                format = img.GetImageFormat();

            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, format);
                stream.Position = 0;
                return stream.ToArray();
            }
        }

        
    }
}
