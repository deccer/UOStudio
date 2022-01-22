using Serilog;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class InputLayoutMapper : IInputLayoutMapper
    {
        public const string InputPosition = "i_position";
        public const string InputColor = "i_color";
        public const string InputNormal = "i_normal";
        public const string InputUv = "i_uv";
        public const string InputUvw = "i_uvw";
        public const string InputTangent = "i_tangent";
        public const string InputBinormal = "i_binormal";

        private readonly ILogger _logger;

        public InputLayoutMapper(ILogger logger)
        {
            _logger = logger.ForContext<InputLayoutMapper>();
        }

        public IReadOnlyCollection<VertexAttribute> MapVertexType(VertexType vertexType)
        {
            var attributes = new List<VertexAttribute>();
            switch (vertexType)
            {
                case VertexType.ImGui:
                    /*
            GL.VertexArrayVertexBuffer(_vertexArray, 0, _vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(_vertexArray, _indexBuffer);

            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 0, 2, GL.VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 1, 2, GL.VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(_vertexArray, 2);
            GL.VertexArrayAttribBinding(_vertexArray, 2, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 2, 4, GL.VertexAttribType.UnsignedByte, true, 16);
                     */
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 2, 0));
                    attributes.Add(new VertexAttribute(InputUv, 1, VertexAttributeType.Float, 2, 8));
                    attributes.Add(new VertexAttribute(InputColor, 2, VertexAttributeType.UnsignedByte, 4, 16));
                    break;
                case VertexType.Position:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    break;
                case VertexType.PositionUv:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputUv, 3, VertexAttributeType.Float, 2, 12));
                    break;
                case VertexType.PositionUvw:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputUvw, 3, VertexAttributeType.Float, 3, 12));
                    break;
                case VertexType.PositionColor:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputColor, 1, VertexAttributeType.Float, 3, 12));
                    break;
                case VertexType.PositionColorNormal:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputColor, 1, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 24));
                    break;
                case VertexType.PositionColorNormalUv:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputColor, 1, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 24));
                    attributes.Add(new VertexAttribute(InputUv, 3, VertexAttributeType.Float, 2, 36));
                    break;
                case VertexType.PositionColorNormalUvw:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputColor, 1, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 24));
                    attributes.Add(new VertexAttribute(InputUvw, 3, VertexAttributeType.Float, 3, 36));
                    break;
                case VertexType.PositionColorUv:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputColor, 1, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputUv, 3, VertexAttributeType.Float, 2, 24));
                    break;
                case VertexType.PositionColorUvw:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputColor, 1, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputUvw, 3, VertexAttributeType.Float, 3, 24));
                    break;
                case VertexType.PositionNormal:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 12));
                    break;
                case VertexType.PositionNormalUv:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputUv, 3, VertexAttributeType.Float, 2, 24));
                    break;
                case VertexType.PositionNormalUvw:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputUvw, 3, VertexAttributeType.Float, 3, 24));
                    break;
                case VertexType.PositionNormalUvTangent:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputUv, 3, VertexAttributeType.Float, 2, 24));
                    attributes.Add(new VertexAttribute(InputTangent, 4, VertexAttributeType.Float, 4, 32));
                    break;
                case VertexType.PositionNormalUvwTangent:
                    attributes.Add(new VertexAttribute(InputPosition, 0, VertexAttributeType.Float, 3, 0));
                    attributes.Add(new VertexAttribute(InputNormal, 2, VertexAttributeType.Float, 3, 12));
                    attributes.Add(new VertexAttribute(InputUvw, 3, VertexAttributeType.Float, 3, 24));
                    attributes.Add(new VertexAttribute(InputTangent, 4, VertexAttributeType.Float, 3, 36));
                    break;

                default:
                    _logger.Error("InputLayoutMapper: Unknown VertexType {@VertexType}", vertexType);
                    throw new ArgumentOutOfRangeException(nameof(vertexType), vertexType, null);
            }

            return attributes.ToArray();
        }
    }
}
