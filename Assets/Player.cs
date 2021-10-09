using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rope rope;

    void Start()
    {
        rope = new Rope();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rope.CreateRope(Vector3.zero, 10, 5);
        }
        if (Input.GetKey(KeyCode.K))
        {
            rope.FirstPoint.position += Vector3.right * 2 * Time.deltaTime;
        }
        if (rope.LastPoint != null)
        {
            transform.position = rope.LastPoint.position;
        }
    }

    private void FixedUpdate()
    {
        rope.Simulate();
    }

    private void OnDrawGizmos()
    {
        rope.DrawRope();
    }
}
