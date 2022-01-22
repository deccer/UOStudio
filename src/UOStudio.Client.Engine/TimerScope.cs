namespace UOStudio.Client.Engine
{
    public class TimerScope : IDisposable
    {
        private readonly Timer _timer;

        public TimerScope(Timer timer)
        {
            _timer = timer;
            timer.Begin();
        }

        public void Dispose()
        {
            _timer.End();
        }
    }
}