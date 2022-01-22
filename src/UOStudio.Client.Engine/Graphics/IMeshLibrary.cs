namespace UOStudio.Client.Engine.Graphics
{
    public interface IMeshLibrary
    {
        void AddMesh(string meshName, string filePath);

        IReadOnlyList<MeshData> GetMesh(string meshName);
    }
}