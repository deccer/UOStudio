namespace UOStudio.Client.Engine.Graphics
{
    public interface IShaderProgramLibrary : IDisposable
    {
        bool AddShaderProgram(
            string programName,
            string vertexShaderFilePath,
            string fragmentShaderFilePath);

        void DetectChangedFiles();

        void LoadShaders();

        IShader GetShaderProgram(string programName);
    }
}