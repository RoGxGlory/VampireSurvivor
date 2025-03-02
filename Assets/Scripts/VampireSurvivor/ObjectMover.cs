using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    public void SetMovement(Vector3 moveDirection, float moveSpeed)
    {
        direction = moveDirection.normalized;
        speed = moveSpeed;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
