using CSharpFunctionalExtensions;
using UOStudio.Client.Engine.Mathematics;

namespace UOStudio.Client.Engine.Graphics
{
    public class Material
    {
        public string Name { get; set; }

        public Vector3 AmbientColor { get; set; }
        
        public Vector3 DiffuseColor { get; set; }
        
        public Vector3 EmissiveColor { get; set; }
        
        public Vector3 SpecularColor { get; set; }

        public Maybe<string> AmbientTextureFilePath { get; set; }

        public Maybe<string> AmbientOcclusionTextureFilePath { get; set; }

        public Maybe<string> DiffuseTextureFilePath { get; set; }

        public Maybe<string> DisplacementTextureFilePath { get; set; }

        public Maybe<string> NormalTextureFilePath { get; set; }

        public Maybe<string> HeightMapTextureFilePath { get; set; }

        public Maybe<string> SpecularTextureFilePath { get; set; }
    }
}