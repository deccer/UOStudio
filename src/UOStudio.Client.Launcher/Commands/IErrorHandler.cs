using System;

namespace UOStudio.Client.Launcher.Commands
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
