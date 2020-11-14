using System;
using System.Runtime.Serialization;

namespace UOStudio.Server.Network.PacketHandlers
{
    [Serializable]
    public class AmbiguousHandlerException : Exception
    {
        public AmbiguousHandlerException(string actionName)
            : base($"More than one handler found for action {actionName}.")
        {
        }

        protected AmbiguousHandlerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
