using CSharpFunctionalExtensions;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class Shader : IDisposable, IShader
    {
        private readonly uint _programId;
        private readonly uint _vertexShader;
        private readonly uint _fragmentShader;
        private readonly IDictionary<string, int> _uniformLocations;
        private readonly IDictionary<string, uint> _uniformBlockBindings;
        private readonly IDictionary<string, uint> _uniformBufferBindings;
        
        private Shader(string label)
        {
            _programId = GL.CreateProgramPipeline();
            GL.ObjectLabel(GL.ObjectIdentifier.Program, _programId, label);

            _uniformLocations = new Dictionary<string, int>();
            _uniformBlockBindings = new Dictionary<string, uint>();
            _uniformBufferBindings = new Dictionary<string, uint>();
        }
        
        private Shader(
            string label,
            uint vertexShader,
            uint fragmentShader)
            : this(label)
        {
            var vertexShaderLabel = $"S_{GL.ShaderType.VertexShader}_{label}";
            GL.ObjectLabel(GL.ObjectIdentifier.Program, _programId, vertexShaderLabel);

            var fragmentShaderLabel = $"S_{GL.ShaderType.FragmentShader}_{label}";
            GL.ObjectLabel(GL.ObjectIdentifier.Program, _programId, fragmentShaderLabel);

            _vertexShader = vertexShader;
            _fragmentShader = fragmentShader;
        }

        public static Result<IShader> FromFiles(
            string label,
            string vertexShaderFileName,
            string fragmentShaderFileName)
        {
            var vertexShader = CreateShaderProgramFromFile(GL.ShaderType.VertexShader, vertexShaderFileName);
            var fragmentShader = CreateShaderProgramFromFile(GL.ShaderType.FragmentShader, fragmentShaderFileName);

            var program = new Shader(label, vertexShader, fragmentShader);
            var validationResult = program.Validate();

            return validationResult.IsSuccess
                ? program
                : Result.Failure<IShader>(validationResult.Error);
        }

        public static Result<IShader> FromSources(
            string label,
            string vertexShaderSource,
            string fragmentShaderSource)
        {
            var vertexShader = CreateShaderProgramFromSource(GL.ShaderType.VertexShader, vertexShaderSource);
            var fragmentShader = CreateShaderProgramFromSource(GL.ShaderType.FragmentShader, fragmentShaderSource);

            var program = new Shader(label, vertexShader, fragmentShader);
            var validationResult = program.Validate();

            return validationResult.IsSuccess
                ? program
                : Result.Failure<IShader>(validationResult.Error);
        }

        public void Bind()
        {
            GL.BindProgramPipeline(_programId);
        }

        public void Dispose()
        {
            GL.DeleteProgram(_vertexShader);
            GL.DeleteProgram(_fragmentShader);
            GL.DeleteProgramPipeline(_programId);
        }

        public void SetUniformBuffer(string uniformBlockName, IBuffer uniformBuffer)
        {
            if (_uniformBlockBindings.TryGetValue(uniformBlockName, out var uniformBlockBinding))
            {
                GL.BindBufferBase(GL.BufferTargetARB.UniformBuffer, uniformBlockBinding, (Buffer)uniformBuffer);
            }
        }
        
        public void SetStorageBuffer(string uniformBlockName, IBuffer storageBuffer)
        {
            if (_uniformBufferBindings.TryGetValue(uniformBlockName, out var uniformBufferBinding))
            {
                GL.BindBufferBase(GL.BufferTargetARB.ShaderStorageBuffer, uniformBufferBinding, (Buffer)storageBuffer);
            }
        }

        public void SetVertexUniform(string uniformName, int value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_vertexShader, location, value);
            }
        }

        public void SetVertexUniform(string uniformName, float value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_vertexShader, location, value);
            }
        }

        public void SetVertexUniform(string uniformName, Vector2 value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_vertexShader, location, value);
            }
        }

        public void SetVertexUniform(string uniformName, Vector3 value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_vertexShader, location, value);
            }
        }

        public void SetVertexUniform(string uniformName, Vector4 value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_vertexShader, location, value);
            }
        }

        public void SetVertexUniform(string uniformName, Matrix value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_vertexShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, int value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, Point value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, float value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, Vector2 value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, Vector3 value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, Vector4 value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        public void SetFragmentUniform(string uniformName, Matrix value)
        {
            if (_uniformLocations.TryGetValue(uniformName, out var location))
            {
                SetValue(_fragmentShader, location, value);
            }
        }

        private void ExtractUniforms()
        {
            ExtractUniforms(_programId);
        }

        private void ExtractUniforms(uint programId)
        {
            var uniformCount = 0;
            GL.GetProgram(programId, GL.ProgramPropertyARB.ActiveUniforms, ref uniformCount);
            for (var i = 0u; i < uniformCount; i++)
            {
                var uniformNameLength = 0;
                var uniformSize = 0;
                var uniformType = GL.UniformType.Bool;

                var uniformName = GL.GetActiveUniform(programId, i, 128, ref uniformNameLength, ref uniformSize, ref uniformType);
                var uniformLocation = GL.GetUniformLocation(programId, uniformName);
                if (uniformLocation != -1)
                {
                    _uniformLocations[uniformName] = uniformLocation;
                }
            }

            uniformCount = 0;
            GL.GetProgram(programId, GL.ProgramPropertyARB.ActiveUniformBlocks, ref uniformCount);
            for (var i = 0u; i < uniformCount; i++)
            {
                var blockBinding = 0;
                GL.GetActiveUniformBlock(programId, i, GL.UniformBlockPName.UniformBlockBinding, ref blockBinding);
                var uniformBlockNameLength = 0;
                var uniformBlockName = GL.GetActiveUniformBlockName(programId, i, 128, ref uniformBlockNameLength);

                _uniformBlockBindings[uniformBlockName] = (uint)blockBinding;
            }

            /*
            var resourceBufferCount = 0;
            GL.GetProgramInterface(programId, GL.ProgramInterface.ShaderStorageBlock, GL.ProgramInterfacePName.ActiveResources, ref resourceBufferCount);
            var blockProperties = new Span<GL.ProgramResourceProperty>(new[]
            {
                GL.ProgramResourceProperty.BufferBinding,
            });

            for (var i = 0u; i < resourceBufferCount; i++)
            {
                var blockPropertyValues = new int[blockProperties.Length];
                var length = blockPropertyValues.Length;
                GL.GetProgramResource(_programId, GL.ProgramInterface.ShaderStorageBlock, i, blockProperties, ref length, blockPropertyValues);

                var resourceNameLength = 0;
                var resourceName = GL.GetProgramResourceName(programId, GL.ProgramInterface.ShaderStorageBlock, i, 64, ref resourceNameLength);
                _uniformBufferBindings[resourceName] = (uint)blockPropertyValues[0];
            }
            */
        }

        private static uint CreateShaderProgramFromFile(GL.ShaderType shaderType, string fileName)
        {
            var shaderText = File.ReadAllText(fileName);
            return CreateShaderProgramFromSource(shaderType, shaderText);
        }

        private static uint CreateShaderProgramFromSource(GL.ShaderType shaderType, string shaderText)
        {
            return GL.CreateShaderProgram(shaderType, shaderText);
        }

        private Result Validate()
        {
            var validateVertexShaderResult = ValidateProgram(_vertexShader);
            var validateFragmentShaderResult = ValidateProgram(_fragmentShader);

            var fromResourcesResult = Result.Combine(validateVertexShaderResult, validateFragmentShaderResult);
            if (fromResourcesResult.IsFailure)
            {
                return Result.Failure<Shader>(fromResourcesResult.Error);
            }

            GL.UseProgramStages(_programId, GL.UseProgramStageMask.VertexShaderBit, _vertexShader);
            GL.UseProgramStages(_programId, GL.UseProgramStageMask.FragmentShaderBit, _fragmentShader);

            ExtractUniforms();

            return Result.Success();
        }

        private void SetValue(uint program, int location, int value)
        {
            GL.ProgramUniform1i(program, location, value);
        }

        private void SetValue(uint program, int location, Point value)
        {
            GL.ProgramUniform2i(program, location, value.X, value.Y);
        }

        private void SetValue(uint program, int location, float value)
        {
            GL.ProgramUniform1f(program, location, value);
        }

        private void SetValue(uint program, int location, Vector2 value)
        {
            GL.ProgramUniform2f(program, location, value.X, value.Y);
        }

        private void SetValue(uint program, int location, Vector3 value)
        {
            GL.ProgramUniform3f(program, location, value.X, value.Y, value.Z);
        }

        private void SetValue(uint program, int location, Vector4 value)
        {
            GL.ProgramUniform4f(program, location, value.X, value.Y, value.Z, value.W);
        }

        private void SetValue(uint program, int location, Matrix value)
        {
            GL.ProgramUniformMatrix4f(program, location, false, value.M11);
        }

        private Result ValidateProgram(uint shader)
        {
            GL.ProgramParameteri(shader, GL.ProgramParameterPName.ProgramSeparable, 1);
            var linkStatus = 0;
            GL.GetProgram(shader, GL.ProgramPropertyARB.LinkStatus, ref linkStatus);

            if (linkStatus == 0)
            {
                var infoLogLength = 0;
                var programInfoLog = GL.GetProgramInfoLog(shader, 1024, ref infoLogLength);
                GL.DeleteProgram(shader);

                return Result.Failure(programInfoLog);
            }

            ExtractUniforms(shader);

            return Result.Success();
        }
    }
}