namespace UOStudio.Client.Engine
{
    public static class MathHelper
    {
        public const float PiOver2 = (float)Math.PI / 2.0f;
        public const float PiOver4 = (float)Math.PI / 2.0f;

        public static float ToRadians(float degrees)
        {
            return degrees * MathF.PI / 180f;
        }
    }
}
