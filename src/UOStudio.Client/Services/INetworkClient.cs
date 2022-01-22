using System;
using LiteNetLib;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Common.Network;

namespace UOStudio.Client.Services
{
    public interface INetworkClient
    {
        event Action<NetPeer> Connected;

        event Action Disconnected;

        event Action<ChunkData> ChunkReceived;

        void Connect(string hostname, int port);

        void Disconnect();

        void Update();

        /// <summary>
        /// Tells the server client is moving to <paramref name="position"/> and possibly request new chunks to be loaded
        /// </summary>
        /// <param name="worldId">Id of world to request a chunk</param>
        /// <param name="position">Position in world space</param>
        void RequestChunk(int worldId, Point position);
    }
}
