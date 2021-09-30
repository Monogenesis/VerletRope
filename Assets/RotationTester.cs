using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTester : MonoBehaviour
{


    public Transform pointA;
    public Transform pointB;
    public Transform pointC;

    public float stiffness = 45.0f;


    // public float angle;


    void Update()
    {
        Vector2 dirAB = (pointB.position - pointA.position).normalized;
        Vector2 dirBC = (pointC.position - pointB.position).normalized;
        float dirABBCAngle = Vector2.Angle(dirAB, dirBC);
        // Debug.Log(Vector2.Angle(dirAB, dirBC));
        // Debug.Log(Vector2.SignedAngle(dirAB, dirBC));
        if (dirABBCAngle > stiffness)
        {
            float desiredAngle;
            bool onLeftSide = Vector2.SignedAngle(dirAB, dirBC) > 0 ? true : false;
            Debug.Log($"On left Side: {onLeftSide}");
            desiredAngle = Vector2.SignedAngle(dirAB, Vector2.right) + (onLeftSide ? -stiffness : stiffness);
            // desiredAngle *= -1;
            Debug.Log($"DirBCRight: {Vector2.SignedAngle(dirBC, Vector2.right)}");
            Debug.Log($"DirAB and RIght angle {Vector2.SignedAngle(dirAB, Vector2.right)}");
            Debug.Log($"Desired angle {desiredAngle}");
            float x = Mathf.Cos(desiredAngle * Mathf.Deg2Rad);
            float y = Mathf.Sin(desiredAngle * Mathf.Deg2Rad);
            Vector2 newDir = new Vector2(x, -y);
            Debug.Log($"Corrected position: {newDir}");
            // pointC.position = pointB.position + new Vector3(newDir.x, newDir.y, 0f) * Vector3.Distance(pointB.position, pointC.position);
            pointC.position = pointB.position + new Vector3(newDir.x, newDir.y, 0f) * 5f;
        }


        Debug.DrawLine(pointA.position, pointA.position + new Vector3(dirAB.x, dirAB.y, 0.0f) * 10.0f);
        Debug.DrawLine(pointB.position, pointC.position);
        Debug.DrawLine(pointB.position, Vector3.right * 10.0f);
    }
}
