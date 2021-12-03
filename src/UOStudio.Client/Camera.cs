using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UOStudio.Client
{
    public class Camera
    {
        private const float Epsilon = 0.0001f;
        private Vector3 _position;
        private Vector3 _direction;
        private float _nearPlane;
        private float _farPlane;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private float _aspectRatio;

        public Camera(float width, float height)
        {
            _position = new Vector3(0, 0, 768);
            _direction = Vector3.Forward;
            _nearPlane = 0.1f;
            _farPlane = 8192;
            _aspectRatio = width / height;
            UpdateProjectionMatrix(width, height);
            UpdateViewMatrix();
        }

        public CameraMode Mode { get; set; }

        public Vector3 Position => _position;

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

        public bool Update(
            float width,
            float height,
            KeyboardState currentKeyboardState,
            MouseState currentMouseState,
            KeyboardState previousKeyboardState,
            MouseState previousMouseState)
        {
            var wasMoved = false;
            var speedFactor = 1.0f;

            if ((currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift)) &&
                (previousKeyboardState.IsKeyUp(Keys.LeftShift) || previousKeyboardState.IsKeyUp(Keys.RightShift)))
            {
                speedFactor = 10.0f;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left)) &&
                (previousKeyboardState.IsKeyUp(Keys.A) || previousKeyboardState.IsKeyUp(Keys.Left)))
            {
                _position += Vector3.Left * speedFactor;
                wasMoved = true;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right)) &&
                (previousKeyboardState.IsKeyUp(Keys.D) || previousKeyboardState.IsKeyUp(Keys.Right)))
            {
                _position += Vector3.Right * speedFactor;
                wasMoved = true;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up)) &&
                (previousKeyboardState.IsKeyUp(Keys.W) || previousKeyboardState.IsKeyUp(Keys.Up)))
            {
                _position += Vector3.Up * speedFactor;
                wasMoved = true;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down)) &&
                (previousKeyboardState.IsKeyUp(Keys.S) || previousKeyboardState.IsKeyUp(Keys.Down)))
            {
                _position += Vector3.Down * speedFactor;
                wasMoved = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Q) && previousKeyboardState.IsKeyUp(Keys.Q))
            {
                _position += Vector3.Forward * speedFactor;
                wasMoved = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Z) && previousKeyboardState.IsKeyUp(Keys.Z))
            {
                _position += Vector3.Backward * speedFactor;
                wasMoved = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.PageUp))
            {
                _position += Vector3.Up * speedFactor;
                wasMoved = true;
            }

            if (currentKeyboardState.IsKeyDown(Keys.PageUp))
            {
                _position += Vector3.Down * speedFactor;
                wasMoved = true;
            }

            if (Math.Abs(_aspectRatio - width / height) > Epsilon)
            {
                UpdateProjectionMatrix(width, height);
            }

            UpdateViewMatrix();

            return wasMoved;
        }

        private void UpdateProjectionMatrix(float width, float height)
        {
            _aspectRatio = width / height;
            _projectionMatrix = Mode == CameraMode.Perspective
                ? Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _aspectRatio, _nearPlane, _farPlane)
                : Matrix.CreateOrthographic(width, height, _nearPlane, _farPlane);
        }

        private void UpdateViewMatrix()
        {
            _viewMatrix = Matrix.CreateLookAt(_position, _position + _direction, Vector3.Up);
        }
    }
}
