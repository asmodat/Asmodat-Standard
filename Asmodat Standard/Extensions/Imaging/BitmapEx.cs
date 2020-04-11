using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Extensions.Imaging
{
   
    public static partial class BitmapEx
    {
        public static Bitmap CloneRGB24Bit(this Bitmap bmp)
        {
            if (bmp.IsNullOrEmpty())
                return null;

            return bmp.GetColorData24Bit().To24BitBitmapFromRGB(bmp.Width, bmp.Height);
        }

        public static Bitmap To24BitBitmapFromRGB(this byte[] buffer, int width, int height)
        {
            Bitmap b = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle BoundsRect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            var ptr = bmpData.Scan0;
            var skipByte = bmpData.Stride - width * 3;
            var newBuff = new byte[buffer.Length + skipByte * height];
            for (int j = 0; j < height; j++)
                Buffer.BlockCopy(buffer, j * width * 3, newBuff, j * (width * 3 + skipByte), width * 3);

            Marshal.Copy(newBuff, 0, ptr, newBuff.Length);
            b.UnlockBits(bmpData);
            return b;
        }

        public static byte[] GetColorData24Bit(this Bitmap bmp, bool reverseRGB = false)
        {
            if (bmp == null)
                return null;

            if (bmp.IsNullOrEmpty())
                return new byte[0];

            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            var ptr = data.Scan0;
            var bytes = data.Stride * bmp.Height;
            var results = new byte[bytes];
            Marshal.Copy(ptr, results, 0, bytes);

            if (reverseRGB)
            {
                byte tmp;
                int pos = 0;
                while (pos + 2 < bytes)
                {
                    tmp = results[pos];
                    results[pos] = results[pos + 2];
                    results[pos + 2] = tmp;
                    pos += 3;
                }
            }

            bmp.UnlockBits(data);
            return results;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this Bitmap bmp) => (bmp == null || bmp.Width <= 0 || bmp.Height <= 0);

        public static BitmapData LockBitsBase(this Bitmap bmp, Rectangle rect, ImageLockMode mode)
        {
            if (bmp.IsNullOrEmpty() || !rect.IsValid() || !bmp.Fits(rect))
                return null;

            BitmapData bmd = bmp.LockBits(rect, mode, bmp.PixelFormat);
            return bmd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitmapData LockBitsRW(this Bitmap bmp, Rectangle rect) => bmp.LockBitsBase(rect, ImageLockMode.ReadWrite);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitmapData LockBitsRW(this Bitmap bmp) => bmp.LockBitsRW(bmp.ToRectangle());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitmapData LockBitsR(this Bitmap bmp, Rectangle rect) => bmp.LockBitsBase(rect, ImageLockMode.ReadOnly);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fits(this Bitmap bmp, Rectangle rect) =>
             (bmp != null && bmp.Width > 0 && bmp.Height > 0 &&
              rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0 &&
              (rect.Width + rect.X) <= bmp.Width && (rect.Height + rect.Y) <= bmp.Height);

        public static Color[,] GetPixels(this Bitmap bmp)
        {
            if (bmp == null)
                return null;

            int x = bmp.Width;
            int y = bmp.Height;

            Color[,] pixels = new Color[x, y];

            for (int ix = 0; ix < x; ix++)
                for (int iy = 0; iy < y; iy++)
                    pixels[ix, iy] = bmp.GetPixel(ix, iy);

            return pixels;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] TryGetRawBytes(this Bitmap bmp) 
            => bmp.TryGetRawBytes(bmp.ToRectangle());

        public static byte[] TryGetRawBytes(this Bitmap bmp, Rectangle rect)
        {
            if (bmp.IsNullOrEmpty() || !bmp.Fits(rect))
                return null;

            try
            {
                BitmapData data = bmp.LockBitsR(rect);
                int length = data.Stride * data.Height;
                byte[] output = new byte[length];

                Marshal.Copy(data.Scan0, output, 0, length);
                bmp.UnlockBits(data);
                return output;
            }
            catch
            {
                return null;
            }
        }

        public static void Clear(this Bitmap bmp, Color color)
        {

            if (bmp == null)
                return;

            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(color);
            }
        }

        public static Bitmap Convert(this Bitmap bmp, PixelFormat format)
        {
            if (bmp.IsNullOrEmpty())
                return null;

            Bitmap result = new Bitmap(bmp.Width, bmp.Height, format);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.DrawImage(bmp, bmp.ToRectangle());
            }
            return result;
        }

        public static Bitmap[,] Convert(this Bitmap[,] bitmaps, PixelFormat format)
        {
            int xParts = bitmaps.Width();
            int yParts = bitmaps.Height();

            if (xParts < 1 || yParts < 1)
                return null;

            Bitmap[,] result = new Bitmap[xParts, yParts];
            int x = 0, y;
            for (; x < xParts; x++)
                for (y = 0; y < yParts; y++)
                    result[x, y] = bitmaps[x, y].Convert(format);

            return result;
        }

       /* /// <summary>
        /// TODO: this method migt not have complementary frombytearray method
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Bitmap bmp, ImageFormat format = null)
        {
            if (bmp.IsNullOrEmpty())
                return null;

            if(format == null)
                format = bmp.GetImageFormat();

            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, format);
                return stream.ToArray();
            }
        }*/

        public static void TryWriteText(this Bitmap bmp, string text, decimal fontSizePercentage, string font = "Thoma")
        {
            if (bmp.IsNullOrEmpty() || bmp.Width < 3 || bmp.Height < 3)
                return;

            try
            {
                RectangleF rectf = new RectangleF(1, 1, bmp.Width - 1, bmp.Height - 1);
                Graphics graphics = Graphics.FromImage(bmp);

                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                int fontSize = (int)((fontSizePercentage * bmp.Height) / 100);

                if (fontSize <= 0)
                    fontSize = 1;

                graphics.DrawString(text, new Font(font, fontSize), Brushes.Red, rectf);
                graphics.Flush();
            }
            catch
            {
                return;
            }
        }
    }
}
