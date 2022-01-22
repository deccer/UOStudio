using Serilog;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class ShaderProgramLibrary : IShaderProgramLibrary
    {
        private readonly ILogger _logger;
        private readonly IDictionary<string, IShader> _shaderPrograms;
        private readonly IDictionary<string, bool> _shaderFromFileSystem;
        private readonly IDictionary<string, (DateTime VertexShaderLastWrite, DateTime FragmentShaderLastWrite)> _shaderTimestamps;
        private readonly IDictionary<string, (string VertexShaderFilePath, string FragmentShaderFilePath)> _shaderFileNames;

        public ShaderProgramLibrary(
            ILogger logger)
        {
            _logger = logger;
            _shaderPrograms = new Dictionary<string, IShader>();
            _shaderFromFileSystem = new Dictionary<string, bool>();
            _shaderTimestamps = new Dictionary<string, (DateTime VertexShaderLastWrite, DateTime FragmentShaderLastWrite)>();
            _shaderFileNames = new Dictionary<string, (string VertexShaderFilePath, string FragmentShaderFilePath)>();
        }

        public void Dispose()
        {
            foreach (var shaderProgram in _shaderPrograms)
            {
                shaderProgram.Value.Dispose();
            }
        }

        public void LoadShaders()
        {
            AddShaderProgram("Empty", "Shaders/Empty.vs.glsl", "Shaders/Empty.fs.glsl");
        }

        public IShader GetShaderProgram(string programName)
        {
            return _shaderPrograms.TryGetValue(programName, out var shaderProgram)
                ? shaderProgram
                : _shaderPrograms["Empty"];
            // detect file changes
            // dispose
            // recompile
            // set
        }

        public bool AddShaderProgram(
            string programName,
            string vertexShaderFilePath,
            string fragmentShaderFilePath)
        {
            if (!_shaderPrograms.ContainsKey(programName))
            {
                _logger.Debug(
                    "ShaderLibrary: Adding shader program {@Name} from {@VertexShaderFilePath} and {@FragmentShaderFilePath}",
                    programName,
                    vertexShaderFilePath,
                    fragmentShaderFilePath);
                var shaderResult = Shader.FromFiles(programName, vertexShaderFilePath, fragmentShaderFilePath);
                if (shaderResult.IsSuccess)
                {
                    _shaderPrograms.Add(programName, shaderResult.Value);
                    _shaderTimestamps.Add(programName, (File.GetLastWriteTimeUtc(vertexShaderFilePath), File.GetLastWriteTimeUtc(fragmentShaderFilePath)));
                    _shaderFileNames.Add(programName, (vertexShaderFilePath, fragmentShaderFilePath));
                }
                else
                {
                    _logger.Error("{@Shader} {@Error}", programName, shaderResult.Error);
                    return false;
                }
            }

            return true;
        }

        public void DetectChangedFiles()
        {
            foreach (var shaderTimestamp in _shaderTimestamps)
            {
                var programName = shaderTimestamp.Key;
                var lastWrites = shaderTimestamp.Value;

                if (_shaderFileNames.TryGetValue(programName, out var shaderFileNames))
                {
                    var currentVertexShaderLastWriteTime = File.GetLastWriteTimeUtc(shaderFileNames.VertexShaderFilePath);
                    var currentFragmentShaderLastWriteTime = File.GetLastWriteTimeUtc(shaderFileNames.FragmentShaderFilePath);

                    if (currentVertexShaderLastWriteTime >= lastWrites.VertexShaderLastWrite.ToUniversalTime() ||
                        currentFragmentShaderLastWriteTime >= lastWrites.FragmentShaderLastWrite.ToUniversalTime())
                    {

                        var shaderResult = Shader.FromFiles(programName, shaderFileNames.VertexShaderFilePath,
                            shaderFileNames.FragmentShaderFilePath);
                        if (shaderResult.IsSuccess)
                        {
                            _shaderPrograms[programName]?.Dispose();
                            _shaderPrograms[programName] = shaderResult.Value;
                        }
                        
                        continue;
                    }
                }
            }
        }

        private bool IsShaderFromFileSystem(string programName)
        {
            return _shaderFromFileSystem.TryGetValue(programName, out var isShaderFomFileSystem) && isShaderFomFileSystem;
        }
    }
}