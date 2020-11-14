using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Network.PacketHandlers
{
    public interface IPacketProcessor
    {
        Task<Result> Process<TPacket>(IPacket packet)
            where TPacket : IPacket;

        Task<Result<TResult>> Process<TPacket, TResult>(IPacket packet)
            where TPacket : IPacket;
    }
}
