using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public static List<Point> points = new List<Point>();
    public static List<Stick> sticks = new List<Stick>();
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private int segments = 30;
    [SerializeField] private float segmentLength = 0.1f;
    [SerializeField] private float maxVelocity = 1.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float drag = 0.1f;
    [SerializeField, Range(0.0f, 180.0f)] private float stiffness = 60.0f;
    [SerializeField, Range(1, 100)] private int elasticity = 20;

    private void Start()
    {
        CreateRope();
    }


    private void FixedUpdate()
    {
        Simulate();
    }

    void CreateRope()
    {

        Point anker = new GameObject().AddComponent<Point>();
        anker.locked = true;
        Point prevPoint = anker;
        for (int i = 1; i <= segments; i++)
        {
            Stick stick = new GameObject().AddComponent<Stick>();
            prevPoint.stick = stick;
            stick.length = segmentLength;
            stick.pointA = prevPoint;
            stick.pointA.position = new Vector2(transform.position.x, transform.position.y) + new Vector2(0.0f, i * segmentLength);
            stick.pointA.transform.position = stick.pointA.position;
            Point nextP = new GameObject().AddComponent<Point>();
            nextP.stick = stick;
            stick.pointB = nextP;
            stick.pointB.position = new Vector2(transform.position.x, transform.position.y) + new Vector2(0.0f, i * segmentLength * 2);
            stick.pointB.transform.position = stick.pointB.position;
            prevPoint = nextP;
        }

    }

    void Simulate()
    {
        MoveRopeSegments();


        for (int i = 0; i < elasticity; i++)
        {
            DistanceConstraint();
            StiffnessConstraint();
        }
    }
    private void MoveRopeSegments()
    {
        foreach (Point p in points)
        {
            if (!p.locked)
            {
                Vector2 positionBeforeUpdate = p.position;
                Vector2 vel = p.position - p.prevPosition;
                vel += Vector2.down * gravity * Time.deltaTime * Time.deltaTime; // Gravity
                vel += -vel * drag * Time.deltaTime; // Drag
                vel = Vector2.ClampMagnitude(vel, maxVelocity * Time.deltaTime);
                p.position += vel;
                p.prevPosition = positionBeforeUpdate;
            }
        }
    }

    private void StiffnessConstraint()
    {
        for (int s = 0; s < sticks.Count; s++)
        {
            if (sticks.Count > s + 1)
            {
                Stick stick = sticks[s];
                Stick nextStick = sticks[s + 1];

                if (!nextStick.pointB.locked)
                {
                    Vector2 prevToCurrent = (stick.pointB.position - stick.pointA.position).normalized;
                    Vector2 currentToNext = (nextStick.pointB.position - stick.pointB.position).normalized;
                    float angleCurrentStickNextStick = Vector2.Angle(prevToCurrent, currentToNext);

                    if (angleCurrentStickNextStick > stiffness)
                    {

                        bool onLeftSide = Vector2.SignedAngle(prevToCurrent, currentToNext) > 0 ? true : false;
                        float desiredAngle = Vector2.SignedAngle(prevToCurrent, Vector2.right) + (onLeftSide ? -stiffness : stiffness);
                        float x = Mathf.Cos(desiredAngle * Mathf.Deg2Rad);
                        float y = Mathf.Sin(desiredAngle * Mathf.Deg2Rad);
                        Vector2 newDir = new Vector2(x, -y);
                        nextStick.pointB.position = stick.pointB.position + newDir * segmentLength;
                        // nextStick.pointB.prevPosition = Vector2.Lerp(nextStick.pointB.prevPosition, nextStick.pointB.position, Time.deltaTime * 10);
                        nextStick.pointB.prevPosition = nextStick.pointB.position;


                    }
                }
            }

        }

    }
    private void DistanceConstraint()
    {
        foreach (Stick stick in sticks)
        {
            // Distance Constraint
            Vector2 stickCenter = (stick.pointA.position + stick.pointB.position) / 2.0f;
            Vector2 stickDir = (stick.pointA.position - stick.pointB.position).normalized;
            if (!stick.pointA.locked)
            {
                stick.pointA.position = stickCenter + stickDir * stick.length / 2.0f;
            }
            if (!stick.pointB.locked)
            {
                stick.pointB.position = stickCenter - stickDir * stick.length / 2.0f;
            }
        }
    }
}
