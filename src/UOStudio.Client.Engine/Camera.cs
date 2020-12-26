using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UOStudio.Client.Engine
{
    public class Camera
    {
        private Vector3 _position;
        private Vector3 _direction;
        private float _nearPlane;
        private float _farPlane;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;

        public Camera(float width, float height)
        {
            _position = new Vector3(0, 0, 768);
            _direction = Vector3.Forward;
            _nearPlane = 0.1f;
            _farPlane = 2048f;
            UpdateProjectionMatrix(width, height);
            UpdateViewMatrix();
        }

        public CameraMode Mode { get; set; }

        public float NearPlane
        {
            get => _nearPlane;
            set
            {
                _nearPlane = value;
                UpdateViewMatrix();
            }
        }

        public float FarPlane
        {
            get => _farPlane;
            set
            {
                _farPlane = value;
                UpdateViewMatrix();
            }
        }

        public Matrix ViewMatrix => _viewMatrix;

        public Matrix ProjectionMatrix => _projectionMatrix;

        public void Update(float width, float height)
        {
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            var speedFactor = 1.0f;

            if (_currentKeyboardState.IsKeyDown(Keys.LeftShift) || _currentKeyboardState.IsKeyDown(Keys.RightShift))
            {
                speedFactor = 10.0f;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
            {
                _position += Vector3.Left * speedFactor;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
            {
                _position += Vector3.Right * speedFactor;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up))
            {
                _position += Vector3.Up * speedFactor;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentKeyboardState.IsKeyDown(Keys.Down))
            {
                _position += Vector3.Down * speedFactor;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Q) || _currentKeyboardState.IsKeyDown(Keys.PageUp))
            {
                _position += Vector3.Forward * speedFactor;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Z) || _currentKeyboardState.IsKeyDown(Keys.PageDown))
            {
                _position += Vector3.Backward * speedFactor;
            }

            UpdateViewMatrix();
        }

        private void UpdateProjectionMatrix(float width, float height)
        {
            _projectionMatrix = Mode == CameraMode.Perspective
                ? Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / height, _nearPlane, _farPlane)
                : Matrix.CreateOrthographic(width, height, _nearPlane, _farPlane);
        }

        private void UpdateViewMatrix()
        {
            _viewMatrix = Matrix.CreateLookAt(_position, _position + _direction, Vector3.Up);
        }
    }
}
