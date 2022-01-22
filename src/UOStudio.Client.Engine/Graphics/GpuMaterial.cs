using System.Runtime.InteropServices;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GpuMaterial
    {
        public Vector4 ColorAmbient;
        public Vector4 ColorDiffuse;
        public Vector4 ColorSpecularAndShininess;
        public Int4 TextureDiffuseAndNormal;
        public Int4 TextureSpecularAndAmbientOcclusion;
        public Int4 TextureRoughnessAndMetalness;
    }
}