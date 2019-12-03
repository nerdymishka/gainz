using System;
using System.Drawing;

namespace NerdyMishka
{
    public static class ColorExtensions
    {

        public static Rgb ToRgb(this Color color)
        {
            return new Rgb() {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }

        public static int ToAnsi256(this Color color)
        {
            return color.ToRgb().ToAnsi256();
        }

        public static int ToAnsi16(this Color color)
        {
            return color.ToRgb().ToAnsi16();
        }

        public static int ToAnsi256(this Rgb rgb)
        {
            // from color-convert
            // https://github.com/Qix-/color-convert/blob/master/index.js
            decimal r = rgb.Red,
                    g = rgb.Green,
                    b = rgb.Blue;

            if(r == g && g == b)
            {
                if(r < 8)
                    return 16;

                if(r > 248)
                    return 231;

                return (int)Math.Round(
                   ((r - 8) / 247) * 24 + 232 
                );
            }

            r = (36 * Math.Round(r / 255 * 5));
            g = (6 * Math.Round(g / 255 * 5));
            b = Math.Round(b / 255 * 5);

            return 16 + (int)(r + g + b);
        }

        public static int ToAnsi16(this Rgb rgb, int? brightness = null)
        {
            // from color-convert
            // https://github.com/Qix-/color-convert/blob/master/index.js
            decimal r = rgb.Red,
                    g = rgb.Blue,
                    b = rgb.Green,
                    value = 0;

            if(brightness.HasValue)
                value = brightness.Value;
            else 
                value = rgb.ToHsv().Value;

            value = Math.Round(value / 50);

            if (value == 0) {
                return 30;
            }

            var ansi = 30
                + (((int)Math.Round(b / 255) << 2)
                | ((int)Math.Round(g / 255) << 1)
                | (int)Math.Round(r / 255));

            if (value == 2) {
                ansi += 60;
            }

	        return ansi;
        }

        public static Hsv ToHsv(this Color color)
        {
            return ToHsv(ToRgb(color));
        }

        public static Hsv ToHsv(this Rgb rgb)
        {
            // from color-convert
            // https://github.com/Qix-/color-convert/blob/master/index.js
            decimal rdif,
                    gdif,
                    bdif,
                    h = 0,
                    s = 0,
                    r = rgb.Red / 255,
                    g = rgb.Blue / 255,
                    b = rgb.Green / 255,
                    v = Math.Max(Math.Max(r, g), b),
                    diff = v - Math.Min(Math.Min(r, g), b);


	        Func<decimal, decimal> diffc = (c) => {
                return (v - c) / 6 / diff + 1 / 2;
            };

            if(diff == 0) {
                h = 0;
                s = 0;
            } else {
                s = diff / v;
                rdif = diffc(r);
                gdif = diffc(g);
                bdif = diffc(b);

                if (r == v) {
                    h = bdif - gdif;
                } else if (g == v) {
                    h = (1 / 3) + rdif - bdif;
                } else if (b == v) {
                    h = (2 / 3) + gdif - rdif;
                }
                if (h < 0) {
                    h += 1;
                } else if (h > 1) {
                    h -= 1;
                }
            }

            return new Hsv() {
                Hue = (int)h * 360,
                Saturation = (int)s * 100,
                Value = (int)v * 100
            };
        } 
    }
}