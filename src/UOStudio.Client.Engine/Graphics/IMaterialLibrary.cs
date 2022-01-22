namespace UOStudio.Client.Engine.Graphics
{
    public interface IMaterialLibrary
    {
        void AddMaterial(Material material);

        int IndexOfMaterial(string materialName);

        void UploadResources(
            out IBuffer materialBuffer,
            out IEnumerable<ITextureArray> textureArrays);
    }
}