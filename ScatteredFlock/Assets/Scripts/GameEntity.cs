using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    public Level level;
    public bool active = false;

    public float maxSpeed;
    public float acceleration = 3.0f;
    public float attenuation = -0.5f;
    public float rotationSpeed;

    public Vector3 currentHeading;

    public bool isAtracting = false;
    public bool isThreatening = false;
    public float influenceRadius = 6.0f;
    public float avoidRadius = 0.5f;

    public float speed = 0;
    protected Vector3 lastPos = Vector3.zero;

    public virtual void DeltaUpdate(float dt)
    {
        if (active)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
                lastPos = transform.position;
            }
            Move(dt);
            CalculateHeading(dt);
        }
        else
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual Vector3 MoveDirection(float dt)
    {
        Vector3 moveDirection = Vector3.zero;
        return moveDirection.normalized;
    }

    private void Move(float dt)
    {
        Vector3 moveDirection = MoveDirection(dt);
        if (moveDirection.magnitude == 0)
        {
            moveDirection = currentHeading;
            if (speed > 0)
            {
                speed += attenuation * dt;
                if (speed < 0)
                {
                    speed = 0;
                }
            }
        }
        else
        {
            if (speed < maxSpeed)
            {
                speed += acceleration * dt;
                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }
            }
        }

        moveDirection = (moveDirection + currentHeading).normalized;

        transform.position = Vector2.MoveTowards(transform.position, transform.position + moveDirection, speed * dt);

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * dt);
    }

    protected virtual void CalculateHeading(float dt)
    {
        Vector3 dir = (transform.position - lastPos) / dt;
        currentHeading = dir.normalized;
        lastPos = transform.position;
    }

    public Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
}
