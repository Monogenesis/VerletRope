using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public Point pointA, pointB;
    public float length;

    private void Start()
    {
        RopeController.sticks.Add(this);
        this.name = "Segment";
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(pointA.position, pointB.position, Color.red);
    }
}
