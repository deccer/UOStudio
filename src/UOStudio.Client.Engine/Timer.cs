using System.Diagnostics;
using UOStudio.Client.Engine.Mathematics;
using UOStudio.Client.Engine.Native.OpenGL;

namespace UOStudio.Client.Engine
{
    public class Timer : IDisposable
    {
        private static readonly List<Timer> timers = new List<Timer>();
        private static int index = 0;
        private static Timer activeTimer;
        private static readonly HashSet<Timer> active = new HashSet<Timer>();

        public static ICollection<Timer> Timers => timers.AsReadOnly();

        public static void BeginFrame()
        {
            foreach (var timer in timers)
            {
                timer.BeginFrameInstance();
            }
        }

        public static void EndFrame()
        {
            foreach (var timer in timers)
            {
                timer.EndFrameInstance();
            }

            ++index;
            if (index == 4)
            {
                index = 0;
            }
        }

        private readonly bool[] _beginQuery = new bool[4];
        private readonly bool[] _endQuery = new bool[4];
        private readonly bool[] _pendingQuery = new bool[4];
        private readonly int[] _queryObjects = new int[4];
        private readonly Stopwatch _stopwatch;
        private int _lastIndex = -1; // nothing to wait
        private ulong _lastResult;
        private readonly bool _useGpu;
        private bool _usedGpuThisFrame = false;
        public Vector3 Color;
        public string Label;

        public ulong LastResult => _lastResult;

        public override string ToString()
        {
            return _useGpu
                ? Label + " " + GPUTime.ToString("0.00") + " / " + CPUTime.ToString("0.00")
                : Label + " " + CPUTime.ToString("0.00");
        }

        public float CPUTime => (float)(1000.0 * _stopwatch.ElapsedTicks / Stopwatch.Frequency);
        public float GPUTime => _useGpu ? _lastResult / 1000000.0f : 0.0f;

        public Timer(string label, float r, float g, float b, bool useGpu)
            : this(useGpu)
        {
            Label = label;
            Color = new Vector3(r, g, b);
        }

        public Timer(string label, float r, float g, float b)
            : this(label, r, g, b, false)
        {
        }

        public Timer() : this(true)
        {
        }

        public unsafe Timer(bool useGpu)
        {
            _useGpu = useGpu;
            timers.Add(this);
            if (_useGpu)
            {
                GL.GenQueries(_queryObjects.Length, _queryObjects);
            }

            _stopwatch = new Stopwatch();
        }

        ~Timer()
        {
            Dispose();
        }

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed)
            {
                timers.Remove(this);
            }
        }

        public void Begin()
        {
            if (_useGpu)
            {
                if (activeTimer != null)
                {
                    throw new Exception("!");
                }

                activeTimer = this;
                {
                    GL.BeginQuery(GL.QueryTarget.TimeElapsed, _queryObjects[index]);
                    active.Add(this);
                    _beginQuery[index] = true;
                    _usedGpuThisFrame = true;
                    _lastIndex = index;
                }
            }

            _stopwatch.Start();
        }

        public void End()
        {
            if (_useGpu)
            {
                {
                    if (activeTimer != this)
                    {
                        throw new System.Exception("!");
                    }

                    GL.EndQuery(GL.QueryTarget.TimeElapsed);
                    active.Remove(this);
                    _endQuery[index] = true;
                    _pendingQuery[index] = true;
                    if (_lastIndex != index)
                    {
                        throw new InvalidOperationException("query crossed frame");
                    }
                }
                activeTimer = null;
            }

            _stopwatch.Stop();
        }

        private void BeginFrameInstance()
        {
            _stopwatch.Reset();
        }

        private void EndFrameInstance()
        {
            if (_usedGpuThisFrame == false)
            {
                _lastResult = 0;
            }

            if (_useGpu == false)
            {
                return;
            }

            if (_usedGpuThisFrame == false)
            {
                return;
            }

            var available = 0;
            // Start polling from lastWritter + 1 eg. 3 frames behind
            var pollIndex = index + 1;
            if (pollIndex == 4)
            {
                pollIndex = 0;
            }
            for (var stepCount = 0; stepCount < 5; ++stepCount)
            {
                if (_pendingQuery[pollIndex])
                {
                    var ok = GL.IsQuery(_queryObjects[pollIndex]);
                    if (ok == false)
                    {
                        var ok0 = GL.IsQuery(_queryObjects[0]);
                        var ok1 = GL.IsQuery(_queryObjects[1]);
                        var ok2 = GL.IsQuery(_queryObjects[2]);
                        var ok3 = GL.IsQuery(_queryObjects[3]);
                        var allOk = ok0 && ok1 && ok2 && ok3;
                    }

                    GL.GetQueryObjecti(_queryObjects[pollIndex], GL.QueryObjectParameterName.QueryResultAvailable,
                        ref available);
                    if (available != 0)
                    {
                        ulong time = 0;
                        GL.GetQueryObjectui64(_queryObjects[pollIndex], GL.QueryObjectParameterName.QueryResult, ref time);
                        _lastResult = time;
                        _pendingQuery[pollIndex] = false;
                    }
                }

                pollIndex = (pollIndex == 3) ? 0 : pollIndex + 1;
            }

            for (int i = 0; i < 4; ++i)
            {
                _beginQuery[i] = false;
                _endQuery[i] = false;
            }

            //  Reset for next time
            _usedGpuThisFrame = false;
        }
    }
}