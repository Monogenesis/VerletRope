using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Point : MonoBehaviour
{
    public Vector2 position, prevPosition;
    public Stick stick;
    public bool locked;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    private void Start()
    {
        RopeController.points.Add(this);
        position = transform.position;
        // meshFilter = GetComponent<MeshFilter>();
        // meshFilter.mesh = new List<Mesh>(Resources.FindObjectsOfTypeAll<Mesh>()).Find((mesh) => mesh.name == "Sphere");
        this.name = "Bead";

    }
    private void FixedUpdate()
    {
        transform.position = position;
    }
}
