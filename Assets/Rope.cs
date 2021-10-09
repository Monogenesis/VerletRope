using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rope
{
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private float maxVelocity = 10.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float drag = 0.1f;
    [SerializeField, Range(0.0f, 180.0f)] private float stiffness = 60.0f; // TODO make the constraint more soft with lerp
    [SerializeField, Range(1, 100)] private int elasticity = 40;

    private Vector3 position;
    private Point firstPoint;
    private Point lastPoint;
    private List<Point> points = new List<Point>();
    private List<Stick> sticks = new List<Stick>();

    private bool ropeCreated;

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    public class Stick
    {
        public Point pointA;
        public Point pointB;
        public float length;
    }


    public class Point
    {
        public Vector3 position, prevPosition;
        public bool locked;
        public bool ankerPoint;
    }

    public Point FirstPoint
    { get => firstPoint; set => firstPoint = value; }

    public Point LastPoint
    { get => lastPoint; set => lastPoint = value; }


    public Rope(float gravity = 30.0f, float maxVelocity = 10.0f, float drag = 0.1f, float stiffness = 60.0f, int elasticity = 40)
    {
        this.gravity = gravity;
        this.maxVelocity = maxVelocity;
        this.drag = drag;
        this.stiffness = stiffness;
        this.elasticity = elasticity;
    }


    public void CreateRope(Vector3 position, int numberOfSegments, float totalLength)
    {
        float distancePerSegment = (float)(totalLength / numberOfSegments);
        RemoveRope();
        ropeCreated = true;
        Point anker = new Point();
        anker.ankerPoint = true;
        anker.locked = true;
        firstPoint = anker;
        Point prevPoint = anker;
        points.Add(anker);
        for (int i = 1; i <= numberOfSegments; i++)
        {
            Stick stick = new Stick();
            sticks.Add(stick);
            stick.length = distancePerSegment;
            stick.pointA = prevPoint;
            stick.pointA.position = new Vector3(position.x, position.y, position.z) + new Vector3(0.0f, -i * distancePerSegment, 0.0f);
            Point nextP = new Point();
            nextP.locked = false;
            stick.pointB = nextP;
            stick.pointB.position = new Vector3(position.x, position.y, position.z) + new Vector3(0.0f, -i * distancePerSegment * 2, 0.0f);
            prevPoint = nextP;
            lastPoint = nextP;
            points.Add(nextP);
        }

    }


    public void RemoveRope()
    {
        ropeCreated = false;
        points.Clear();
        sticks.Clear();
    }


    public void DrawRope()
    {
        if (ropeCreated)
            foreach (Stick stick in sticks)
            {
                Debug.DrawLine(stick.pointA.position, stick.pointB.position, Color.blue);
            }
    }


    public void Simulate()
    {
        if (ropeCreated)
        {
            MoveRopeSegments();


            for (int i = 0; i < elasticity; i++)
            {
                DistanceConstraint();
            }

            // StiffnessConstraint();
        }

    }


    private void MoveRopeSegments()
    {
        foreach (Point p in points)
        {
            if (!p.locked)
            {
                Vector3 positionBeforeUpdate = p.position;
                Vector3 vel = p.position - p.prevPosition;
                vel += Vector3.down * gravity * Time.deltaTime * Time.deltaTime; // Gravity
                vel += -vel * drag * Time.deltaTime; // Drag
                vel = Vector3.ClampMagnitude(vel, maxVelocity * Time.deltaTime);
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
                    Vector3 prevToCurrent = (stick.pointB.position - stick.pointA.position).normalized;
                    Vector3 currentToNext = (nextStick.pointB.position - stick.pointB.position).normalized;
                    float angleCurrentStickNextStick = Vector2.Angle(prevToCurrent, currentToNext);

                    if (angleCurrentStickNextStick > stiffness)
                    {

                        bool onLeftSide = Vector2.SignedAngle(prevToCurrent, currentToNext) > 0 ? true : false;
                        float desiredAngle = Vector2.SignedAngle(prevToCurrent, Vector2.right) + (onLeftSide ? -stiffness : stiffness);
                        float x = Mathf.Cos(desiredAngle * Mathf.Deg2Rad);
                        float y = Mathf.Sin(desiredAngle * Mathf.Deg2Rad);
                        float z = Mathf.Tan(desiredAngle * Mathf.Deg2Rad);
                        Vector3 newDir = new Vector3(x, -y, z);
                        nextStick.pointB.position = stick.pointB.position + newDir * stick.length;
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
            Vector3 stickCenter = (stick.pointA.position + stick.pointB.position) / 2.0f;
            Vector3 stickDir = (stick.pointA.position - stick.pointB.position).normalized;
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
