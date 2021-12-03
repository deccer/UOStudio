using System;
using Microsoft.Xna.Framework;

namespace UOStudio.Client
{
    public class FrameTimeCalculator
    {
        private const int DesiredFrameRate = 60;
        private const int NumberSamples = 128;

        private readonly int[] _samples = new int[NumberSamples];
        private float _fps;
        private int _currentSample;
        private int _ticksAggregate;
        private float _averageFrameTime;

        public FrameTimeCalculator(Game game)
        {
            game.TargetElapsedTime = new TimeSpan(TimeSpan.TicksPerSecond / DesiredFrameRate);
        }

        public float Fps => _fps;

        public float AverageFrameTime => _averageFrameTime;

        public void Calculate(GameTime gameTime)
        {
            var ticks = (int)gameTime.ElapsedGameTime.Ticks;
            _samples[_currentSample++] = ticks;
            _ticksAggregate += ticks;
            if (_ticksAggregate > TimeSpan.TicksPerSecond)
            {
                _ticksAggregate -= (int)TimeSpan.TicksPerSecond;
            }
            if (_currentSample == NumberSamples)
            {
                _averageFrameTime = Sum(_samples) / NumberSamples;
                _fps = TimeSpan.TicksPerSecond / _averageFrameTime;
                _currentSample = 0;
            }
        }

        private static float Sum(int[] samples)
        {
            float value = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                value = value + samples[i];
            }
            return value;
        }
    }
}
