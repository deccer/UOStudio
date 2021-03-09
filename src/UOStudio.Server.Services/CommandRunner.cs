using System.Diagnostics;
using System.Text;
using CSharpFunctionalExtensions;

namespace UOStudio.Server.Services
{
    public sealed class CommandRunner : ICommandRunner
    {
        private readonly StringBuilder _outputStringBuilder;
        private readonly StringBuilder _errorStringBuilder;

        public CommandRunner()
        {
            _outputStringBuilder = new StringBuilder(16384);
            _errorStringBuilder = new StringBuilder(16384);
        }

        public Result<string> RunCommand(string command, string arguments, string workDirectory)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(command, arguments)
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workDirectory
                }
            };
            process.Start();
            process.PriorityClass = ProcessPriorityClass.High;
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.WaitForExit();

            return process.ExitCode == 0
                ? Result.Success(_outputStringBuilder.ToString())
                : Result.Failure<string>(_errorStringBuilder.ToString());
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _outputStringBuilder.Append(e.Data);
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _errorStringBuilder.Append(e.Data);
        }
    }
}
