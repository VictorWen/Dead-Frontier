using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionScript : MonoBehaviour
{
    [Range(3, 1000)]
    [SerializeField] private int numRays = 100;
    [SerializeField] private int range = 10;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private void Start() {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        vertices = new Vector3[numRays + 1];
        uv = new Vector2[vertices.Length];
        triangles = new int[numRays * 3];

        vertices[numRays] = Vector3.zero;

        // DrawView();
    }

    private void Update() {
        DrawView();
    }

    private void DrawView() {
        for (int i = 0; i < numRays; i++) {
            float angle = 2 * Mathf.PI * i / numRays;
            Vector2 ray = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * range;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, range);
            if (hit.collider != null) {
                ray = hit.point - new Vector2(transform.position.x, transform.position.y);
            }
            vertices[i] = ray;
        }

        for (int i = 0; i < numRays; i++) {
            int j = i * 3;
            triangles[j + 0] = numRays;
            triangles[j + 1] = (i+1) % numRays;
            triangles[j + 2] = i;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
