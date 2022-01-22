using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Mathematics
{
    /// <summary>
    /// Represents a 32-bit color (4 bytes) in the form of BGRA (in byte order: B, G, B, A).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public partial struct ColorBgra : IEquatable<ColorBgra>, IFormattable
    {
        private const string toStringFormat = "A:{0} R:{1} G:{2} B:{3}";

        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public byte B;

        /// <summary>
        /// The green component of the color.
        /// </summary>
        public byte G;

        /// <summary>
        /// The red component of the color.
        /// </summary>
        public byte R;

        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        public byte A;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public ColorBgra(byte value)
        {
            A = R = G = B = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public ColorBgra(float value)
        {
            A = R = G = B = ToByte(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public ColorBgra(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public ColorBgra(float red, float green, float blue, float alpha)
        {
            R = ToByte(red);
            G = ToByte(green);
            B = ToByte(blue);
            A = ToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="value">The red, green, blue, and alpha components of the color.</param>
        public ColorBgra(Vector4 value)
        {
            R = ToByte(value.X);
            G = ToByte(value.Y);
            B = ToByte(value.Z);
            A = ToByte(value.W);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="value">The red, green, and blue components of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public ColorBgra(Vector3 value, float alpha)
        {
            R = ToByte(value.X);
            G = ToByte(value.Y);
            B = ToByte(value.Z);
            A = ToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="bgra">A packed integer containing all four color components in BGRA order.</param>
        public ColorBgra(uint bgra)
        {
            A = (byte)((bgra >> 24) & 255);
            R = (byte)((bgra >> 16) & 255);
            G = (byte)((bgra >> 8) & 255);
            B = (byte)(bgra & 255);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="bgra">A packed integer containing all four color components in BGRA.</param>
        public ColorBgra(int bgra)
        {
            A = (byte)((bgra >> 24) & 255);
            R = (byte)((bgra >> 16) & 255);
            G = (byte)((bgra >> 8) & 255);
            B = (byte)(bgra & 255);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the red, green, and blue, alpha components of the color. This must be an array with four elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public ColorBgra(float[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(values), "There must be four and only four input values for ColorBGRA.");

            B = ToByte(values[0]);
            G = ToByte(values[1]);
            R = ToByte(values[2]);
            A = ToByte(values[3]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBgra"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the red, green, and blue, alpha components of the color. This must be an array with four elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public ColorBgra(byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(values), "There must be four and only four input values for ColorBGRA.");

            B = values[0];
            G = values[1];
            R = values[2];
            A = values[3];
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the alpha, red, green, or blue component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the alpha component, 1 for the red component, 2 for the green component, and 3 for the blue component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return B;
                    case 1: return G;
                    case 2: return R;
                    case 3: return A;
                }

                throw new ArgumentOutOfRangeException(nameof(index), "Indices for ColorBGRA run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: B = value; break;
                    case 1: G = value; break;
                    case 2: R = value; break;
                    case 3: A = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for ColorBGRA run from 0 to 3, inclusive.");
                }
            }
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public int ToBgra()
        {
            int value = B;
            value |= G << 8;
            value |= R << 16;
            value |= A << 24;

            return value;
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public int ToRgba()
        {
            int value = R;
            value |= G << 8;
            value |= B << 16;
            value |= A << 24;

            return value;
        }

        /// <summary>
        /// Converts the color into a three component vector.
        /// </summary>
        /// <returns>A three component vector containing the red, green, and blue components of the color.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        /// <summary>
        /// Converts the color into a three component color.
        /// </summary>
        /// <returns>A three component color containing the red, green, and blue components of the color.</returns>
        public Color3 ToColor3()
        {
            return new Color3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        /// <summary>
        /// Converts the color into a four component vector.
        /// </summary>
        /// <returns>A four component vector containing all four color components.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }

        /// <summary>
        /// Creates an array containing the elements of the color.
        /// </summary>
        /// <returns>A four-element array containing the components of the color in BGRA order.</returns>
        public byte[] ToArray()
        {
            return new[] { B, G, R, A };
        }

        /// <summary>
        /// Gets the brightness.
        /// </summary>
        /// <returns>The Hue-Saturation-Brightness (HSB) saturation for this <see cref="DXPlayground.Mathematics.Color"/></returns>
        public float GetBrightness()
        {
            var r = R / 255.0f;
            var g = G / 255.0f;
            var b = B / 255.0f;

            var max = r;
            var min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            return (max + min) / 2;
        }

        /// <summary>
        /// Gets the hue.
        /// </summary>
        /// <returns>The Hue-Saturation-Brightness (HSB) saturation for this <see cref="DXPlayground.Mathematics.Color"/></returns>
        public float GetHue()
        {
            if (R == G && G == B)
                return 0; // 0 makes as good an UNDEFINED value as any

            var r = R / 255.0f;
            var g = G / 255.0f;
            var b = B / 255.0f;

            var hue = 0.0f;

            var max = r;
            var min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            var delta = max - min;

            if (r == max)
            {
                hue = (g - b) / delta;
            }
            else if (g == max)
            {
                hue = 2 + (b - r) / delta;
            }
            else if (b == max)
            {
                hue = 4 + (r - g) / delta;
            }
            hue *= 60;

            if (hue < 0.0f)
            {
                hue += 360.0f;
            }
            return hue;
        }

        /// <summary>
        /// Gets the saturation.
        /// </summary>
        /// <returns>The Hue-Saturation-Brightness (HSB) saturation for this <see cref="DXPlayground.Mathematics.Color"/></returns>
        public float GetSaturation()
        {
            var r = R / 255.0f;
            var g = G / 255.0f;
            var b = B / 255.0f;

            float s = 0;

            var max = r;
            var min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            // if max == min, then there is no color and
            // the saturation is zero.
            if (max != min)
            {
                var l = (max + min) / 2;

                if (l <= .5)
                {
                    s = (max - min) / (max + min);
                }
                else
                {
                    s = (max - min) / (2 - max - min);
                }
            }
            return s;
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in BGRA order</param>
        /// <returns>A color.</returns>
        public static ColorBgra FromBgra(int color)
        {
            return new ColorBgra(color);
        }

        /// <summary>
        /// Converts the color from a packed BGRA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in BGRA order</param>
        /// <returns>A color.</returns>
        public static ColorBgra FromBgra(uint color)
        {
            return new ColorBgra(color);
        }

        /// <summary>
        /// Converts the color from a packed RGBA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in RGBA order</param>
        /// <returns>A color.</returns>
        public static ColorBgra FromRgba(int color)
        {
            return new ColorBgra((byte)(color & 255), (byte)((color >> 8) & 255), (byte)((color >> 16) & 255), (byte)((color >> 24) & 255));
        }

        /// <summary>
        /// Converts the color from a packed RGBA integer.
        /// </summary>
        /// <param name="color">A packed integer containing all four color components in RGBA order</param>
        /// <returns>A color.</returns>
        public static ColorBgra FromRgba(uint color)
        {
            return FromRgba(unchecked((int)color));
        }

        /// <summary>
        /// Adds two colors.
        /// </summary>
        /// <param name="left">The first color to add.</param>
        /// <param name="right">The second color to add.</param>
        /// <param name="result">When the method completes, completes the sum of the two colors.</param>
        public static void Add(ref ColorBgra left, ref ColorBgra right, out ColorBgra result)
        {
            result.A = (byte)(left.A + right.A);
            result.R = (byte)(left.R + right.R);
            result.G = (byte)(left.G + right.G);
            result.B = (byte)(left.B + right.B);
        }

        /// <summary>
        /// Adds two colors.
        /// </summary>
        /// <param name="left">The first color to add.</param>
        /// <param name="right">The second color to add.</param>
        /// <returns>The sum of the two colors.</returns>
        public static ColorBgra Add(ColorBgra left, ColorBgra right)
        {
            return new ColorBgra(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);
        }

        /// <summary>
        /// Subtracts two colors.
        /// </summary>
        /// <param name="left">The first color to subtract.</param>
        /// <param name="right">The second color to subtract.</param>
        /// <param name="result">WHen the method completes, contains the difference of the two colors.</param>
        public static void Subtract(ref ColorBgra left, ref ColorBgra right, out ColorBgra result)
        {
            result.A = (byte)(left.A - right.A);
            result.R = (byte)(left.R - right.R);
            result.G = (byte)(left.G - right.G);
            result.B = (byte)(left.B - right.B);
        }

        /// <summary>
        /// Subtracts two colors.
        /// </summary>
        /// <param name="left">The first color to subtract.</param>
        /// <param name="right">The second color to subtract</param>
        /// <returns>The difference of the two colors.</returns>
        public static ColorBgra Subtract(ColorBgra left, ColorBgra right)
        {
            return new ColorBgra(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param>
        /// <param name="right">The second color to modulate.</param>
        /// <param name="result">When the method completes, contains the modulated color.</param>
        public static void Modulate(ref ColorBgra left, ref ColorBgra right, out ColorBgra result)
        {
            result.A = (byte)(left.A * right.A / 255.0f);
            result.R = (byte)(left.R * right.R / 255.0f);
            result.G = (byte)(left.G * right.G / 255.0f);
            result.B = (byte)(left.B * right.B / 255.0f);
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param>
        /// <param name="right">The second color to modulate.</param>
        /// <returns>The modulated color.</returns>
        public static ColorBgra Modulate(ColorBgra left, ColorBgra right)
        {
            return new ColorBgra((left.R * right.R) >> 8, (left.G * right.G) >> 8, (left.B * right.B) >> 8, (left.A * right.A) >> 8);
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The color to scale.</param>
        /// <param name="scale">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled color.</param>
        public static void Scale(ref ColorBgra value, float scale, out ColorBgra result)
        {
            result.A = (byte)(value.A * scale);
            result.R = (byte)(value.R * scale);
            result.G = (byte)(value.G * scale);
            result.B = (byte)(value.B * scale);
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The color to scale.</param>
        /// <param name="scale">The amount by which to scale.</param>
        /// <returns>The scaled color.</returns>
        public static ColorBgra Scale(ColorBgra value, float scale)
        {
            return new ColorBgra((byte)(value.R * scale), (byte)(value.G * scale), (byte)(value.B * scale), (byte)(value.A * scale));
        }

        /// <summary>
        /// Negates a color.
        /// </summary>
        /// <param name="value">The color to negate.</param>
        /// <param name="result">When the method completes, contains the negated color.</param>
        public static void Negate(ref ColorBgra value, out ColorBgra result)
        {
            result.A = (byte)(255 - value.A);
            result.R = (byte)(255 - value.R);
            result.G = (byte)(255 - value.G);
            result.B = (byte)(255 - value.B);
        }

        /// <summary>
        /// Negates a color.
        /// </summary>
        /// <param name="value">The color to negate.</param>
        /// <returns>The negated color.</returns>
        public static ColorBgra Negate(ColorBgra value)
        {
            return new ColorBgra(255 - value.R, 255 - value.G, 255 - value.B, 255 - value.A);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="result">When the method completes, contains the clamped value.</param>
        public static void Clamp(ref ColorBgra value, ref ColorBgra min, ref ColorBgra max, out ColorBgra result)
        {
            var alpha = value.A;
            alpha = alpha > max.A ? max.A : alpha;
            alpha = alpha < min.A ? min.A : alpha;

            var red = value.R;
            red = red > max.R ? max.R : red;
            red = red < min.R ? min.R : red;

            var green = value.G;
            green = green > max.G ? max.G : green;
            green = green < min.G ? min.G : green;

            var blue = value.B;
            blue = blue > max.B ? max.B : blue;
            blue = blue < min.B ? min.B : blue;

            result = new ColorBgra(red, green, blue, alpha);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static ColorBgra Clamp(ColorBgra value, ColorBgra min, ColorBgra max)
        {
            Clamp(ref value, ref min, ref max, out var result);
            return result;
        }

        /// <summary>
        /// Performs a linear interpolation between two colors.
        /// </summary>
        /// <param name="start">Start color.</param>
        /// <param name="end">End color.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two colors.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
        /// </remarks>
        public static void Lerp(ref ColorBgra start, ref ColorBgra end, float amount, out ColorBgra result)
        {
            result.B = MathUtil.Lerp(start.B, end.B, amount);
            result.G = MathUtil.Lerp(start.G, end.G, amount);
            result.R = MathUtil.Lerp(start.R, end.R, amount);
            result.A = MathUtil.Lerp(start.A, end.A, amount);
        }

        /// <summary>
        /// Performs a linear interpolation between two colors.
        /// </summary>
        /// <param name="start">Start color.</param>
        /// <param name="end">End color.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The linear interpolation of the two colors.</returns>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned.
        /// </remarks>
        public static ColorBgra Lerp(ColorBgra start, ColorBgra end, float amount)
        {
            Lerp(ref start, ref end, amount, out var result);
            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation between two colors.
        /// </summary>
        /// <param name="start">Start color.</param>
        /// <param name="end">End color.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two colors.</param>
        public static void SmoothStep(ref ColorBgra start, ref ColorBgra end, float amount, out ColorBgra result)
        {
            amount = MathUtil.SmoothStep(amount);
            Lerp(ref start, ref end, amount, out result);
        }

        /// <summary>
        /// Performs a cubic interpolation between two colors.
        /// </summary>
        /// <param name="start">Start color.</param>
        /// <param name="end">End color.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two colors.</returns>
        public static ColorBgra SmoothStep(ColorBgra start, ColorBgra end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out var result);
            return result;
        }

        /// <summary>
        /// Returns a color containing the smallest components of the specified colorss.
        /// </summary>
        /// <param name="left">The first source color.</param>
        /// <param name="right">The second source color.</param>
        /// <param name="result">When the method completes, contains an new color composed of the largest components of the source colorss.</param>
        public static void Max(ref ColorBgra left, ref ColorBgra right, out ColorBgra result)
        {
            result.A = left.A > right.A ? left.A : right.A;
            result.R = left.R > right.R ? left.R : right.R;
            result.G = left.G > right.G ? left.G : right.G;
            result.B = left.B > right.B ? left.B : right.B;
        }

        /// <summary>
        /// Returns a color containing the largest components of the specified colorss.
        /// </summary>
        /// <param name="left">The first source color.</param>
        /// <param name="right">The second source color.</param>
        /// <returns>A color containing the largest components of the source colors.</returns>
        public static ColorBgra Max(ColorBgra left, ColorBgra right)
        {
            Max(ref left, ref right, out var result);
            return result;
        }

        /// <summary>
        /// Returns a color containing the smallest components of the specified colors.
        /// </summary>
        /// <param name="left">The first source color.</param>
        /// <param name="right">The second source color.</param>
        /// <param name="result">When the method completes, contains an new color composed of the smallest components of the source colors.</param>
        public static void Min(ref ColorBgra left, ref ColorBgra right, out ColorBgra result)
        {
            result.A = left.A < right.A ? left.A : right.A;
            result.R = left.R < right.R ? left.R : right.R;
            result.G = left.G < right.G ? left.G : right.G;
            result.B = left.B < right.B ? left.B : right.B;
        }

        /// <summary>
        /// Returns a color containing the smallest components of the specified colors.
        /// </summary>
        /// <param name="left">The first source color.</param>
        /// <param name="right">The second source color.</param>
        /// <returns>A color containing the smallest components of the source colors.</returns>
        public static ColorBgra Min(ColorBgra left, ColorBgra right)
        {
            Min(ref left, ref right, out var result);
            return result;
        }

        /// <summary>
        /// Adjusts the contrast of a color.
        /// </summary>
        /// <param name="value">The color whose contrast is to be adjusted.</param>
        /// <param name="contrast">The amount by which to adjust the contrast.</param>
        /// <param name="result">When the method completes, contains the adjusted color.</param>
        public static void AdjustContrast(ref ColorBgra value, float contrast, out ColorBgra result)
        {
            result.A = value.A;
            result.R = ToByte(0.5f + contrast * (value.R / 255.0f - 0.5f));
            result.G = ToByte(0.5f + contrast * (value.G / 255.0f - 0.5f));
            result.B = ToByte(0.5f + contrast * (value.B / 255.0f - 0.5f));
        }

        /// <summary>
        /// Adjusts the contrast of a color.
        /// </summary>
        /// <param name="value">The color whose contrast is to be adjusted.</param>
        /// <param name="contrast">The amount by which to adjust the contrast.</param>
        /// <returns>The adjusted color.</returns>
        public static ColorBgra AdjustContrast(ColorBgra value, float contrast)
        {
            return new ColorBgra(
                ToByte(0.5f + contrast * (value.R / 255.0f - 0.5f)),
                ToByte(0.5f + contrast * (value.G / 255.0f - 0.5f)),
                ToByte(0.5f + contrast * (value.B / 255.0f - 0.5f)),
                value.A);
        }

        /// <summary>
        /// Adjusts the saturation of a color.
        /// </summary>
        /// <param name="value">The color whose saturation is to be adjusted.</param>
        /// <param name="saturation">The amount by which to adjust the saturation.</param>
        /// <param name="result">When the method completes, contains the adjusted color.</param>
        public static void AdjustSaturation(ref ColorBgra value, float saturation, out ColorBgra result)
        {
            var grey = value.R / 255.0f * 0.2125f + value.G / 255.0f * 0.7154f + value.B / 255.0f * 0.0721f;

            result.A = value.A;
            result.R = ToByte(grey + saturation * (value.R / 255.0f - grey));
            result.G = ToByte(grey + saturation * (value.G / 255.0f - grey));
            result.B = ToByte(grey + saturation * (value.B / 255.0f - grey));
        }

        /// <summary>
        /// Adjusts the saturation of a color.
        /// </summary>
        /// <param name="value">The color whose saturation is to be adjusted.</param>
        /// <param name="saturation">The amount by which to adjust the saturation.</param>
        /// <returns>The adjusted color.</returns>
        public static ColorBgra AdjustSaturation(ColorBgra value, float saturation)
        {
            var grey = value.R / 255.0f * 0.2125f + value.G / 255.0f * 0.7154f + value.B / 255.0f * 0.0721f;

            return new ColorBgra(
                ToByte(grey + saturation * (value.R / 255.0f - grey)),
                ToByte(grey + saturation * (value.G / 255.0f - grey)),
                ToByte(grey + saturation * (value.B / 255.0f - grey)),
                value.A);
        }

        /// <summary>
        /// Computes the premultiplied value of the provided color.
        /// </summary>
        /// <param name="value">The non-premultiplied value.</param>
        /// <param name="result">The premultiplied result.</param>
        public static void Premultiply(ref ColorBgra value, out ColorBgra result)
        {
            var a = value.A / (255f * 255f);
            result.A = value.A;
            result.R = ToByte(value.R * a);
            result.G = ToByte(value.G * a);
            result.B = ToByte(value.B * a);
        }

        /// <summary>
        /// Computes the premultiplied value of the provided color.
        /// </summary>
        /// <param name="value">The non-premultiplied value.</param>
        /// <returns>The premultiplied result.</returns>
        public static ColorBgra Premultiply(ColorBgra value)
        {
            Premultiply(ref value, out var result);
            return result;
        }

        /// <summary>
        /// Adds two colors.
        /// </summary>
        /// <param name="left">The first color to add.</param>
        /// <param name="right">The second color to add.</param>
        /// <returns>The sum of the two colors.</returns>
        public static ColorBgra operator +(ColorBgra left, ColorBgra right)
        {
            return new ColorBgra(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);
        }

        /// <summary>
        /// Assert a color (return it unchanged).
        /// </summary>
        /// <param name="value">The color to assert (unchange).</param>
        /// <returns>The asserted (unchanged) color.</returns>
        public static ColorBgra operator +(ColorBgra value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two colors.
        /// </summary>
        /// <param name="left">The first color to subtract.</param>
        /// <param name="right">The second color to subtract.</param>
        /// <returns>The difference of the two colors.</returns>
        public static ColorBgra operator -(ColorBgra left, ColorBgra right)
        {
            return new ColorBgra(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);
        }

        /// <summary>
        /// Negates a color.
        /// </summary>
        /// <param name="value">The color to negate.</param>
        /// <returns>A negated color.</returns>
        public static ColorBgra operator -(ColorBgra value)
        {
            return new ColorBgra(-value.R, -value.G, -value.B, -value.A);
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="scale">The factor by which to scale the color.</param>
        /// <param name="value">The color to scale.</param>
        /// <returns>The scaled color.</returns>
        public static ColorBgra operator *(float scale, ColorBgra value)
        {
            return new ColorBgra((byte)(value.R * scale), (byte)(value.G * scale), (byte)(value.B * scale), (byte)(value.A * scale));
        }

        /// <summary>
        /// Scales a color.
        /// </summary>
        /// <param name="value">The factor by which to scale the color.</param>
        /// <param name="scale">The color to scale.</param>
        /// <returns>The scaled color.</returns>
        public static ColorBgra operator *(ColorBgra value, float scale)
        {
            return new ColorBgra((byte)(value.R * scale), (byte)(value.G * scale), (byte)(value.B * scale), (byte)(value.A * scale));
        }

        /// <summary>
        /// Modulates two colors.
        /// </summary>
        /// <param name="left">The first color to modulate.</param>
        /// <param name="right">The second color to modulate.</param>
        /// <returns>The modulated color.</returns>
        public static ColorBgra operator *(ColorBgra left, ColorBgra right)
        {
            return new ColorBgra((byte)(left.R * right.R / 255.0f), (byte)(left.G * right.G / 255.0f), (byte)(left.B * right.B / 255.0f), (byte)(left.A * right.A / 255.0f));
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ColorBgra left, ColorBgra right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ColorBgra left, ColorBgra right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ColorBgra"/> to <see cref="Color3"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color3(ColorBgra value)
        {
            return new Color3(value.R, value.G, value.B);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ColorBgra"/> to <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3(ColorBgra value)
        {
            return new Vector3(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ColorBgra"/> to <see cref="Vector4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector4(ColorBgra value)
        {
            return new Vector4(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ColorBgra"/> to <see cref="Color4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color4(ColorBgra value)
        {
            return new Color4(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3"/> to <see cref="ColorBgra"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator ColorBgra(Vector3 value)
        {
            return new ColorBgra(value.X / 255.0f, value.Y / 255.0f, value.Z / 255.0f, 1.0f);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Color3"/> to <see cref="ColorBgra"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator ColorBgra(Color3 value)
        {
            return new ColorBgra(value.Red, value.Green, value.Blue, 1.0f);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector4"/> to <see cref="ColorBgra"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator ColorBgra(Vector4 value)
        {
            return new ColorBgra(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Color4"/> to <see cref="ColorBgra"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator ColorBgra(Color4 value)
        {
            return new ColorBgra(value.Red, value.Green, value.Blue, value.Alpha);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DXPlayground.Mathematics.Color"/> to <see cref="ColorBgra"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ColorBgra(Color value)
        {
            return new ColorBgra(value.R, value.G, value.B, value.A);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ColorBgra"/> to <see cref="DXPlayground.Mathematics.Color"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color(ColorBgra value)
        {
            return new Color(value.R, value.G, value.B, value.A);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ColorBgra"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator int(ColorBgra value)
        {
            return value.ToBgra();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Int32"/> to <see cref="ColorBgra"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator ColorBgra(int value)
        {
            return new ColorBgra(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format to apply to each channel (byte).</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, toStringFormat, A, R, G, B);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format to apply to each channel (byte).</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider,
                                 toStringFormat,
                                 A.ToString(format, formatProvider),
                                 R.ToString(format, formatProvider),
                                 G.ToString(format, formatProvider),
                                 B.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = B.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ R.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ColorBgra"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ColorBgra"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="ColorBgra"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref ColorBgra other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ColorBgra"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ColorBgra"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="ColorBgra"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ColorBgra other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is ColorBgra))
                return false;

            var strongValue = (ColorBgra)value;
            return Equals(ref strongValue);
        }

        private static byte ToByte(float component)
        {
            var value = (int)(component * 255.0f);
            return (byte)(value < 0 ? 0 : value > 255 ? 255 : value);
        }
    }
}
