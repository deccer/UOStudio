using System;
using System.Runtime.Serialization;

namespace UOStudio.Client.Engine.Native
{
    public sealed class SdlException : Exception
    {
        public SdlException()
        {
        }

        public SdlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SdlException(string message)
            : base(message)
        {
        }

        public SdlException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
