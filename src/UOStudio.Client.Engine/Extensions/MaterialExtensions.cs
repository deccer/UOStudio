using CSharpFunctionalExtensions;
using Material = UOStudio.Client.Engine.Graphics.Material;

namespace UOStudio.Client.Engine.Extensions
{
    public static class MaterialExtensions
    {
        public static Material ToEngine(this Assimp.Material material)
        {
            return new Material
            {
                Name = material.Name,
                AmbientColor = material.ColorAmbient.ToVector3(),
                DiffuseColor = material.ColorDiffuse.ToVector3(),
                EmissiveColor = material.ColorEmissive.ToVector3(),
                SpecularColor = material.ColorSpecular.ToVector3(),

                AmbientTextureFilePath = ToLocalTextures(material.TextureAmbient.FilePath),
                DiffuseTextureFilePath = ToLocalTextures(material.TextureDiffuse.FilePath),
                DisplacementTextureFilePath = ToLocalTextures(material.TextureDisplacement.FilePath),
                HeightMapTextureFilePath = ToLocalTextures(material.TextureHeight.FilePath),
                NormalTextureFilePath = ToLocalTextures(material.TextureNormal.FilePath),
                SpecularTextureFilePath = ToLocalTextures(material.TextureSpecular.FilePath)
            };
        }

        private static Maybe<string> ToLocalTextures(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return Maybe<string>.None;
            }

            var texturesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Textures");
            return Path.ChangeExtension(Path.Combine(texturesDirectory, Path.GetFileName(filePath)), ".png");
        }
    }
}