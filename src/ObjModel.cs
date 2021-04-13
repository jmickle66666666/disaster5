// loads obj files (badly)

using System;
using System.Numerics;
using System.IO;
using System.Collections.Generic;
using OpenGL;

namespace Disaster {

    public struct ObjVertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;

        public bool Equals(ObjVertex other)
        {
            return position == other.position && normal == other.normal && uv == other.uv;
        }
    }

    public struct ObjTriangle {
        public ObjVertex A;
        public ObjVertex B;
        public ObjVertex C;
    }

    public struct ObjModel {
        public string name;
        public VBO<Vector3> vertices;
        public VBO<Vector2> uvs;
        public VBO<Vector3> normals;
        public VBO<uint> triangles;
        public int hash;

        public void Dispose()
        {
            vertices.Dispose();
            uvs.Dispose();
            normals.Dispose();
            triangles.Dispose();
        }

        public static ObjModel Parse(string path)
        {
            string objName = "unnamed";
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();

            // the faces are lists of triplets of position, normal, uvs
            List<List<(int pos,int normal,int uv)>> faces = new List<List<(int pos,int normal,int uv)>>();

            // split the obj file into separate lines, since every line is a separate piece of data
            string[] lines = File.ReadAllLines(path);

            int numLines = lines.Length; 
            for (int i = 0; i < numLines; i++) {
                var line = lines[i];

                // split each line into space-separated tokens
                string[] tokens = line.Split(' ');

                // the first token defines what the data is supposed to read
                switch (tokens[0]) {
                    case "o":
                        if (tokens.Length != 2) Console.WriteLine($"Unexpected number of tokens on line {i}");
                        // TODO: do something with like. separate objects
                        objName = tokens[1];
                        break;

                    // position
                    case "v":
                        if (tokens.Length != 4) Console.WriteLine($"Unexpected number of tokens on line {i}");

                        // read out the x y z positions from the tokens
                        float vx, vy, vz;
                        if (
                            float.TryParse(tokens[1], out vx) && 
                            float.TryParse(tokens[2], out vy) && 
                            float.TryParse(tokens[3], out vz)
                        ) {
                            vertices.Add(new Vector3(vx, vy, vz));
                        } else {
                            Console.WriteLine($"Failed to parse vertex: {line}");
                        }
                        break;

                    // uv
                    case "vt":
                        if (tokens.Length != 3) Console.WriteLine($"Unexpected number of tokens on line {i}");
                        float vtx, vty;
                        if (
                            float.TryParse(tokens[1], out vtx) && 
                            float.TryParse(tokens[2], out vty)
                        ) {
                            uvs.Add(new Vector2(vtx, 1.0f - vty));
                        } else {
                            Console.WriteLine($"Failed to parse uv: {line}");
                        }
                        break;

                    // normal
                    case "vn":
                        if (tokens.Length != 4) Console.WriteLine($"Unexpected number of tokens on line {i}");
                        float vnx, vny, vnz;
                        if (
                            float.TryParse(tokens[1], out vnx) && 
                            float.TryParse(tokens[2], out vny) && 
                            float.TryParse(tokens[3], out vnz)
                        ) {
                            normals.Add(new Vector3(vnx, vny, vnz));
                        } else {
                            Console.WriteLine($"Failed to parse normal: {line}");
                        }
                        break;
                    
                    // face
                    case "f":
                        if (tokens.Length < 4) Console.WriteLine($"Unexpected number of tokens on line {i}");

                        List<(int,int,int)> faceVerts = new List<(int, int, int)>();

                        for (int j = 1; j < tokens.Length; j++) {

                            // our tokens now look like "5/63/2", so split on the slashes. (it could also be just "5" or "5/63")
                            var vert = tokens[j].Split('/');

                            bool failed = false;
                            int vertVert;
                            int vertUV = -1;
                            int vertNormal = -1;

                            // this ugly tree is a) checking *if* it contains normals and uvs, and b) making sure the format is correct
                            if (int.TryParse(vert[0], out vertVert)) {
                                if (vert.Length > 1) {
                                    if (int.TryParse(vert[1], out vertUV)) {
                                        if (vert.Length > 2) {
                                            if (int.TryParse(vert[2], out vertNormal)) {

                                            } else { failed = true; }
                                        }
                                    } else { failed = true; }
                                }
                            } else { failed = true; }

                            // do the error checking now
                            if (failed) {
                                Console.WriteLine($"Failed to parse : {tokens[j]}");
                                break;
                            }

                            // indices are 1-indexed in obj format, so we subtract one.
                            // oops, just noticed i'm not actually handling the case for when it only provides position, or just position and uv etc
                            faceVerts.Add(((vertVert-1), (vertNormal-1), (vertUV-1)));
                        }

                        // add our new list of vertex/normal/uv to the list
                        faces.Add(faceVerts);

                        break;

                    // i don't use any of these tokens, but they are expected to come up
                    case "mtllib":
                    case "usemtl":
                    case "s":
                    case "g":
                    case "#":
                    case "l":
                    case "":
                    case " ":
                        // expected nothing
                        break;

                    // if any *other* tokens show up, something is wrong!
                    default:
                        // unexpected
                        Console.WriteLine($"Unexpected token in obj parse. File: {path} Line: {i} Token: {tokens[0]}");
                        break;

                }
            }

            // ObjVertex is just a container for all three of Position, Normal and UV
            List<ObjVertex> objVertices = new List<ObjVertex>();

            // this is our output list of triangle indices
            List<uint> indices = new List<uint>();

            foreach (var f in faces) {

                // this is where i only look for the first 3 verts in a face, since i only care about triangles
                // i make sure to triangulate my models before exporting.
                for (int i = 0; i < 3; i++) {
                    bool found = false;
                    uint index = 0;
                    Vector3 position = vertices[f[i].pos];
                    Vector3 normal = normals[f[i].normal];
                    Vector2 uv = uvs[f[i].uv];
                    
                    // create a new ObjVertex from each face, by looking up the indices
                    ObjVertex objVertex = new ObjVertex() {
                        position = position,
                        normal = normal,
                        uv = uv
                    };

                    // take a look to see if this one already exists
                    // this is optional! you could just not care about duplicated as long as your meshes aren't super huge
                    for (int o = 0; o < objVertices.Count; o++) {
                        if (objVertices[o].Equals(objVertex)) {
                            index = (uint)o;
                            found = true;
                            break;
                        }
                    }

                    // if its unique (or you skip the previous step), store the new ObjVertex
                    if (!found) {
                        index = (uint)objVertices.Count;
                        objVertices.Add(objVertex);
                    }

                    // add it's index to the new list of triangle indices
                    indices.Add(index);
                }
            }

            // now just replace the lists with the ObjVertices list
            vertices.Clear();
            uvs.Clear();
            normals.Clear();

            for (int i = 0; i < objVertices.Count; i++)
            {
                vertices.Add(objVertices[i].position);
                uvs.Add(objVertices[i].uv);
                normals.Add(objVertices[i].normal);
            }

            // convert to VBOs for opengl to play with
            var output = new ObjModel () {
                vertices = new VBO<Vector3>(vertices.ToArray()),
                triangles = new VBO<uint>(indices.ToArray(), BufferTarget.ElementArrayBuffer),
                normals = new VBO<Vector3>(normals.ToArray()),
                uvs = new VBO<Vector2>(uvs.ToArray()),
                name = objName
            };
            output.hash = output.GetHashCode();
            return output;
        }
    }

}