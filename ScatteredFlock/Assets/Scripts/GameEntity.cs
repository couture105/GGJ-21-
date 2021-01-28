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

    public bool useAvoidObjects;
    public float avoidObjectDirectionFallOffSpeed = 20f;
    public float avoidObjectMultiplier = 20f;
    public Vector2 scanAngle = new Vector2(60, -60);
    public float avoidForceProximityMultiplier = 100;
    public float avoidObjectsScanDistance = 2.0f;
    public float currentAvoidForce = 0;

    public float speed = 0;
    protected Vector3 lastPos = Vector3.zero;
    protected Vector3 avoidObjectDirection = Vector3.zero;

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

        moveDirection += GetAvoidObjectsDirection(dt);

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

    protected Vector3 GetAvoidObjectsDirection(float dt)
    {
        Vector3 moveDirection = Vector3.zero;
        if (useAvoidObjects)
        {
            Vector3 avoidObjectDirectionT = ScanForObjectsToAvoid().normalized * avoidObjectMultiplier;
            if (avoidObjectDirectionT.magnitude > 0)
            {
                avoidObjectDirection = avoidObjectDirectionT;
            }
            avoidObjectDirection = Vector3.Lerp(avoidObjectDirection, Vector3.zero, dt * avoidObjectDirectionFallOffSpeed);
            if (avoidObjectDirection.magnitude < 0.01f)
            {
                avoidObjectDirection = Vector3.zero;
            }
            moveDirection += avoidObjectDirection;
        }

        return moveDirection;
    }

    protected Vector3 ScanForObjectsToAvoid()
    {
        Vector2 avgDirection = Vector2.zero;
        Vector3 longestOpenPathDirection = Vector3.zero;
        Vector3 longestClosedPathDirection = Vector3.zero;
        bool hitSomething = false;
        currentAvoidForce = 1;
        float lastHitDistance = 0;

        float angle = Mathf.Atan2(currentHeading.y, currentHeading.x) * Mathf.Rad2Deg;
        Vector3 dirOne = DegreeToVector2(scanAngle.x + angle);
        dirOne = (currentHeading + dirOne).normalized * avoidObjectsScanDistance;
        Vector3 dirTwo = DegreeToVector2(scanAngle.y + angle);
        dirTwo = (currentHeading + dirTwo).normalized * avoidObjectsScanDistance;

        for (int t = 0; t < 3; t++)
        {
            RaycastHit2D raycastHit2DResult;
            Vector3 currentRayDir = Vector3.zero;
            if (t == 0)
            {
                currentRayDir = currentHeading * (avoidObjectsScanDistance + 0.5f);
                raycastHit2DResult = Physics2D.Raycast(this.transform.position, currentRayDir, avoidObjectsScanDistance + 0.5f);
                //Debug.DrawRay(this.transform.position, currentRayDir, Color.blue, 0.1f);
            }
            else if (t == 1)
            {
                currentRayDir = dirOne;
                raycastHit2DResult = Physics2D.Raycast(this.transform.position, dirOne, avoidObjectsScanDistance);
                //Debug.DrawRay(this.transform.position, currentRayDir, Color.red, 0.1f);
            }
            else
            {
                currentRayDir = dirTwo;
                raycastHit2DResult = Physics2D.Raycast(this.transform.position, dirTwo, avoidObjectsScanDistance);
                //Debug.DrawRay(this.transform.position, currentRayDir, Color.green, 0.1f);
            }
            if (raycastHit2DResult.collider != null)
            {
                hitSomething = true;
                float dist = Vector3.Distance(transform.position, raycastHit2DResult.point);
                if (dist > lastHitDistance)
                {
                    lastHitDistance = dist;
                    longestClosedPathDirection = -currentRayDir;
                }
            }
            else
            {
                longestOpenPathDirection = currentRayDir;
            }

        }
        if (!hitSomething)
        {
            return Vector2.zero;
        }
        else if (longestOpenPathDirection == Vector3.zero)
        {
            return longestClosedPathDirection;
        }
        else
        {
            return longestOpenPathDirection;
        }
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
