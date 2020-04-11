using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using System.Text;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace AsmodatStandard.Extensions.Imaging
{
    public static partial class RectangleEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToRectangle(this Bitmap bmp)
        {
            if (bmp == null)
                return Rectangle.Empty;
            else
                return new Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToRectangle(this Int32Rect rect)
        {
            if (rect == null)
                return Rectangle.Empty;
            else
                return new Rectangle(0, 0, rect.Width, rect.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToRectangle(this WriteableBitmap wbm)
        {
            if (wbm == null)
                return Rectangle.Empty;
            else
                return new Rectangle(0, 0, wbm.PixelWidth, wbm.PixelHeight);
        }*/

        public static Rectangle[,] Split(this Rectangle rect, int xParts, int yParts)
        {
            int width = rect.Width;
            int height = rect.Height;

            if (xParts <= 0 || yParts <= 0 || width < xParts || height < yParts)
                return null;

            Rectangle[,] array = new Rectangle[xParts, yParts];

            int x = 0, y = 0;
            int rW = (width / xParts);
            int rH = (height / yParts);

            //last rectangle dimentions might be diffrent due to not even division
            int rW_last = width - (rW * (xParts - 1));
            int rH_last = height - (rH * (yParts - 1));

            int w, h;
            for (; x < xParts; x++)
            {
                for (y = 0; y < yParts; y++)
                {
                    if (y == yParts - 1)
                        h = rH_last;
                    else
                        h = rH;

                    if (x == xParts - 1)
                        w = rW_last;
                    else
                        w = rW;

                    array[x, y] = new Rectangle(x * rW, y * rH, w, h);
                }
            }

            return array;
        }




        public static readonly Rectangle Default = new Rectangle();

        public static bool IsValid(this Rectangle rect)
        {
            if (rect.Width > 0 && rect.Height > 0 && rect.X >= 0 && rect.Y >= 0)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if one rect1 surround rect2, this method is location (X,Y), and order (rect1,rect2) sensitive
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fits(this Rectangle rect1, Rectangle rect2)
        {
            if ((rect1.Width + rect1.X) >= (rect2.Width + rect2.X) &&
                (rect1.Height + rect1.Y) >= (rect2.Height + rect2.Y) &&
                rect1.X <= rect2.X &&
                rect1.Y <= rect2.Y)
                return true;
            else
                return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualSize(this Rectangle rect1, Rectangle rect2)
        {
            if (rect1.Width == rect2.Width && rect1.Height == rect2.Height)
                return true;
            else
                return false;
        }

        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }
    }
}
