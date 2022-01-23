using System.Text;
using UOStudio.Client.Engine.Extensions;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class InputLayout : IInputLayout
    {
        private readonly uint _id;
        private readonly IEnumerable<VertexAttribute> _attributes;
        private readonly IList<(int Location, uint VertexBuffer, int VertexStride)> _vertexBuffers;
        private readonly IList<uint> _elementBuffers;
        private readonly Dictionary<uint, uint> _vertexBufferBindings;
        private uint _totalVertexBuffers;
        private bool _bufferBindingsDirty;

        private InputLayout()
        {
            _id = GL.CreateVertexArray();
            _vertexBuffers = new List<(int, uint, int)>();
            _vertexBufferBindings = new Dictionary<uint, uint>();
            _elementBuffers = new List<uint>();
        }

        public InputLayout(IReadOnlyCollection<VertexAttribute> attributes)
            : this()
        {
            _attributes = attributes;

            var label = attributes.Any()
                ? $"VAO_{VertexAttributesToFriendlyName(attributes)}"
                : "VAO";
            GL.ObjectLabel(GL.ObjectIdentifier.VertexArray, _id, label);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_id);
        }

        public void AddVertexBuffer(
            IBuffer vertexBuffer,
            int bindingIndex)
        {
            _vertexBuffers.Add((bindingIndex, (Buffer)vertexBuffer, vertexBuffer.Stride));
            _bufferBindingsDirty = true;
        }

        public void AddElementBuffer(IBuffer elementBuffer)
        {
            _elementBuffers.Add((Buffer)elementBuffer);
        }

        public void Bind()
        {
            if (_bufferBindingsDirty)
            {
                CreateBufferBindings();
            }
            GL.BindVertexArray(_id);
        }

        private static string VertexAttributesToFriendlyName(IEnumerable<VertexAttribute> attributes)
        {
            var map = new Dictionary<string, string>
            {
                { InputLayoutMapper.InputPosition, "Pos" },
                { InputLayoutMapper.InputColor, "Col" },
                { InputLayoutMapper.InputNormal, "Nrm" },
                { InputLayoutMapper.InputUv, "Uv" },
                { InputLayoutMapper.InputUvw, "Uvw" },
                { InputLayoutMapper.InputBinormal, "Bin" },
                { InputLayoutMapper.InputTangent, "Tan" }
            };
            var friendlyName = new StringBuilder();
            foreach (var attribute in attributes)
            {
                if (map.TryGetValue(attribute.Name, out var attributeName))
                {
                    friendlyName.Append(attributeName);
                }
            }

            return friendlyName.ToString();
        }

        private void CreateBufferBindings()
        {
            if (!_vertexBuffers.Any())
            {
                throw new UOStudioEngineException("No vertex buffers bound");
            }

            foreach (var attribute in _attributes)
            {
                foreach (var vertexBuffer in _vertexBuffers)
                {
                    if (!_vertexBufferBindings.TryGetValue(vertexBuffer.VertexBuffer, out var bufferBinding))
                    {
                        bufferBinding = ++_totalVertexBuffers;
                        _vertexBufferBindings.Add(vertexBuffer.VertexBuffer, bufferBinding);
                        GL.VertexArrayVertexBuffer(_id, bufferBinding, vertexBuffer.VertexBuffer, IntPtr.Zero, vertexBuffer.VertexStride);
                    }

                    GL.EnableVertexArrayAttrib(_id, attribute.Index);
                    GL.VertexArrayAttribFormat(_id, attribute.Index, attribute.Components, attribute.Type.ToVertexAttribType(), false, attribute.Offset);
                    GL.VertexArrayAttribBinding(_id, attribute.Index, bufferBinding);
                }
            }

            foreach (var elementBuffer in _elementBuffers)
            {
                GL.VertexArrayElementBuffer(_id, elementBuffer);
            }

            _bufferBindingsDirty = false;
        }
    }
}
