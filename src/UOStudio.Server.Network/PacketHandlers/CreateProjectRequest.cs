using System;
using LiteNetLib.Utils;

namespace UOStudio.Server.Network.PacketHandlers
{
    public readonly struct CreateProjectRequest : IPacket
    {
        public CreateProjectRequest(NetDataReader reader)
        {
            AccountId = Guid.Parse(reader.GetString());
            Name = reader.GetString();
            Description = reader.GetString();
            ClientVersion = reader.GetString();
        }
        
        public Guid AccountId { get;}
        
        public string Name { get; }
        
        public string Description { get; }
        
        public string ClientVersion { get; } 
    }
}
