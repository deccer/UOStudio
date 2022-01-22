using Serilog;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class MeshLoader : IMeshLoader
    {
        private readonly ILogger _logger;
        private readonly IMeshLibrary _meshLibrary;
        private readonly IMaterialLibrary _materialLibrary;

        public MeshLoader(
            ILogger logger,
            IMaterialLibrary materialLibrary)
        {
            _logger = logger.ForContext<MeshLoader>();
            _materialLibrary = materialLibrary;
        }

        public IReadOnlyCollection<MeshData> LoadMesh(string filePath)
        {
            return MeshData.CreateMeshDataFromFile(filePath, _materialLibrary);
        }
    }
}