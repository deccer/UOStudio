using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace UOStudio.Client.Engine.Windows.General
{
    public class LogWindow : Window
    {
        private readonly IList<(LogType, DateTime, string)> _messages;

        public LogWindow()
            : base("Log")
        {
            _messages = new List<(LogType, DateTime, string)>(128);

            AddLogMessage(LogType.Info, "test1");
            AddLogMessage(LogType.Debug, "test2");
            AddLogMessage(LogType.Info, "test3");
            AddLogMessage(LogType.Info, "test3");
            AddLogMessage(LogType.Info, "test3");
            AddLogMessage(LogType.Info, "test3");
            AddLogMessage(LogType.Info, "test3");
            AddLogMessage(LogType.Error, "test4");
            AddLogMessage(LogType.Error, "test4");
            AddLogMessage(LogType.Error, "test4");
            AddLogMessage(LogType.Error, "test4");
            AddLogMessage(LogType.Warning, "test5");
            AddLogMessage(LogType.Warning, "test5");
            AddLogMessage(LogType.Warning, "test5");
            AddLogMessage(LogType.Warning, "test5");
            AddLogMessage(LogType.Warning, "test5");
        }

        public void AddLogMessage(LogType logType, string message)
        {
            _messages.Add((logType, DateTime.Now, message));
        }

        protected override void DrawInternal()
        {
            if (ImGui.Button("Clear"))
            {
                _messages.Clear();
            }
            ImGui.Separator();
            ImGui.BeginChild(Caption, Vector2.Zero, true, ImGuiWindowFlags.AlwaysVerticalScrollbar);
            for (var i = _messages.Count - 1; i > 0; i--)
            {
                var message = _messages[i];
                ImGui.TextColored(LogTypeToColor(message.Item1), message.Item2.ToString("yyyy-MM-dd HH:mm:ss"));
                ImGui.SameLine();
                ImGui.TextUnformatted(message.Item3);
            }
            ImGui.EndChild();
        }

        private static Vector4 LogTypeToColor(LogType logType)
        {
            return logType switch
            {
                LogType.Info => new Vector4(1, 1, 1, 1),
                LogType.Debug => new Vector4(0.2f, 0.8f, 0.0f, 1.0f),
                LogType.Warning => new Vector4(0.8f, 0.75f, 0.0f, 1.0f),
                LogType.Error => new Vector4(1.0f, 0.2f, 0.0f, 1.0f),
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
        }
    }
}
