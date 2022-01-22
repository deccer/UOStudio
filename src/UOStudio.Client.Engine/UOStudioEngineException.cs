using System.Runtime.Serialization;

namespace UOStudio.Client.Engine
{
    [Serializable]
    public class UOStudioEngineException : Exception
    {
        public UOStudioEngineException()
        {
        }

        protected UOStudioEngineException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public UOStudioEngineException(string? message)
            : base(message)
        {
        }

        public UOStudioEngineException(
            string? message,
            Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
