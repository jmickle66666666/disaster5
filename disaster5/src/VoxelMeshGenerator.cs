using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;
using System;

namespace Disaster
{
    public struct MultiTextureVoxel
    {
        public int top;
        public int bottom;
        public int forward;
        public int backward;
        public int left;
        public int right;

        public MultiTextureVoxel(int top, int bottom, int forward, int backward, int left, int right)
        {
            this.top = top;
            this.bottom = bottom;
            this.forward = forward;
            this.backward = backward;
            this.left = left;
            this.right = right;
        }

        public static MultiTextureVoxel TopBottom(int topBottom, int sides)
        {
            return new MultiTextureVoxel(topBottom, topBottom, sides, sides, sides, sides);
        }

        public static MultiTextureVoxel FrontBack(int frontBack, int sides)
        {
            return new MultiTextureVoxel(sides, sides, frontBack, frontBack, sides, sides);
        }

        public static MultiTextureVoxel LeftRight(int leftRight, int sides)
        {
            return new MultiTextureVoxel(sides, sides, sides, sides, leftRight, leftRight);
        }
    }

    public class VoxelMeshGenerator
    {
        public static Model Generate(Vector3Int chunkSize, Texture2D texture, Vector2Int textureSize, int[] data, MultiTextureVoxel[] multiTextures)
        {
            int textureNumWidth = texture.width / textureSize.x;
            int textureNumHeight = texture.height / textureSize.y;
            int totalTextures = textureNumWidth * textureNumHeight;

            Vector2 uvSize = new Vector2(
                1f / textureNumWidth,
                1f / textureNumHeight
            );

            bool CheckSide(int x, int y, int z)
            {
                if (x < 0 || x >= chunkSize.x || y < 0 || y >= chunkSize.y || z < 0 || z >= chunkSize.z) return true; // are chunk border sides visible
                int index = x + (y * chunkSize.x) + (z * chunkSize.x * chunkSize.y);
                return data[index] == -1;
            }

            Vector2 indexToUV(int index, int side)
            {
                // side == left, right, top, bottom, front, back
                if (index >= totalTextures)
                {
                    index -= totalTextures;
                    MultiTextureVoxel mtv = multiTextures[index];
                    if (side == 0) index = mtv.left;
                    if (side == 1) index = mtv.right;
                    if (side == 2) index = mtv.top;
                    if (side == 3) index = mtv.bottom;
                    if (side == 4) index = mtv.forward;
                    if (side == 5) index = mtv.backward;
                }

                int indexX = index % textureNumWidth;
                int indexY = index / textureNumHeight;
                return new Vector2(
                    indexX * uvSize.X,
                    indexY * uvSize.Y
                );
            }

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indices = new List<int>();

            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] == -1) continue;
                int x = i % chunkSize.x;
                int yz = i / chunkSize.x;
                int y = yz % chunkSize.y;
                int z = yz / chunkSize.y;

                int vertexIndex;
                // left
                if (CheckSide(x - 1, y, z))
                {
                    vertexIndex = vertices.Count;
                    vertices.AddRange(new Vector3[]
                    {
                        new Vector3(x, y, z),
                        new Vector3(x, y+1, z),
                        new Vector3(x, y+1, z+1),
                        new Vector3(x, y, z+1),
                    });

                    indices.AddRange(new int[]
                    {
                        vertexIndex + 0,
                        vertexIndex + 2,
                        vertexIndex + 1,
                        vertexIndex + 0,
                        vertexIndex + 3,
                        vertexIndex + 2,
                    });

                    Vector2 baseUV = indexToUV(data[i], 0);
                    uvs.AddRange(new Vector2[]
                    {
                        new Vector2(baseUV.X, baseUV.Y + uvSize.Y),
                        baseUV,
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y + uvSize.Y),
                    });

                    normals.AddRange(new Vector3[]
                    {
                        -Vector3.UnitX,
                        -Vector3.UnitX,
                        -Vector3.UnitX,
                        -Vector3.UnitX
                    });
                }

                // right
                if (CheckSide(x + 1, y, z))
                {
                    vertexIndex = vertices.Count;
                    vertices.AddRange(new Vector3[]
                    {
                        new Vector3(x+1, y, z),
                        new Vector3(x+1, y+1, z),
                        new Vector3(x+1, y+1, z+1),
                        new Vector3(x+1, y, z+1),
                    });

                    indices.AddRange(new int[]
                    {
                        vertexIndex + 0,
                        vertexIndex + 1,
                        vertexIndex + 2,
                        vertexIndex + 0,
                        vertexIndex + 2,
                        vertexIndex + 3,
                    });

                    Vector2 baseUV = indexToUV(data[i], 1);
                    uvs.AddRange(new Vector2[]
                    {
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y),
                        baseUV,
                        new Vector2(baseUV.X, baseUV.Y + uvSize.Y),
                    });

                    normals.AddRange(new Vector3[]
                    {
                        Vector3.UnitX,
                        Vector3.UnitX,
                        Vector3.UnitX,
                        Vector3.UnitX
                    });
                }

                // top
                if (CheckSide(x, y + 1, z))
                {
                    vertexIndex = vertices.Count;
                    vertices.AddRange(new Vector3[]
                    {
                        new Vector3(x, y+1, z),
                        new Vector3(x+1, y+1, z),
                        new Vector3(x+1, y+1, z+1),
                        new Vector3(x, y+1, z+1),
                    });

                    indices.AddRange(new int[]
                    {
                        vertexIndex + 0,
                        vertexIndex + 2,
                        vertexIndex + 1,
                        vertexIndex + 0,
                        vertexIndex + 3,
                        vertexIndex + 2,
                    });

                    Vector2 baseUV = indexToUV(data[i], 2);
                    uvs.AddRange(new Vector2[]
                    {
                        baseUV,
                        new Vector2(baseUV.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y),
                    });

                    normals.AddRange(new Vector3[]
                    {
                        Vector3.UnitY,
                        Vector3.UnitY,
                        Vector3.UnitY,
                        Vector3.UnitY
                    });
                }

                // bottom
                if (CheckSide(x, y - 1, z))
                {
                    vertexIndex = vertices.Count;
                    vertices.AddRange(new Vector3[]
                    {
                        new Vector3(x, y, z),
                        new Vector3(x+1, y, z),
                        new Vector3(x+1, y, z+1),
                        new Vector3(x, y, z+1),
                    });

                    indices.AddRange(new int[]
                    {
                        vertexIndex + 0,
                        vertexIndex + 1,
                        vertexIndex + 2,
                        vertexIndex + 0,
                        vertexIndex + 2,
                        vertexIndex + 3,
                    });

                    Vector2 baseUV = indexToUV(data[i], 3);
                    uvs.AddRange(new Vector2[]
                    {
                        baseUV,
                        new Vector2(baseUV.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y),
                    });

                    normals.AddRange(new Vector3[]
                    {
                        -Vector3.UnitY,
                        -Vector3.UnitY,
                        -Vector3.UnitY,
                        -Vector3.UnitY
                    });
                }

                // forward
                if (CheckSide(x, y, z + 1))
                {
                    vertexIndex = vertices.Count;
                    vertices.AddRange(new Vector3[]
                    {
                        new Vector3(x, y, z+1),
                        new Vector3(x+1, y, z+1),
                        new Vector3(x+1, y+1, z+1),
                        new Vector3(x, y+1, z+1),
                    });

                    indices.AddRange(new int[]
                    {
                        vertexIndex + 0,
                        vertexIndex + 1,
                        vertexIndex + 2,
                        vertexIndex + 0,
                        vertexIndex + 2,
                        vertexIndex + 3,
                    });

                    Vector2 baseUV = indexToUV(data[i], 4);
                    uvs.AddRange(new Vector2[]
                    {
                        new Vector2(baseUV.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y),
                        baseUV,
                    });

                    normals.AddRange(new Vector3[]
                    {
                        Vector3.UnitZ,
                        Vector3.UnitZ,
                        Vector3.UnitZ,
                        Vector3.UnitZ
                    });
                }

                // backward
                if (CheckSide(x, y, z - 1))
                {
                    vertexIndex = vertices.Count;
                    vertices.AddRange(new Vector3[]
                    {
                        new Vector3(x, y, z),
                        new Vector3(x+1, y, z),
                        new Vector3(x+1, y+1, z),
                        new Vector3(x, y+1, z),
                    });

                    indices.AddRange(new int[]
                    {
                        vertexIndex + 0,
                        vertexIndex + 2,
                        vertexIndex + 1,
                        vertexIndex + 0,
                        vertexIndex + 3,
                        vertexIndex + 2,
                    });

                    Vector2 baseUV = indexToUV(data[i], 5);
                    uvs.AddRange(new Vector2[]
                    {
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y + uvSize.Y),
                        new Vector2(baseUV.X, baseUV.Y + uvSize.Y),
                        baseUV,
                        new Vector2(baseUV.X + uvSize.X, baseUV.Y),
                    });

                    normals.AddRange(new Vector3[]
                    {
                        -Vector3.UnitZ,
                        -Vector3.UnitZ,
                        -Vector3.UnitZ,
                        -Vector3.UnitZ
                    });
                }
            }

            Mesh output = new Mesh();

            output.vertexCount = vertices.Count;
            output.triangleCount = indices.Count / 3;
            output.vertices = TypeInterface.ArrayPointer(vertices.ToArray());

            short[] shortArray = new short[indices.Count];
            for (var i = 0; i < indices.Count; i++)
            {
                shortArray[i] = (short)indices[i];
            }

            output.indices = TypeInterface.ArrayPointer(shortArray);
            output.texcoords = TypeInterface.ArrayPointer(uvs.ToArray());
            //output.normals = TypeInterface.ArrayPointer(normals.ToArray());

            Raylib.UploadMesh(ref output, false);
            Model model = Raylib.LoadModelFromMesh(output);
            TypeInterface.SetMaterialTexture(ref model, 0, MaterialMapIndex.MATERIAL_MAP_DIFFUSE, ref texture);
            return model;
        }


    }
}