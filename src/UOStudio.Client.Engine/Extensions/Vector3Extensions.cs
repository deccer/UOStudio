using Assimp;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ToVector3(this Color4D color)
        {
            return new Vector3(color.R, color.G, color.B);
        }

        public static Vector3 ToVector3(this Vector3D vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}