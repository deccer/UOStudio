using System.Runtime.Serialization;

namespace UOStudio.Client.Engine
{
    [Serializable]
    public class SpaceConflictException : Exception
    {
        public SpaceConflictException()
        {
        }

        protected SpaceConflictException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public SpaceConflictException(string? message)
            : base(message)
        {
        }

        public SpaceConflictException(
            string? message,
            Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
