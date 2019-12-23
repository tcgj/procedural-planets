using System;
using UnityEngine;

public struct MeshData {
    public readonly Vector3[] vertices;
    public readonly int[] triangles;
    public readonly Vector2[] uv;

    public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uv) {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uv = uv;
    }

    public void UpdateMesh(Mesh mesh) {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }
}
