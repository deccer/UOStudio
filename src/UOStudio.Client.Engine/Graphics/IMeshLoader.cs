namespace UOStudio.Client.Engine.Graphics
{
    public interface IMeshLoader
    {
        IReadOnlyCollection<MeshData> LoadMesh(string filePath);
    }
}