using System;

namespace UOStudio.Client.Engine.Windows
{
    public sealed class ToolDescription
    {
        public string Name { get; set; }

        public IntPtr TextureHandle { get; set; }

        public ToolGroup Group { get; set; }

        public int Size { get; set; }

        public event Action Clicked;

        public void Click()
        {
            var clicked = Clicked;
            clicked?.Invoke();
        }
    }
}
