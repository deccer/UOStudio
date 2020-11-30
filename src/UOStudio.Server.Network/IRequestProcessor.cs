using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UOStudio.Server.Network.PacketHandlers;

namespace UOStudio.Server.Network
{
    public interface IRequestProcessor
    {
        Task<Result> Process<TPacket>(IPacket packet)
            where TPacket : IPacket;

        Task<Result<TResult>> Process<TPacket, TResult>(IPacket packet)
            where TPacket : IPacket;
    }
}
