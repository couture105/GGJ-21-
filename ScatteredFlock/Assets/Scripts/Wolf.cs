using UnityEngine;

public class Wolf : GameEntity
{
    public bool useExternalDirection = true;

    public Vector3 startPos = Vector3.zero;
    public float maxWanderDistance = 24;
    public float changeDirectionTime = 1.0f;
    public float externalDirectionMultiplier = 5f;

    private float changeDirectionTimer = 0f;
    private Vector3 lastMoveDirection = Vector3.zero;

    protected override Vector3 MoveDirection(float dt)
    {
        changeDirectionTimer += dt;
        if (changeDirectionTimer > changeDirectionTime)
        {
            lastMoveDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            changeDirectionTimer = 0;
        }

        Vector3 moveDirection = lastMoveDirection;

        moveDirection += GetExternalDirection() * externalDirectionMultiplier;

        if ((startPos - transform.position).magnitude > maxWanderDistance)
        {
            moveDirection += (startPos - transform.position).normalized;
        }

        moveDirection += GetAvoidObjectsDirection(dt);

        return moveDirection.normalized;
    }

    private Vector3 GetExternalDirection()
    {
        if (!useExternalDirection)
            return Vector3.zero;

        Vector3 externalDirection = Vector3.zero;

        Shepherd shepherd = level.shepherd;
        if (shepherd.active && shepherd.isThreatening)
        {
            float distance = Vector3.Distance(transform.position, shepherd.transform.position);
            if (distance <= shepherd.influenceRadius)
            {
                Vector3 diff = this.transform.position - shepherd.transform.position;
                diff.Normalize();
                diff = diff / distance;
                externalDirection += diff;
            }
        }
        return externalDirection;
    }
}
