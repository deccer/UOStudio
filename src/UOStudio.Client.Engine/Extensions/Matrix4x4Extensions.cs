using Assimp;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Extensions
{
    public static class Matrix4x4Extensions
    {
        public static Matrix ToMatrix(this Matrix4x4 matrix)
        {
            return new Matrix
            {
                Column1 = new Vector4(matrix.A1, matrix.B1, matrix.C1, matrix.D1),
                Column2 = new Vector4(matrix.A2, matrix.B2, matrix.C2, matrix.D2),
                Column3 = new Vector4(matrix.A3, matrix.B3, matrix.C3, matrix.D3),
                Column4 = new Vector4(matrix.A4, matrix.B4, matrix.C4, matrix.D4),
            };
        }
    }
}