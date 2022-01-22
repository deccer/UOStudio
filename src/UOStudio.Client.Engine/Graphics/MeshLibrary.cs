using System.Diagnostics;
using Serilog;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class MeshLibrary : IMeshLibrary
    {
        private readonly ILogger _logger;
        private readonly IMeshLoader _meshLoader;
        private readonly IDictionary<string, MeshData> _meshes;
        private readonly IDictionary<string, string> _meshNameToFilePathMap;
        private readonly IDictionary<string, IList<string>> _filePathToMeshNamesInFile;

        public MeshLibrary(
            ILogger logger,
            IMeshLoader meshLoader)
        {
            _logger = logger.ForContext<MeshLibrary>();
            _meshLoader = meshLoader;
            _meshes = new Dictionary<string, MeshData>();

            _meshNameToFilePathMap = new Dictionary<string, string>(8);
            _filePathToMeshNamesInFile = new Dictionary<string, IList<string>>(8);
        }

        public void AddMesh(string meshName, string filePath)
        {
            if (_meshes.ContainsKey(meshName))
            {
                return;
            }

            _logger.Debug("MeshLibrary: Adding mesh {@MeshName} from {@FilePath}", meshName, filePath);
            var meshDates = _meshLoader.LoadMesh(filePath);
            foreach (var meshData in meshDates)
            {
                if (_meshes.ContainsKey(meshData.MeshName))
                {
                    meshData.MeshName += $".{Guid.NewGuid().ToString()}";
                }
                _meshes.Add(meshData.MeshName, meshData);
            }

            if (_filePathToMeshNamesInFile.ContainsKey(filePath))
            {
                filePath += $".{Guid.NewGuid()}";
            }
            _meshNameToFilePathMap.Add(meshName, filePath);
            _filePathToMeshNamesInFile.Add(filePath, meshDates.Select(md => md.MeshName).ToList());
        }

        public IReadOnlyList<MeshData> GetMesh(string meshName)
        {
            // resolve meshName to filePath
            if (_meshNameToFilePathMap.TryGetValue(meshName, out var filePath))
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return new List<MeshData>();
                }
                // resolve meshNames which exist in filePath
                if (_filePathToMeshNamesInFile.TryGetValue(filePath, out var meshNamesPerFileName))
                {
                    var meshDatesInFilePath = new List<MeshData>();
                    if (meshNamesPerFileName == null)
                    {
                        Debugger.Break();
                        return meshDatesInFilePath;
                    }

                    // and resolve meshData per meshName from filePath
                    foreach (var meshNamePerFileName in meshNamesPerFileName)
                    {
                        if (_meshes.TryGetValue(meshNamePerFileName, out var meshData))
                        {
                            meshDatesInFilePath.Add(meshData);
                        }
                    }

                    return meshDatesInFilePath;
                }

                _logger.Debug("MeshLibrary: Mesh {@MeshName} not found", meshName);
                return Enumerable.Empty<MeshData>().ToList();
            }

            _logger.Debug("MeshLibrary: Mesh {@MeshName} not found", meshName);
            return Enumerable.Empty<MeshData>().ToList();
        }
    }
}
