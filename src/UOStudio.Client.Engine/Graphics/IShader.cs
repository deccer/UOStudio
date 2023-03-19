using UOStudio.Client.Engine.Mathematics;
using Point = UOStudio.Client.Engine.Mathematics.Point;

namespace UOStudio.Client.Engine.Graphics
{
    public interface IShader : IDisposable
    {
        void Bind();

        void SetUniformBuffer(string uniformBlockName, IBuffer uniformBuffer);

        void SetStorageBuffer(string uniformBlockName, IBuffer storageBuffer);

        void SetVertexUniform(string uniformName, int value);

        void SetVertexUniform(string uniformName, float value);

        void SetVertexUniform(string uniformName, Vector2 value);

        void SetVertexUniform(string uniformName, Vector3 value);

        void SetVertexUniform(string uniformName, Vector4 value);

        void SetVertexUniform(string uniformName, Matrix value);

        void SetFragmentUniform(string uniformName, int value);

        void SetFragmentUniform(string uniformName, Point value);

        void SetFragmentUniform(string uniformName, float value);

        void SetFragmentUniform(string uniformName, Vector2 value);

        void SetFragmentUniform(string uniformName, Vector3 value);

        void SetFragmentUniform(string uniformName, Vector4 value);

        void SetFragmentUniform(string uniformName, Matrix value);
    }
}
