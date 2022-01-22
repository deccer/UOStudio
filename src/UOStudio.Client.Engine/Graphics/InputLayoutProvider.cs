using Serilog;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class InputLayoutProvider : IInputLayoutProvider
    {
        private readonly ILogger _logger;
        private readonly IInputLayoutMapper _inputLayoutMapper;
        private readonly IDictionary<VertexType, InputLayout> _inputLayouts;

        public InputLayoutProvider(
            ILogger logger,
            IInputLayoutMapper inputLayoutMapper)
        {
            _logger = logger.ForContext<InputLayoutProvider>();
            _inputLayoutMapper = inputLayoutMapper;
            _inputLayouts = new Dictionary<VertexType, InputLayout>(16);
        }

        public IInputLayout GetInputLayout(VertexType vertexType)
        {
            if (_inputLayouts.TryGetValue(vertexType, out var inputLayout))
            {
                return inputLayout;
            }

            _logger.Debug("InputLayout: Mapping Vertex Attributes against {@VertexType}", vertexType);
            var vertexAttributes = _inputLayoutMapper.MapVertexType(vertexType);
            inputLayout = new InputLayout(vertexAttributes);
            _inputLayouts[vertexType] = inputLayout;
            return inputLayout;
        }

        public void Dispose()
        {
            foreach (var inputLayout in _inputLayouts)
            {
                inputLayout.Value.Dispose();
            }
        }
    }
}
