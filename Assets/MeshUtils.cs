using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public static class MeshUtils {
    private static readonly VertexAttributeDescriptor[] layout = {
        new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3), 
        new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2), 
    };
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexData {
        public Vector3 loc;
        public Vector2 uv;
    }
    
    public static (Mesh, NativeArray<VertexData>) CreateMesh(int h, int w) {
        int numVerts = (h + 1) * (w + 1);
        int[] tris = WHTris(h, w);
        var mesh = new Mesh();
        mesh.SetVertexBufferParams(numVerts, layout);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);
        mesh.triangles = tris;
        return (mesh, new NativeArray<VertexData>(numVerts, Allocator.Persistent));
    }
    
    public static int[] WHTris(int h, int w) {
        int vw = w + 1;
        int[] tris = new int[2 * h * w * 3];
        for (int ih = 0; ih < h; ++ih) {
            for (int iw = 0; iw < w; ++iw) {
                int it = 2 * (w * ih + iw) * 3;
                int iv = ih * vw + iw;
                tris[it + 0] = iv;
                tris[it + 1] = iv + vw + 1;
                tris[it + 2] = iv + 1;
                
                tris[it + 3] = iv + vw + 1;
                tris[it + 4] = iv;
                tris[it + 5] = iv + vw;
            }
        }
        return tris;
    }
}