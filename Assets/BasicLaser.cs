using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static MeshUtils;

public class BasicLaser : MonoBehaviour {
    private Mesh mesh;
    private NativeArray<VertexData> verts;
    private MeshFilter mf;
    private MeshRenderer mr;
    private MaterialPropertyBlock pb;
    public Sprite sprite;
    private int w = 100;
    private int h = 1;
    private void Awake() {
        pb = new MaterialPropertyBlock();
        pb.SetTexture(Shader.PropertyToID("_MainTex"), sprite.texture);
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();
        (mesh, verts) = MeshUtils.CreateMesh(h, w);
        mf.mesh = mesh;
        Draw();
        Commit();
    }

    private void Update() {
        Draw();
        Commit();
    }

    private void Draw() {
        int vw = w + 1;
        for (int iw = 0; iw <= w; ++iw) {
            for (int ih = 0; ih <= h; ++ih) {
                var v = verts[0];
                v.loc.x = iw / 10.0f;
                v.loc.y = ih;
                v.uv.x = iw / 40.0f;
                v.uv.y = ih;
                verts[iw + ih * vw] = v;
            }
        }
    }

    private void Commit() {
        mesh.SetVertexBufferData(verts, 0, 0, verts.Length);
        mr.SetPropertyBlock(pb);
    }

    private void OnDestroy() {
        verts.Dispose();
    }
}