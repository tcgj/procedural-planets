using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace {

    public Vector3 top;
    public Vector3 left;
    public Vector3 right;
        
    Mesh mesh;

    /*
     * Defines a triangle mesh representing a icosahedron face.
     * Triangle is defined clockwise from the top vector a.
     */
    public PlanetFace(Mesh mesh, Vector3 a, Vector3 b, Vector3 c) {
        this.mesh = mesh;

        top = a;
        left = c;
        right = b;
    }

    public MeshData GetMeshData() {
        return new MeshData(mesh.vertices, mesh.triangles, mesh.uv);
    }

    public void OnMeshDataReceived(MeshData meshData) {
        meshData.UpdateMesh(mesh);
    }
}
