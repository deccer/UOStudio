using UOStudio.Client.Engine.Input;
using UOStudio.Client.Engine.Mathematics;
using Vector3 = UOStudio.Client.Engine.Mathematics.Vector3;

namespace UOStudio.Client.Engine
{
    public class Camera
    {
        private const float SPEED = 0.5f;
        private const float SENSITIVITY = 0.05f;
        private const float ZOOM = 45.0f;

        private readonly Vector3 _worldUp;
        private Vector3 _position;
        private Vector3 _front;
        private Vector3 _up;
        private Vector3 _right;

        private float _yaw;
        private float _pitch;

        private int _width;
        private int _height;
        private Vector2 _previousMousePosition;

        private CameraMode _cameraMode;

        public CameraMode CameraMode
        {
            get => _cameraMode;
            set => _cameraMode = value;
        }

        public Matrix ViewMatrix { get; private set; }

        public Matrix ProjectionMatrix { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateCameraVectors();
            }
        }

        public Vector3 Direction => _front;

        public Vector3 Right => _right;

        public Vector3 Up => _up;

        public Camera(int width, int height, Vector3 position, Vector3 up, CameraMode cameraMode = CameraMode.Perspective, float yaw = -90f, float pitch = 0.0f)
        {
            _width = width;
            _height = height;
            Position = position;
            _worldUp = up;
            _yaw = yaw;
            _pitch = pitch;
            _front = new Vector3(0, 0, -1);
            _previousMousePosition = new Vector2(_width / 2.0f, _height / 2.0f);
            _cameraMode = cameraMode;
            UpdateCameraVectors();
        }

        public void ProcessKeyboard(Vector3 movement, float deltaTime)
        {
            var velocity = SPEED * deltaTime;
            Position += movement * velocity;
        }

        public void ProcessMouseMovement()
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            var delta = _previousMousePosition - mousePosition;

            delta *= SENSITIVITY;
            _yaw += -delta.X;
            _pitch += delta.Y;
            if (_pitch > 89.0f)
            {
                _pitch = 89.0f;
            }
            if (_pitch < -89.0f)
            {
                _pitch = -89.0f;
            }

            UpdateCameraVectors();

            Mouse.SetPosition(_width / 2, _height / 2);
            _previousMousePosition = new Vector2(_width / 2.0f, _height / 2.0f);
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        private void UpdateCameraVectors()
        {
            if (_cameraMode == CameraMode.Perspective)
            {
                UpdateCameraVectorsForPerspective();
            }
            else
            {
                UpdateCameraVectorsForOrthogonal();
            }
        }

        private void UpdateCameraVectorsForPerspective()
        {
            var eulerAngles = new Vector3
            {
                X = MathF.Cos(MathHelper.ToRadians(_yaw)) * MathF.Cos(MathHelper.ToRadians(_pitch)),
                Y = MathF.Sin(MathHelper.ToRadians(_pitch)),
                Z = MathF.Sin(MathHelper.ToRadians(_yaw)) * MathF.Cos(MathHelper.ToRadians(_pitch))
            };

            _front = Vector3.Normalize(eulerAngles);
            _right = Vector3.Normalize(Vector3.Cross(_front, _worldUp));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));

            ViewMatrix = Matrix.LookAtRH(Position, Position + _front, _up);
            ProjectionMatrix = Matrix.PerspectiveFovRH(MathHelper.ToRadians(45), _width / (float)_height, 0.1f, 2048f);
        }

        private void UpdateCameraVectorsForOrthogonal()
        {
            var f = new Vector3
            {
                X = MathF.Cos(MathHelper.ToRadians(_yaw)) * MathF.Cos(MathHelper.ToRadians(_pitch)),
                Y = MathF.Sin(MathHelper.ToRadians(_pitch)),
                Z = MathF.Sin(MathHelper.ToRadians(_yaw)) * MathF.Cos(MathHelper.ToRadians(_pitch))
            };

            _front = Vector3.Normalize(f);
            _right = Vector3.Normalize(Vector3.Cross(_front, _worldUp));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));

            ViewMatrix = Matrix.LookAtRH(Position, Position + _front, _up);
            ProjectionMatrix = Matrix.OrthoRH(_width, _height, 0.1f, 2048f);
        }
    }
}
