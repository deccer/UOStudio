using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Network.PacketHandlers
{
    public interface IPacketHandler<in TPacket>
        where TPacket : IPacket
    {
        Task<Result> Handle(TPacket command);
    }

    public interface IRequestHandler<in TPacket, TResult>
        where TPacket : IPacket
    {
        Task<Result<TResult>> Handle(TPacket command);
    }
}
