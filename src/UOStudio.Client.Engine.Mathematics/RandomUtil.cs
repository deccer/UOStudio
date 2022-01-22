using System;

namespace UOStudio.Client.Engine.Mathematics
{
    /// <summary>
    /// Random functions on commun types.
    /// </summary>
    public static class RandomUtil
    {
        /// <summary>
        /// Gets random <c>float</c> number within range.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <c>float</c> number.</returns>
        public static float NextFloat(this Random random, float min, float max)
        {
            return MathUtil.Lerp(min, max, (float)random.NextDouble());
        }

        /// <summary>
        /// Gets random <c>double</c> number within range.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <c>double</c> number.</returns>
        public static double NextDouble(this Random random, double min, double max)
        {
            return MathUtil.Lerp(min, max, random.NextDouble());
        }

        /// <summary>
        /// Gets random <c>long</c> number.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <returns>Random <c>long</c> number.</returns>
        public static long NextLong(this Random random)
        {
            var buffer = new byte[sizeof(long)];
            random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Gets random <c>long</c> number within range.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <c>long</c> number.</returns>
        public static long NextLong(this Random random, long min, long max)
        {
            var buf = new byte[sizeof(long)];
            random.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);

            return Math.Abs(longRand % (max - min + 1)) + min;
        }

        /// <summary>
        /// Gets random <see cref="Vector2"/> within range.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <see cref="Vector2"/>.</returns>
        public static Vector2 NextVector2(this Random random, Vector2 min, Vector2 max)
        {
            return new Vector2(random.NextFloat(min.X, max.X), random.NextFloat(min.Y, max.Y));
        }

        /// <summary>
        /// Gets random <see cref="Vector3"/> within range.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <see cref="Vector3"/>.</returns>
        public static Vector3 NextVector3(this Random random, Vector3 min, Vector3 max)
        {
            return new Vector3(random.NextFloat(min.X, max.X), random.NextFloat(min.Y, max.Y), random.NextFloat(min.Z, max.Z));
        }

        /// <summary>
        /// Gets random <see cref="Vector4"/> within range.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <see cref="Vector4"/>.</returns>
        public static Vector4 NextVector4(this Random random, Vector4 min, Vector4 max)
        {
            return new Vector4(random.NextFloat(min.X, max.X), random.NextFloat(min.Y, max.Y), random.NextFloat(min.Z, max.Z), random.NextFloat(min.W, max.W));
        }

        /// <summary>
        /// Gets random opaque <see cref="DXPlayground.Mathematics.Color"/>.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <returns>Random <see cref="DXPlayground.Mathematics.Color"/>.</returns>
        public static Color NextColor(this Random random)
        {
            return new Color(random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), random.NextFloat(0.0f, 1.0f), 1.0f);
        }

        /// <summary>
        /// Gets random opaque <see cref="DXPlayground.Mathematics.Color"/>.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="minBrightness">Minimum brightness.</param>
        /// <param name="maxBrightness">Maximum brightness</param>
        /// <returns>Random <see cref="DXPlayground.Mathematics.Color"/>.</returns>
        public static Color NextColor(this Random random, float minBrightness, float maxBrightness)
        {
            return new Color(random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minBrightness, maxBrightness), 1.0f);
        }

        /// <summary>
        /// Gets random <see cref="DXPlayground.Mathematics.Color"/>.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="minBrightness">Minimum brightness.</param>
        /// <param name="maxBrightness">Maximum brightness</param>
        /// <param name="alpha">Alpha value.</param>
        /// <returns>Random <see cref="DXPlayground.Mathematics.Color"/>.</returns>
        public static Color NextColor(this Random random, float minBrightness, float maxBrightness, float alpha)
        {
            return new Color(random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minBrightness, maxBrightness), alpha);
        }

        /// <summary>
        /// Gets random <see cref="DXPlayground.Mathematics.Color"/>.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="minBrightness">Minimum brightness.</param>
        /// <param name="maxBrightness">Maximum brightness</param>
        /// <param name="minAlpha">Minimum alpha.</param>
        /// <param name="maxAlpha">Maximum alpha.</param>
        /// <returns>Random <see cref="DXPlayground.Mathematics.Color"/>.</returns>
        public static Color NextColor(this Random random, float minBrightness, float maxBrightness, float minAlpha, float maxAlpha)
        {
            return new Color(random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minBrightness, maxBrightness), random.NextFloat(minAlpha, maxAlpha));
        }

        /// <summary>
        /// Gets random <see cref="Point"/>.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <see cref="Point"/>.</returns>
        public static Point NextPoint(this Random random, Point min, Point max)
        {
            return new Point(random.Next(min.X, max.X), random.Next(min.Y, max.Y));
        }

        /// <summary>
        /// Gets random <see cref="System.TimeSpan"/>.
        /// </summary>
        /// <param name="random">Current <see cref="System.Random"/>.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Random <see cref="System.TimeSpan"/>.</returns>
        public static TimeSpan NextTime(this Random random, TimeSpan min, TimeSpan max)
        {
            return TimeSpan.FromTicks(random.NextLong(min.Ticks, max.Ticks));
        }
    }
}
