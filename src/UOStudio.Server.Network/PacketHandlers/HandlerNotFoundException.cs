using System;
using System.Runtime.Serialization;

namespace UOStudio.Server.Network.PacketHandlers
{
    [Serializable]
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(Type handlerType)
            : base($"No handler for {handlerType} found.")
        {
        }

        protected HandlerNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
