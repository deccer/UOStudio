using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UOStudio.Client.Engine
{
    public sealed class LandTile : Tile
    {
        private static Vector3[,,] _vectorCache = new Vector3[3, 3, 4];

        public LandTile(ushort graphicId, ushort x, ushort y, sbyte groundZ)
            : base(graphicId, x, y, groundZ)
        {
            IsStretched = graphicId > 0;
            MinimumZ = groundZ;
            AverageZ = groundZ;
        }

        public bool IsStretched { get; set; }

        public sbyte AverageZ { get; set; }

        public sbyte MinimumZ { get; set; }

        public Rectangle Rectangle { get; private set; }

        public Vector3 Normal0;

        public Vector3 Normal1;

        public Vector3 Normal2;

        public Vector3 Normal3;

        public void ApplyStretch(Map map, int x, int y, sbyte z)
        {
            if (IsStretched || !TestIfStretched(map, x, y, z, true))
            {
                IsStretched = false;
                MinimumZ = z;
            }
            else
            {
                IsStretched = true;

                var zWest = map.GetTileZ(x, y + 1);
                var zEast = map.GetTileZ(x + 1, y);
                var zSouth = map.GetTileZ(x + 1, y + 1);
                UpdateZ(z, zWest, zEast, zSouth);

                int i;
                int j;

                for (i = -1; i < 2; ++i)
                {
                    int curX = x + i;
                    int curI = i + 1;

                    for (j = -1; j < 2; ++j)
                    {
                        int curY = y + j;
                        int curJ = j + 1;
                        sbyte currentZ = map.GetTileZ(curX, curY);
                        sbyte leftZ = map.GetTileZ(curX, curY + 1);
                        sbyte rightZ = map.GetTileZ(curX + 1, curY);
                        sbyte bottomZ = map.GetTileZ(curX + 1, curY + 1);

                        if (currentZ == leftZ && currentZ == rightZ && currentZ == bottomZ)
                        {
                            for (int k = 0; k < 4; ++k)
                            {
                                ref Vector3 v = ref _vectorCache[curI, curJ, k];
                                v.X = 0;
                                v.Y = 0;
                                v.Z = 1;
                            }
                        }
                        else
                        {
                            int half_0 = (currentZ - rightZ) << 2;
                            int half_1 = (leftZ - currentZ) << 2;
                            int half_2 = (rightZ - bottomZ) << 2;
                            int half_3 = (bottomZ - leftZ) << 2;

                            ref Vector3 v0 = ref _vectorCache[curI, curJ, 0];
                            v0.X = -TileSizeHalf;
                            v0.Y = TileSizeHalf;
                            v0.Z = half_0;
                            MergeAndNormalize(ref v0, -TileSizeHalf, -TileSizeHalf, half_1);

                            ref Vector3 v1 = ref _vectorCache[curI, curJ, 1];
                            v1.X = TileSizeHalf;
                            v1.Y = TileSizeHalf;
                            v1.Z = half_2;
                            MergeAndNormalize(ref v1, -TileSizeHalf, TileSizeHalf, half_0);

                            ref Vector3 v2 = ref _vectorCache[curI, curJ, 2];
                            v2.X = TileSizeHalf;
                            v2.Y = -TileSizeHalf;
                            v2.Z = half_3;
                            MergeAndNormalize(ref v2, TileSizeHalf, TileSizeHalf, half_2);

                            ref Vector3 v3 = ref _vectorCache[curI, curJ, 3];
                            v3.X = -TileSizeHalf;
                            v3.Y = -TileSizeHalf;
                            v3.Z = half_1;
                            MergeAndNormalize(ref v3, TileSizeHalf, -TileSizeHalf, half_3);
                        }
                    }
                }

                i = 1;
                j = 1;

                // 0
                SumAndNormalize(ref _vectorCache, i - 1, j - 1, 2, i - 1, j, 1, i, j - 1, 3, i, j, 0, out Normal0);
                // 1
                SumAndNormalize(ref _vectorCache, i, j - 1, 2, i, j, 1, i + 1, j - 1, 3, i + 1, j, 0, out Normal1);
                // 2
                SumAndNormalize(ref _vectorCache, i, j, 2, i, j + 1, 1, i + 1, j, 3, i + 1, j + 1, 0, out Normal2);
                // 3
                SumAndNormalize(ref _vectorCache, i - 1, j, 2, i - 1, j + 1, 1, i, j, 3, i, j + 1, 0, out Normal3);
            }
        }

        public override void Draw(TileBatcher batcher, Texture2D texture)
        {
            var rectangle = Rectangle;
            var hue = Vector3.One;
            batcher.DrawTile(texture, X, Y, ref rectangle, ref Normal0, ref Normal1, ref Normal2, ref Normal3, ref hue);
        }

        [MethodImpl(256)]
        private static void MergeAndNormalize(ref Vector3 v, float x, float y, float z)
        {
            var newX = v.Y * z - v.Z * y;
            var newY = v.Z * x - v.X * z;
            var newZ = v.X * y - v.Y * x;
            v.X = newX;
            v.Y = newY;
            v.Z = newZ;

            Vector3.Normalize(ref v, out v);
        }

        [MethodImpl(256)]
        private static void SumAndNormalize
        (
            ref Vector3[,,] vec,
            int index0_x,
            int index0_y,
            int index0_z,
            int index1_x,
            int index1_y,
            int index1_z,
            int index2_x,
            int index2_y,
            int index2_z,
            int index3_x,
            int index3_y,
            int index3_z,
            out Vector3 result
        )
        {
            Vector3.Add(
                ref vec[index0_x, index0_y, index0_z],
                ref vec[index1_x, index1_y, index1_z],
                out var v0Result);

            Vector3.Add(
                ref vec[index2_x, index2_y, index2_z],
                ref vec[index3_x, index3_y, index3_z],
                out var v1Result);

            Vector3.Add(ref v0Result, ref v1Result, out result);
            Vector3.Normalize(ref result, out result);
        }

        private void UpdateZ(sbyte currentZ, int zWest, int zEast, int zSouth)
        {
            if (IsStretched)
            {
                int x = (currentZ << 2) + 1;
                int y = zWest << 2;
                int w = (zEast << 2) - x;
                int h = (zSouth << 2) + 1 - y;

                Rectangle = new Rectangle(x, y, w, h);

                if (Math.Abs(currentZ - zEast) <= Math.Abs(zSouth - zWest))
                {
                    AverageZ = (sbyte)((currentZ + zEast) >> 1);
                }
                else
                {
                    AverageZ = (sbyte)((zSouth + zWest) >> 1);
                }

                MinimumZ = currentZ;

                if (zWest < MinimumZ)
                {
                    MinimumZ = (sbyte)zWest;
                }

                if (zEast < MinimumZ)
                {
                    MinimumZ = (sbyte)zEast;
                }

                if (zSouth < MinimumZ)
                {
                    MinimumZ = (sbyte)zSouth;
                }
            }
        }

        private static bool TestIfStretched(Map map, int x, int y, sbyte z, bool recursive)
        {
            bool isStretched = false;

            for (int i = -1; i < 2 && !isStretched; ++i)
            {
                for (int j = -1; j < 2 && !isStretched; ++j)
                {
                    if (recursive)
                    {
                        isStretched = TestIfStretched(map, x + i, y + j, z, false);
                    }
                    else
                    {
                        sbyte testZ = map.GetTileZ(x + i, y + j);
                        isStretched = testZ != z && testZ != Map.MinimumZ;
                    }
                }
            }

            return isStretched;
        }
    }
}
