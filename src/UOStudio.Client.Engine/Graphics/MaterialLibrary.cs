using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;
using Serilog;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    internal sealed class MaterialLibrary : IMaterialLibrary
    {
        private readonly ILogger _logger;
        private readonly IGraphicsDevice _graphicsDevice;
        private readonly ITextureLibrary _textureLibrary;
        private readonly IDictionary<string, Material> _materials;

        public MaterialLibrary(
            ILogger logger,
            IGraphicsDevice graphicsDevice,
            ITextureLibrary textureLibrary)
        {
            _logger = logger.ForContext<MaterialLibrary>();
            _graphicsDevice = graphicsDevice;
            _textureLibrary = textureLibrary;
            _materials = new Dictionary<string, Material>();
        }

        public void AddMaterial(Material material)
        {
            Guard.Against.Null(material, nameof(material));
            Guard.Against.NullOrEmpty(material.Name, nameof(material.Name));

            if (_materials.ContainsKey(material.Name))
            {
                _logger.Warning("MaterialLibrary: Material {@Name} already exists.", material.Name);
                return;
            }

            LoadMaterialTextures(material);
            _logger.Debug("MaterialLibrary: Adding Material {@Name}", material.Name);
            _materials.Add(material.Name, material);
        }

        public int IndexOfMaterial(string materialName)
        {
            return _materials.Keys.ToList().IndexOf(materialName);
        }

        public void UploadResources(
            out IBuffer materialBuffer,
            out IEnumerable<ITextureArray> textureArrays)
        {
            _textureLibrary.LoadResources(out textureArrays, out var indicesPerTextureName);
            var materials = _materials.Values.Select(material => ConvertMaterial(indicesPerTextureName, material));
            materialBuffer = _graphicsDevice.CreateBuffer(materials);
        }

        private GpuMaterial ConvertMaterial(
            IDictionary<string, (int ArrayIndex, int TextureIndex)> indicesPerTextureName,
            Material material)
        {
            var albedoTextureIndex = GetTextureIndex(indicesPerTextureName, material.DiffuseTextureFilePath);
            var normalTextureIndex = GetTextureIndex(indicesPerTextureName, material.NormalTextureFilePath);
            var roughnessTextureIndex = GetTextureIndex(indicesPerTextureName, material.DisplacementTextureFilePath);
            var specularTextureIndex = GetTextureIndex(indicesPerTextureName, material.SpecularTextureFilePath);

            return new GpuMaterial
            {
                ColorAmbient = new Vector4(material.AmbientColor, 0.0f),
                ColorDiffuse = new Vector4(material.DiffuseColor, 0.0f),
                ColorSpecularAndShininess = new Vector4(material.SpecularColor, 0.0f),
                TextureDiffuseAndNormal = new Int4(
                    albedoTextureIndex.ArrayIndex,
                    albedoTextureIndex.TextureIndex,
                    normalTextureIndex.ArrayIndex,
                    normalTextureIndex.TextureIndex),
                TextureRoughnessAndMetalness = new Int4(
                    roughnessTextureIndex.ArrayIndex,
                    roughnessTextureIndex.TextureIndex,
                    -1,
                    -1),
                TextureSpecularAndAmbientOcclusion = new Int4(
                    specularTextureIndex.ArrayIndex,
                    specularTextureIndex.TextureIndex,
                    -1,
                    -1)
            };
        }

        private (int ArrayIndex, int TextureIndex) GetTextureIndex(
            IDictionary<string, (int ArrayIndex, int TextureIndex)> indicesPerTextureName,
            Maybe<string> textureFilePath)
        {
            var emptyTextureIndex = (-1, -1);
            return textureFilePath.HasValue
                ? indicesPerTextureName.TryGetValue(textureFilePath.Value, out var textureIndex)
                    ? textureIndex
                    : emptyTextureIndex
                : emptyTextureIndex;
        }

        private void LoadMaterialTextures(Material material)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (material.AmbientTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.AmbientTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.AmbientTextureFilePath.Value, ".png"));
            }

            if (material.AmbientOcclusionTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.AmbientOcclusionTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.AmbientOcclusionTextureFilePath.Value, ".png"));
            }

            if (material.DiffuseTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.DiffuseTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.DiffuseTextureFilePath.Value, ".png"));
            }

            if (material.DisplacementTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.DisplacementTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.DisplacementTextureFilePath.Value, ".png"));
            }

            if (material.HeightMapTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.HeightMapTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.HeightMapTextureFilePath.Value, ".png"));
            }

            if (material.NormalTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.NormalTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.NormalTextureFilePath.Value, ".png"));
            }

            if (material.SpecularTextureFilePath.HasValue)
            {
                var textureName = Path.GetRelativePath(baseDirectory, Path.ChangeExtension(material.SpecularTextureFilePath.Value, ".png"));
                _textureLibrary.AddTextureFromFile(textureName, Path.ChangeExtension(material.SpecularTextureFilePath.Value, ".png"));
            }
        }
    }
}