using System.Diagnostics;
using Assimp;
using UOStudio.Client.Engine.Extensions;
using UOStudio.Client.Engine.Mathematics;
using Matrix3x3 = UOStudio.Client.Engine.Mathematics.Matrix3x3;
using Vector2 = UOStudio.Client.Engine.Mathematics.Vector2;
using Vector3 = UOStudio.Client.Engine.Mathematics.Vector3;
using Vector4 = UOStudio.Client.Engine.Mathematics.Vector4;
using AssimpMesh = Assimp.Mesh;
using AssimpMaterial = Assimp.Material;

namespace UOStudio.Client.Engine.Graphics
{
    public sealed class MeshData
    {
        private readonly List<Vector3> _positions;

        private readonly List<Vector3> _normals;

        private readonly List<Color3> _colors;

        private readonly List<Vector3> _uvws;

        private readonly List<Vector2> _uvs;

        private readonly List<Vector3> _tangents;

        private readonly List<Vector3> _biTangents;

        private readonly List<Vector4> _realTangents;

        private readonly List<int> _indices;

        public string MeshName { get; set; }

        public Matrix Transform { get; set; }

        public IList<int> Indices => _indices;

        public int IndexCount => _indices.Count;

        public VertexType VertexType { get; private set; }

        public int VertexCount => _positions.Count;

        public int IndexOffset { get; set; }

        public int VertexOffset { get; set; }

        public string MaterialName { get; set; }

        public MeshData(string meshName)
        {
            MeshName = meshName;
            VertexType = VertexType.Unknown;
            _positions = new List<Vector3>(512);
            _normals = new List<Vector3>(512);
            _colors = new List<Color3>(512);
            _uvws = new List<Vector3>(512);
            _uvs = new List<Vector2>(512);
            _tangents = new List<Vector3>(512);
            _biTangents = new List<Vector3>(512);
            _realTangents = new List<Vector4>(512);
            _indices = new List<int>(1024);
        }

        public static IReadOnlyCollection<MeshData> CreateMeshDataFromFile(
            string fileName,
            IMaterialLibrary materialLibrary)
        {
            using var assimpContext = new AssimpContext();

            var postProcessSteps = PostProcessSteps.None;
            //postProcessSteps |= PostProcessSteps.PreTransformVertices;
            postProcessSteps |= PostProcessSteps.Triangulate;
            postProcessSteps |= PostProcessSteps.GenerateNormals;
            postProcessSteps |= PostProcessSteps.OptimizeMeshes;
            postProcessSteps |= PostProcessSteps.CalculateTangentSpace;
            postProcessSteps |= PostProcessSteps.JoinIdenticalVertices;
            postProcessSteps |= PostProcessSteps.FindDegenerates;
            postProcessSteps |= PostProcessSteps.GlobalScale;
            postProcessSteps |= PostProcessSteps.FindInvalidData;
            postProcessSteps |= PostProcessSteps.FlipWindingOrder;

            var scene = assimpContext.ImportFile(fileName, postProcessSteps);
            if (scene == null || scene.MeshCount == 0)
            {
                return null;
            }

            var materialNames = new List<string>(8);
            if (scene.HasMaterials)
            {
                ProcessMaterials(scene.Materials, materialLibrary, materialNames);
            }

            var meshDates = new List<MeshData>(8);
            ProcessNode(scene.RootNode, scene, materialNames, meshDates);

            return meshDates;
        }

        private static void ProcessMaterials(
            IEnumerable<AssimpMaterial> materials,
            IMaterialLibrary materialLibrary,
            IList<string> materialNames)
        {
            foreach (var material in materials)
            {
                materialLibrary.AddMaterial(material.ToEngine());
                materialNames.Add(material.Name);
            }
        }

        private static void ProcessNode(
            Node node,
            Scene scene,
            IList<string> materialNames,
            ICollection<MeshData> subMeshDates)
        {
            for (var i = 0; i < node.MeshCount; i++)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                subMeshDates.Add(ReadMesh(node, mesh, materialNames));
            }

            for (var i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene, materialNames, subMeshDates);
            }
        }

        private static MeshData ReadMesh(Node node, AssimpMesh mesh, IList<string> materialNames)
        {
            var meshData = new MeshData(mesh.Name);
            meshData.Transform = Matrix.Transpose(node.Transform.ToMatrix());
            //meshData.Transform = node.Transform.ToMatrix();
            meshData.MaterialName = materialNames[mesh.MaterialIndex];

            var hasUvwCoordinates = false;
            if (mesh.HasTextureCoords(0))
            {
                hasUvwCoordinates = mesh.TextureCoordinateChannels[0].Sum(uvw => uvw.Z) > 0.0f;
            }

            var positions = mesh.Vertices
                .Select(v => new Vector3(v.X, v.Y, v.Z))
                .Select(v => Vector3.TransformPosition(v, meshData.Transform))
                .ToArray();

            var aabb = BoundingBox.FromPoints(positions);
            var scaleFactor = Vector3.One;

            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var position = new Vector3(mesh.Vertices[i].X * scaleFactor.X, mesh.Vertices[i].Y * scaleFactor.Y,
                    mesh.Vertices[i].Z * scaleFactor.Z);
                if (mesh.Normals.Count > 0)
                {
                    var normal = mesh.Normals[i].ToVector3();
                    if (mesh.HasTextureCoords(0))
                    {
                        var uvs = mesh.TextureCoordinateChannels[0];
                        var uv = uvs[i].ToVector3();

                        if (hasUvwCoordinates)
                        {
                            if (mesh.HasTangentBasis)
                            {
                                var tangent = mesh.Tangents[i].ToVector3();
                                meshData.AddPositionNormalUvwTangent(
                                    position,
                                    normal,
                                    uv,
                                    tangent);
                            }
                            else
                            {
                                meshData.AddPositionNormalUvw(
                                    position,
                                    normal,
                                    uv);
                            }
                        }
                        else
                        {
                            if (mesh.HasTangentBasis)
                            {
                                var tangent = mesh.Tangents[i].ToVector3();
                                meshData.AddPositionNormalUvTangent(
                                    position,
                                    normal,
                                    new Vector2(uv.X, uv.Y),
                                    tangent);
                            }
                            else
                            {
                                meshData.AddPositionNormalUv(
                                    position,
                                    normal,
                                    new Vector2(uv.X, uv.Y));
                            }
                        }
                    }
                    else
                    {
                        meshData.AddPositionNormal(position, normal);
                    }
                }
                else
                {
                    meshData.AddPosition(position);
                }
            }

            for (var i = 0; i < mesh.FaceCount; i++)
            {
                meshData.AddIndices(mesh.Faces[i].Indices.ToArray());
            }

            meshData.CalculateTangents();
            return meshData;
        }

        public static MeshData Combine(params MeshData[] meshDates)
        {
            var combinedMeshData = new MeshData(Guid.NewGuid().ToString());
            combinedMeshData.VertexType = meshDates.First().VertexType;
            //var vertexOffset = 0;
            //var indexOffset = 0;

            foreach (var meshData in meshDates)
            {
                combinedMeshData._positions.AddRange(meshData._positions);
                combinedMeshData._normals.AddRange(meshData._normals);
                combinedMeshData._colors.AddRange(meshData._colors);
                combinedMeshData._uvws.AddRange(meshData._uvws);
                combinedMeshData._uvs.AddRange(meshData._uvs);
                combinedMeshData._tangents.AddRange(meshData._tangents);
                combinedMeshData._biTangents.AddRange(meshData._biTangents);
                combinedMeshData._realTangents.AddRange(meshData._realTangents);
                combinedMeshData._indices.AddRange(meshData._indices.Select(i => i));
                var combinedMeshName = string.IsNullOrEmpty(meshData.MeshName)
                    ? Guid.NewGuid().ToString()
                    : meshData.MeshName;
                //meshInfos.Add(combinedMeshName, new MeshInfo(meshData.VertexCount, vertexOffset, meshData.IndexCount, indexOffset, meshData.Transform));
                //vertexOffset += meshData._positions.Count;
                //indexOffset += meshData._indices.Count;
            }

            return combinedMeshData;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return BoundingSphere.FromPoints(_positions.ToArray());
        }

        public BoundingBox GetBoundingBox()
        {
            return BoundingBox.FromPoints(_positions.ToArray());
        }

        public void AddIndices(params int[] indices)
        {
            _indices.AddRange(indices);
        }

        public void AddPosition(Vector3 position)
        {
            AddVertex(position);
            VertexType = VertexType.Position;
        }

        public void AddPositionColor(
            Vector3 position,
            Color3 color)
        {
            AddVertex(position, color);
            VertexType = VertexType.PositionColor;
        }

        public void AddPositionColorNormal(
            Vector3 position,
            Color3 color,
            Vector3 normal)
        {
            AddVertex(position, color, normal);
            VertexType = VertexType.PositionColorNormal;
        }

        public void AddPositionColorNormalUv(
            Vector3 position,
            Color3 color,
            Vector3 normal,
            Vector2 uv)
        {
            AddVertex(position, color, normal, uv);
            VertexType = VertexType.PositionColorNormalUv;
        }

        public void AddPositionColorNormalUvw(
            Vector3 position,
            Color3 color,
            Vector3 normal,
            Vector3 uvw)
        {
            AddVertex(position, color, normal, new Vector2(uvw.X, uvw.Y), uvw);
            VertexType = VertexType.PositionColorNormalUvw;
        }

        public void AddPositionNormal(
            Vector3 position,
            Vector3 normal)
        {
            AddVertex(position, normal: normal);
            VertexType = VertexType.PositionNormal;
        }

        public void AddPositionNormalUv(
            Vector3 position,
            Vector3 normal,
            Vector2 uv)
        {
            AddVertex(position, normal: normal, uv: uv);
            VertexType = VertexType.PositionNormalUv;
        }

        public void AddPositionNormalUvTangent(
            Vector3 position,
            Vector3 normal,
            Vector2 uv,
            Vector3 tangent)
        {
            AddVertex(position, normal: normal, uv: uv, tangent: tangent);
            VertexType = VertexType.PositionNormalUvTangent;
        }

        public void AddPositionNormalUvw(
            Vector3 position,
            Vector3 normal,
            Vector3 uvw)
        {
            AddVertex(position, normal: normal, uv: new Vector2(uvw.X, uvw.Y), uvw: uvw);
            VertexType = VertexType.PositionNormalUvw;
        }

        public void AddPositionNormalUvwTangent(
            Vector3 position,
            Vector3 normal,
            Vector3 uvw,
            Vector3 tangent)
        {
            AddVertex(position, normal: normal, uv: new Vector2(uvw.X, uvw.Y), uvw: uvw, tangent: tangent);
            VertexType = VertexType.PositionNormalUvwTangent;
        }

        public IBuffer CreateVertexBuffer(IGraphicsDevice graphicsDevice)
        {
            return VertexType switch
            {
                VertexType.Position => CreateVertexPositionBuffer(graphicsDevice),
                VertexType.PositionColor => CreateVertexPositionColorBuffer(graphicsDevice),
                VertexType.PositionColorNormalUv => CreateVertexPositionColorNormalUvBuffer(graphicsDevice),
                VertexType.PositionColorNormalUvw => CreateVertexPositionColorNormalUvwBuffer(graphicsDevice),
                VertexType.PositionColorUv => CreateVertexPositionColorUvBuffer(graphicsDevice),
                VertexType.PositionColorUvw => CreateVertexPositionColorUvwBuffer(graphicsDevice),
                VertexType.PositionNormal => CreateVertexPositionNormalBuffer(graphicsDevice),
                VertexType.PositionNormalUv => CreateVertexPositionNormalUvBuffer(graphicsDevice),
                VertexType.PositionNormalUvTangent => CreateVertexPositionNormalUvTangentBuffer(graphicsDevice),
                VertexType.PositionNormalUvw => CreateVertexPositionNormalUvwBuffer(graphicsDevice),
                VertexType.PositionNormalUvwTangent => CreateVertexPositionNormalUvwTangentBuffer(graphicsDevice),
                VertexType.PositionUv => CreateVertexPositionUvBuffer(graphicsDevice),
                VertexType.PositionUvw => CreateVertexPositionUvwBuffer(graphicsDevice),
            };
        }

        public IBuffer CreateIndexBuffer(IGraphicsDevice graphicsDevice)
        {
            return graphicsDevice.CreateBuffer(_indices);
        }

        private static Vector3 GetScaleFactor(BoundingBox boundingBox)
        {
            return new Vector3(1.0f / MathF.Max(MathF.Max(boundingBox.Size.X, boundingBox.Size.Y), boundingBox.Size.Z));
        }

        private void AddVertex(
            Vector3 position,
            Vector3? color = null,
            Vector3? normal = null,
            Vector2? uv = null,
            Vector3? uvw = null,
            Vector3? tangent = null)
        {
            _positions.Add(position);
            _colors.Add(color ?? Vector3.Zero);
            _normals.Add(normal ?? Vector3.Zero);
            _uvs.Add(uv ?? Vector2.Zero);
            _uvws.Add(uvw ?? Vector3.Zero);
            _tangents.Add(tangent ?? Vector3.Zero);
            _biTangents.Add(Vector3.Zero);
        }

        private IBuffer CreateVertexPositionBuffer(IGraphicsDevice graphicsDevice)
        {
            return graphicsDevice.CreateBuffer(_positions.Select(position => new VertexPosition(position)).ToArray());
        }

        private IBuffer CreateVertexPositionColorBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _colors.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionColor>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionColor(_positions[i], _colors[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionColorNormalUvBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _colors.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionColorNormalUv>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionColorNormalUv(_positions[i], _colors[i], _normals[i], _uvs[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionColorNormalUvwBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _colors.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionColorNormalUvw>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionColorNormalUvw(_positions[i], _colors[i], _normals[i], _uvws[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionColorUvBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _colors.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _uvs.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionColorUv>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionColorUv(_positions[i], _colors[i], _uvs[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionColorUvwBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _colors.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _uvws.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionColorUvw>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionColorUvw(_positions[i], _colors[i], _uvws[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionNormalBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionNormal>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionNormal(_positions[i], _normals[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionNormalUvBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _uvs.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionNormalUv>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionNormalUv(_positions[i], _normals[i], _uvs[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionNormalUvTangentBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _uvs.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _realTangents.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionNormalUvTangent>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionNormalUvTangent(_positions[i], _normals[i], _uvs[i], _realTangents[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionNormalUvwBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _uvws.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionNormalUvw>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionNormalUvw(_positions[i], _normals[i], _uvws[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionNormalUvwTangentBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _normals.Count)
            {
                Debugger.Break();
            }

            if (_positions.Count != _realTangents.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionNormalUvwTangent>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionNormalUvwTangent(_positions[i], _normals[i], _uvws[i], _realTangents[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionUvBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _uvs.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionUv>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionUv(_positions[i], _uvs[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private IBuffer CreateVertexPositionUvwBuffer(IGraphicsDevice graphicsDevice)
        {
            if (_positions.Count != _uvws.Count)
            {
                Debugger.Break();
            }

            var vertices = new List<VertexPositionUvw>();
            for (var i = 0; i < _positions.Count; i++)
            {
                vertices.Add(new VertexPositionUvw(_positions[i], _uvws[i]));
            }

            return graphicsDevice.CreateBuffer(vertices);
        }

        private void CalculateTangents()
        {
            if (!_positions.Any())
            {
                return;
            }

            if (!_normals.Any())
            {
                return;
            }

            if (!_uvs.Any())
            {
                return;
            }

            for (var i = 0; i < _normals.Count; i++)
            {
                _normals[i] = Vector3.Normalize(_normals[i]);
            }

            for (var i = 0; i < _positions.Count; i += 3)
            {
                if (i >= _positions.Count || i + 1 >= _positions.Count || i + 2 >= _positions.Count ||
                    i >= _uvs.Count || i + 1 >= _uvs.Count || i + 3 >= _uvs.Count)
                {
                    break;
                }
                var triangle = Matrix3x3.Identity;
                triangle.Row1 = _positions[i + 0];
                triangle.Row2 = _positions[i + 1];
                triangle.Row3 = _positions[i + 2];

                var uv0 = _uvs[i + 1] - _uvs[i + 0];
                var uv1 = _uvs[i + 2] - _uvs[i + 0];

                var q1 = triangle.Row2 - triangle.Row1;
                var q2 = triangle.Row3 - triangle.Row1;

                var det = uv0.X * uv1.Y - uv1.X * uv0.Y;
                if (MathF.Abs(det) <= 0.000001f)
                {
                    det = 0.000001f;
                }

                var inverseDet = 1.0f / det;

                var tangent = new Vector3(
                    inverseDet * (uv1.Y * q1.X - uv0.Y * q2.X),
                    inverseDet * (uv1.Y * q1.Y - uv0.Y * q2.Y),
                    inverseDet * (uv1.Y * q1.Z - uv0.Y * q2.Z));
                var biTangent = new Vector3(
                    inverseDet * (-uv1.X * q1.X * uv0.X * q2.X),
                    inverseDet * (-uv1.X * q1.Y * uv0.X * q2.Y),
                    inverseDet * (-uv1.X * q1.Z * uv0.X * q2.Z));

                _tangents[i + 0] += tangent;
                _tangents[i + 1] += tangent;
                _tangents[i + 2] += tangent;
                _biTangents[i + 0] += biTangent;
                _biTangents[i + 1] += biTangent;
                _biTangents[i + 2] += biTangent;
            }

            for (var i = 0; i < _positions.Count; i++)
            {
                if (i >= _tangents.Count)
                {
                    break;
                }

                var normal = _normals[i];
                var tangent = _tangents[i];
                var biTangent = _biTangents[i];

                var realTangent = Vector3.Normalize(Vector3.Subtract(tangent, normal * Vector3.Dot(normal, tangent)));
                var realBiTangent = Vector3.Dot(Vector3.Cross(normal, tangent), biTangent) < 0.0f
                    ? -1.0f
                    : 1.0f;

                _realTangents.Add(new Vector4(realTangent, realBiTangent));
            }
        }
    }
}
