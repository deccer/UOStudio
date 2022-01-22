namespace UOStudio.Client.Engine.Input
{
    public struct MouseState
    {
        public int X { get; internal set; }

        public int Y { get; internal set; }

        public ButtonState LeftButton { get; internal set; }

        public ButtonState RightButton { get; internal set; }

        public ButtonState MiddleButton { get; internal set; }

        public ButtonState XButton1 { get; internal set; }

        public ButtonState XButton2 { get; internal set; }

        public ButtonState XButton3 { get; internal set; }

        public ButtonState XButton4 { get; internal set; }

        public int ScrollWheelValue { get; internal set; }

        public MouseState(
            int x,
            int y,
            int scrollWheel,
            ButtonState leftButton,
            ButtonState middleButton,
            ButtonState rightButton,
            ButtonState xButton1,
            ButtonState xButton2,
            ButtonState xButton3,
            ButtonState xButton4
        )
        {
            X = x;
            Y = y;
            ScrollWheelValue = scrollWheel;
            LeftButton = leftButton;
            MiddleButton = middleButton;
            RightButton = rightButton;
            XButton1 = xButton1;
            XButton2 = xButton2;
            XButton3 = xButton3;
            XButton4 = xButton4;
        }

        public static bool operator ==(MouseState left, MouseState right)
        {
            return left.X == right.X &&
                   left.Y == right.Y &&
                   left.LeftButton == right.LeftButton &&
                   left.MiddleButton == right.MiddleButton &&
                   left.RightButton == right.RightButton &&
                   left.ScrollWheelValue == right.ScrollWheelValue &&
                   left.XButton1 == right.XButton1 &&
                   left.XButton2 == right.XButton2 &&
                   left.XButton3 == right.XButton3 &&
                   left.XButton4 == right.XButton4;
        }

        public static bool operator !=(MouseState left, MouseState right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is MouseState mouseState && this == mouseState;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
