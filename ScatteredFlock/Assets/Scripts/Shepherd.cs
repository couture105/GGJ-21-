using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : GameEntity
{
    public float moveRadius = 16.0f;
    public override void DeltaUpdate(float dt)
    {
        base.DeltaUpdate(dt);
        if (Input.GetMouseButton(0))
        {
            isAtracting = true;
        }
        else
        {
            isAtracting = false;
        }

        if (Input.GetMouseButton(1))
        {
            isThreatening = true;
        }
        else
        {
            isThreatening = false;
        }
    }

    protected override Vector3 MoveDirection(float dt)
    {
        Vector3 moveDirection = Vector3.zero;
        

        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

        Vector3 dir = (worldPosition - transform.position);
        if (dir.magnitude > moveRadius)
        {
            dir = dir / dt;
            moveDirection = dir.normalized;
        }

        Vector3 avoidDirection = GetAvoidObjectsDirection(dt);
        if (avoidObjectDirection.magnitude > 0)
        {
            moveDirection += GetAvoidObjectsDirection(dt);
        }

        return moveDirection.normalized;
    }
}
