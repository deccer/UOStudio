namespace UOStudio.Server.Network.PacketHandlers
{
    public readonly struct CreateProjectResult
    {
        public CreateProjectResult(int projectId)
        {
            ProjectId = projectId;
        }
        
        public int ProjectId { get; }
    }
}
