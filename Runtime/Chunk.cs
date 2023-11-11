using System;
using System.Linq;
using UnityEngine;

namespace TileVoxelRenderer
{
    public class Chunk
    {
        public Vector2 pos;
        public Matrix4x4 matrix;
        public Mesh mesh;

        public static int size;
        public static int vertCount;
        public static int indexByLocalPos(Vector2 pos) { return ((int)Math.Floor(pos.y)) * size + (int)Math.Floor(pos.x); }
        public static RenderParams RP;
        public static Mesh plane;

        public static void CreatePlane(int setSize)
        {
            size = setSize;
            vertCount = size * size + size * 2 + 1;
            Vector3[] verts = new Vector3[vertCount];
            int[] tris = new int[(size * size) * 6];

            int index, vert, tri;
            Vector2 P = new Vector2();
            for (int y = 0; y <= size; y++)
                for (int x = 0; x <= size; x++)
                {
                    P.x = x;
                    P.y = y;
                    index = indexByLocalPos(P);
                    vert = (int)Math.Floor(P.y) * (size + 1) + (int)Math.Floor(P.x);
                    verts[vert] = new Vector3(P.x, 0, P.y);
                    #region DebugVerts
                    //GameObject loc = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //loc.transform.position = P + pos;
                    //loc.transform.localScale = Vector3.one * 0.1f;
                    //loc.name = String.Format("P: {0}, I: {1}", pos, vert.ToString());
                    #endregion
                    //index % (size + 1) == size means we're at the and of the row.
                    if (index % (size + 1) == size)
                        continue;
                    tri = index - index / (size + 1);
                    //We need a extra row of verts for the top of the quads. This just makes sure we don't try to make quads for the top layer
                    if (tri > size * size - 1)
                        continue;
                    tris[tri * 6] = index;
                    tris[tri * 6 + 1] = index + size + 1;
                    tris[tri * 6 + 2] = index + 1;
                    tris[tri * 6 + 3] = index + 1;
                    tris[tri * 6 + 4] = index + size + 1;
                    tris[tri * 6 + 5] = index + size + 2;
                }

            plane = new Mesh();
            plane.vertices = verts;
            plane.triangles = tris;
        }

        public Chunk(Vector2 pos, float[] verts, Vector2[] uvs)
        {
            this.pos = pos;
            matrix = Matrix4x4.Translate(new Vector3(pos.x, 0, pos.y));
            GenerateMesh(verts, uvs);
        }

        public void GenerateMesh(float[] vertHeight, Vector2[] uvs)
        {
            if (vertHeight.Length != vertCount || uvs.Length != vertCount)
            {
                Debug.LogError("verts or uvs not the right count");
                return;
            }

            mesh = new Mesh();
            int index = 0;
            Vector3[] verts = plane.vertices;
            foreach (Vector3 v in plane.vertices)
                verts[index].y = vertHeight[index++];
            mesh.vertices = verts;
            mesh.triangles = plane.triangles;
            mesh.uv = uvs;
        }

        public void Display()
        {
            Graphics.RenderMesh(RP, mesh, 0, matrix);
        }
    }
}
