using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using QRCoder;
using ZXing;
using AsmodatStandard.Extensions.Imaging;
using AsmodatStandard.Extensions.ImageSharp;
using System.Drawing.Imaging;
using System.IO;
using SixLabors.ImageSharp;
using ZXing.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using System.Numerics;
using AsmodatStandard.Extensions.Collections;
using SixLabors.Primitives;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AsmodatStandard.Extensions.Imaging
{
    public static class QRScannerEx
    {
        private const double _defaultScale = ((double)9 / 10);
        private const int _defaultThickness = 3;

        public static async Task<SixLabors.ImageSharp.Image> CropTargetAsync(
            SixLabors.ImageSharp.Image img,
            bool clone = false,
            double scale = _defaultScale,
            int thickness = _defaultThickness,
            int delay = 0)
        {
            var minDimention = Math.Min(img.Width, img.Height);
            var cropSize = (int)(Math.Floor(minDimention * scale) - thickness);
            var targetVectors = await GetTargetVectorsAsync(img, 
                scale: ((double)cropSize / minDimention), 
                thickness: thickness);

            if (targetVectors.IsNullOrEmpty())
                return null;

            var rectangle = TryGetTargetRectangle(targetVectors);

            if (rectangle == null)
                return null;

            await Task.Delay(delay);

            if(clone)
                return img.Clone(x => x.Crop(rectangle.Value));

            img.Mutate(x => x.Crop(rectangle.Value));
            return img;
        }

        public static async Task<SixLabors.ImageSharp.Image> DrawTargetAsync(
            SixLabors.ImageSharp.Image img,
            bool clone = false,
            double scale = _defaultScale,
            int thickness = _defaultThickness,
            int delay = 0)
        {
            var targetVectors = await GetTargetVectorsAsync(img, scale: scale, thickness: thickness, delay: delay);

            if (targetVectors.IsNullOrEmpty())
                return null;

            await Task.Delay(delay);

            var result = img;
            if (clone)
                result = img.Clone(x => x.DrawLines(
                 Rgba32.Red, thickness, targetVectors));
            else
                result.Mutate(x => x.DrawLines(
                 Rgba32.Red, thickness, targetVectors));

            result.Convert(JpegFormat.Instance, quality: 10);

            return result;
        }

        public static async Task<SixLabors.Primitives.PointF[]> GetTargetVectorsAsync(
            SixLabors.ImageSharp.Image img,
            double scale = _defaultScale,
            int thickness = _defaultThickness,
            int delay = 0)
        {
            if (img.IsNullOrEmpty())
                return null;

            var minDimention = Math.Min(img.Width, img.Height);
            var cropSize = (int)Math.Floor((double)minDimention * scale);
            var cropSize2 = cropSize / 2;

            if (cropSize2 < (5 + thickness * 2))
                return null;

            await Task.Delay(delay);
            var center = new Vector2(img.Width / 2, img.Height / 2);
            var topLeft = new Vector2(center.X - cropSize2, center.Y - cropSize2);
            var topRight = new Vector2(center.X + cropSize2, center.Y - cropSize2);
            var bottomLeft = new Vector2(center.X - cropSize2, center.Y + cropSize2);
            var bottomRight = new Vector2(center.X + cropSize2, center.Y + cropSize2);

            await Task.Delay(delay);
            return new SixLabors.Primitives.PointF[] {
                    topLeft,
                    topRight,
                    bottomRight,
                    bottomLeft,
                    topLeft,
                    topRight
                  };
        }

        public static SixLabors.Primitives.Rectangle? TryGetTargetRectangle(SixLabors.Primitives.PointF[] vector)
        {
            if (vector.IsNullOrEmpty())
                return null;

            var size = (int)(vector[1].X - vector[0].X);

            return new SixLabors.Primitives.Rectangle((int)vector[0].X, (int)vector[0].Y, size, size);
        }
    }
}
