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

namespace AsmodatStandard.Imaging
{
    public class QRScanner
    {
        private QRCodeGenerator _gen;

        public QRScanner()
        {
            _gen = new QRCodeGenerator();
        }


        /// <summary>
        /// Returns PNG QR Code
        /// </summary>
        public Bitmap Create(string text, int pixelsPerModule = 20)
        {
            using (var data = _gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(data))
            using (var bitmap = qrCode.GetGraphic(pixelsPerModule: pixelsPerModule))
               return bitmap.ImageSharpConvert(ImageFormat.Png, quality: 100, compressionLevel: 1);
               //return (Bitmap)bitmap.CloneRGB24Bit().Convert(); // GDI -> not suitale for web
        }

        public SixLabors.ImageSharp.Image CreateImage(string text, int pixelsPerModule = 20)
        {
            using (var data = _gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(data))
            using (var bitmap = qrCode.GetGraphic(pixelsPerModule: pixelsPerModule))
            {
                var img = bitmap.ToImageSharpImage();
                return img.Convert(PngFormat.Instance, quality: 100, compressionLevel: 1);
            }
                
        }

        public string ScanText(Bitmap bmp)
        {
            if (bmp.IsNullOrEmpty())
                return null;

            var fmts = new List<BarcodeFormat>() {
                BarcodeFormat.QR_CODE
            };

            Bitmap bmpFormated = null;
            if (bmp.GetImageFormat() != ImageFormat.Png)
                bmpFormated = bmp.ImageSharpConvert(format: ImageFormat.Png, quality: 100, compressionLevel: 1);
            else
                bmpFormated = bmp;

            var reader = new BarcodeReader() {
                AutoRotate = true,
                TryInverted = true,
                Options = {
                    PossibleFormats = fmts,
                    TryHarder = true,
                    ReturnCodabarStartEnd = true,
                    PureBarcode = false
                }
            };

            var rgb = bmpFormated.GetColorData24Bit();
            var liminance = new RGBLuminanceSource(rgb, bmp.Width, bmp.Height);

            var result = reader.Decode(liminance);
            return result?.Text;
        }

        public string ScanText(SixLabors.ImageSharp.Image img)
        {
            if (img.IsNullOrEmpty())
                return null;

            var fmts = new List<BarcodeFormat>() {
                BarcodeFormat.QR_CODE
            };

            var reader = new BarcodeReader()
            {
                AutoRotate = true,
                TryInverted = true,
                Options = {
                    PossibleFormats = fmts,
                    TryHarder = true,
                    ReturnCodabarStartEnd = true,
                    PureBarcode = false
                }
            };

            var bmp = img.Convert(
                BmpFormat.Instance, 
                quality: 100, 
                compressionLevel: 1,
                pngColorType: PngColorType.Rgb,
                bmpBitsPerPixel: BmpBitsPerPixel.Pixel24);
            var rgb24 = bmp.GetColorData();

            if (rgb24.Length != bmp.Width * bmp.Height * 3)
                throw new Exception($"Only 24Bit images supported, but got {bmp.PixelType.BitsPerPixel}");

            var source = new RGBLuminanceSource(rgb24, bmp.Width, bmp.Height);
            var result = reader.Decode(source);

            return result?.Text;
        }
    }
}
