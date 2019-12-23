using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    public enum FaceRenderMask { All, Top, Right, Left, Bottom };

    [Range(0, 320)]
    public int divisions = 7;
    public bool autoUpdate = true;
    public FaceRenderMask faceRenderMask;
    public Vector3 center;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    PlanetFace[] planetFaces;
    Vector3[] faceVertices;

    private const int NUM_FACES = 20;

    public void GeneratePlanet() {
        Initialize();
        GenerateMeshes();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated() {
        if (autoUpdate) {
            Initialize();
            GenerateMeshes();
        }
    }

    public void OnColorSettingsUpdated() {
        if (autoUpdate) {
            Initialize();
            GenerateColors();
        }
    }

    public void GenerateMeshes() {
        for (int i = 0; i < NUM_FACES; i++) {
            if (meshFilters[i].gameObject.activeSelf) {
                MeshData meshData = GenerateMeshData(planetFaces[i]);
                planetFaces[i].OnMeshDataReceived(meshData);
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    public void GenerateColors() {
        colorGenerator.UpdateColors();
        for (int i = 0; i < NUM_FACES; i++) {
            if (meshFilters[i].gameObject.activeSelf) {
                MeshData meshData = GenerateUVData(planetFaces[i]);
                planetFaces[i].OnMeshDataReceived(meshData);
            }
        }
    }

    MeshData GenerateMeshData(PlanetFace face) {
        MeshData meshData = face.GetMeshData();
        int numLevels = divisions + 2;
        int numVertices = numLevels * (numLevels + 1) / 2;
        int numTriangles = (divisions + 1) * (divisions + 1);
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[3 * numTriangles];
        Vector2[] uv = meshData.uv.Length == numVertices ? meshData.uv : new Vector2[numVertices];

        for (int y = 0, vertIndex = 0, triIndex = 0; y < numLevels; y++) {
            float yFraction = (float)y / (numLevels - 1);
            Vector3 rowInit = face.top + yFraction * (face.left - face.top);

            for (int x = 0; x < y + 1; x++) {
                float xFraction = (float)x / (numLevels - 1);
                Vector3 point = rowInit + xFraction * (face.right - face.left);
                Vector3 normalizedPoint = point.normalized;
                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(normalizedPoint);
                vertices[vertIndex] = normalizedPoint * shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[vertIndex].y = unscaledElevation;

                if (y < numLevels - 1) {
                    triangles[triIndex++] = vertIndex;
                    triangles[triIndex++] = vertIndex + y + 2;
                    triangles[triIndex++] = vertIndex + y + 1;

                    if (x < y) {
                        triangles[triIndex++] = vertIndex;
                        triangles[triIndex++] = vertIndex + 1;
                        triangles[triIndex++] = vertIndex + y + 2;
                    }
                }

                vertIndex++;
            }
        }
        return new MeshData(vertices, triangles, uv);
    }

    MeshData GenerateUVData(PlanetFace face) {
        MeshData meshData = face.GetMeshData();
        int numLevels = divisions + 2;
        Vector2[] uv = meshData.uv;

        for (int y = 0, vertIndex = 0; y < numLevels; y++) {
            float yFraction = (float)y / (numLevels - 1);
            Vector3 rowInit = face.top + yFraction * (face.left - face.top);

            for (int x = 0; x < y + 1; x++) {
                float xFraction = (float)x / (numLevels - 1);
                Vector3 point = rowInit + xFraction * (face.right - face.left);

                uv[vertIndex].x = colorGenerator.BiomePercentageFromPoint(point.normalized);
                vertIndex++;
            }
        }

        return new MeshData(meshData.vertices, meshData.triangles, uv);
    }

    void Initialize() {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (faceVertices == null || faceVertices.Length == 0) {
            CalculateFaceVertices();
        }
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[NUM_FACES];
        }
        planetFaces = new PlanetFace[NUM_FACES];
        if (transform.Find("center") == null) {
            GameObject centerPoint = new GameObject("center");
            centerPoint.transform.parent = transform;
            centerPoint.transform.position = center;
        }

        for (int i = 0; i < NUM_FACES; i++) {
            if (meshFilters[i] == null) {
                GameObject meshObj = new GameObject("mesh#" + i);
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i / 5;
            meshFilters[i].gameObject.SetActive(renderFace);
        }

        planetFaces[0] = new PlanetFace(meshFilters[0].sharedMesh, faceVertices[0], faceVertices[2], faceVertices[1]);
        planetFaces[1] = new PlanetFace(meshFilters[1].sharedMesh, faceVertices[0], faceVertices[3], faceVertices[2]);
        planetFaces[2] = new PlanetFace(meshFilters[2].sharedMesh, faceVertices[0], faceVertices[4], faceVertices[3]);
        planetFaces[3] = new PlanetFace(meshFilters[3].sharedMesh, faceVertices[0], faceVertices[5], faceVertices[4]);
        planetFaces[4] = new PlanetFace(meshFilters[4].sharedMesh, faceVertices[0], faceVertices[1], faceVertices[5]);

        planetFaces[5] = new PlanetFace(meshFilters[5].sharedMesh, faceVertices[1], faceVertices[2], faceVertices[9]);
        planetFaces[6] = new PlanetFace(meshFilters[6].sharedMesh, faceVertices[8], faceVertices[1], faceVertices[9]);
        planetFaces[7] = new PlanetFace(meshFilters[7].sharedMesh, faceVertices[9], faceVertices[2], faceVertices[10]);
        planetFaces[8] = new PlanetFace(meshFilters[8].sharedMesh, faceVertices[2], faceVertices[3], faceVertices[10]);
        planetFaces[9] = new PlanetFace(meshFilters[9].sharedMesh, faceVertices[10], faceVertices[3], faceVertices[6]);

        planetFaces[10] = new PlanetFace(meshFilters[10].sharedMesh, faceVertices[3], faceVertices[4], faceVertices[6]);
        planetFaces[11] = new PlanetFace(meshFilters[11].sharedMesh, faceVertices[6], faceVertices[4], faceVertices[7]);
        planetFaces[12] = new PlanetFace(meshFilters[12].sharedMesh, faceVertices[4], faceVertices[5], faceVertices[7]);
        planetFaces[13] = new PlanetFace(meshFilters[13].sharedMesh, faceVertices[7], faceVertices[5], faceVertices[8]);
        planetFaces[14] = new PlanetFace(meshFilters[14].sharedMesh, faceVertices[5], faceVertices[1], faceVertices[8]);

        planetFaces[15] = new PlanetFace(meshFilters[15].sharedMesh, faceVertices[11], faceVertices[6], faceVertices[7]);
        planetFaces[16] = new PlanetFace(meshFilters[16].sharedMesh, faceVertices[11], faceVertices[7], faceVertices[8]);
        planetFaces[17] = new PlanetFace(meshFilters[17].sharedMesh, faceVertices[11], faceVertices[8], faceVertices[9]);
        planetFaces[18] = new PlanetFace(meshFilters[18].sharedMesh, faceVertices[11], faceVertices[9], faceVertices[10]);
        planetFaces[19] = new PlanetFace(meshFilters[19].sharedMesh, faceVertices[11], faceVertices[10], faceVertices[6]);
    }

    void CalculateFaceVertices() {
        faceVertices = new Vector3[12];
        float edgeLength = 0.5f;

        float t2 = Mathf.PI / 10f;
        float t4 = Mathf.PI / 5f;
        float R = (edgeLength * 0.5f) / Mathf.Sin(t4);
        float H = Mathf.Cos(t4) * R;
        float Cx = R * Mathf.Sin(t2);
        float Cz = R * Mathf.Cos(t2);
        float H1 = Mathf.Sqrt(edgeLength * edgeLength - R * R);
        float H2 = Mathf.Sqrt((H + R) * (H + R) - H * H);
        float Y2 = (H2 - H1) * 0.5f;
        float Y1 = Y2 + H1;

        faceVertices[0] = new Vector3(0, Y1, 0);
        faceVertices[1] = new Vector3(R, Y2, 0);
        faceVertices[2] = new Vector3(Cx, Y2, Cz);
        faceVertices[3] = new Vector3(-H, Y2, edgeLength * 0.5f);
        faceVertices[4] = new Vector3(-H, Y2, -edgeLength * 0.5f);
        faceVertices[5] = new Vector3(Cx, Y2, -Cz);
        faceVertices[6] = new Vector3(-R, -Y2, 0);
        faceVertices[7] = new Vector3(-Cx, -Y2, -Cz);
        faceVertices[8] = new Vector3(H, -Y2, -edgeLength * 0.5f);
        faceVertices[9] = new Vector3(H, -Y2, edgeLength * 0.5f);
        faceVertices[10] = new Vector3(-Cx, -Y2, Cz);
        faceVertices[11] = new Vector3(0, -Y1, 0);
    }

    private void Start() {
        GeneratePlanet();

    }

    private void Update() {
        transform.RotateAround(center, Vector3.up, 40 * Time.deltaTime);
    }
}
