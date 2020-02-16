using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using static MeshUtils;

public class Laser : MonoBehaviour {
    private Mesh mesh;
    private NativeArray<VertexData> verts;
    private MeshFilter mf;
    private MeshRenderer mr;
    private MaterialPropertyBlock pb;
    public Sprite sprite;
    private Vector2 spriteBounds;
    private int w = 100;
    private int h = 1;
    private float lifetime = 0f;
    public float updateStagger = 0.1f;
    private void Awake() {
        pb = new MaterialPropertyBlock();
        pb.SetTexture(Shader.PropertyToID("_MainTex"), sprite.texture);
        spriteBounds = sprite.bounds.size;
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();
        (mesh, verts) = MeshUtils.CreateMesh(h, w);
        mf.mesh = mesh;
        Draw();
        Commit();
    }

    private void Update() {
        lifetime += Time.deltaTime;
        Draw();
        Commit();
    }

    private unsafe void Draw() {
        Func<float, float, Vector2> drawFunc = (dt, lt) => new Vector2(dt, Mathf.Sin(dt + lt));
        int vw = w + 1;
        float sh = spriteBounds.y / 2f;
        var vertsPtr = (VertexData*)verts.GetUnsafePtr();
        
        vertsPtr[0].uv.x = vertsPtr[vw].uv.x = 0f; 
        
        Vector2 loc = drawFunc(0f, lifetime);
        float drawtime = updateStagger;
        Vector2 nextLoc = drawFunc(drawtime, lifetime);
        Vector2 delta = nextLoc - loc;
        Vector2 unit_d = delta.normalized;
        vertsPtr[0].loc.x = loc.x + sh * unit_d.y;
        vertsPtr[0].loc.y = loc.y + sh * -unit_d.x;
        vertsPtr[vw].loc.x = loc.x + sh * -unit_d.y;
        vertsPtr[vw].loc.y = loc.y + sh * unit_d.x;
        vertsPtr[0].uv.y = 0;
        vertsPtr[vw].uv.y = 1;
        for (int iw = 1; iw < vw; ++iw) {
            vertsPtr[iw].uv.x = vertsPtr[iw + vw].uv.x = vertsPtr[iw - 1].uv.x + delta.magnitude / spriteBounds.x;
            loc = nextLoc;
            vertsPtr[iw].loc.x = loc.x + sh * unit_d.y;
            vertsPtr[iw].loc.y = loc.y + sh * -unit_d.x;
            vertsPtr[iw + vw].loc.x = loc.x + sh * -unit_d.y;
            vertsPtr[iw + vw].loc.y = loc.y + sh * unit_d.x;
            vertsPtr[iw].uv.y = 0;
            vertsPtr[iw + vw].uv.y = 1;
            
            drawtime += updateStagger;
            nextLoc = drawFunc(drawtime, lifetime);
            delta = nextLoc - loc;
            unit_d = delta.normalized;
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