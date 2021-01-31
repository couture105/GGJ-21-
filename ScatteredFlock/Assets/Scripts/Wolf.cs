using UnityEngine;

public class Wolf : GameEntity
{
    public bool useExternalDirection = true;
    public bool useAttack= true;
    public float attackCoolDown = 2.0f;
    public float attackRadius = 1.5f;

    public float maxWanderDistance = 24;
    public float changeDirectionTime = 1.0f;
    public float externalDirectionMultiplier = 5f;

    private float changeDirectionTimer = 0f;
    private float attackCooldownTimer = 0f;
    private Vector3 lastMoveDirection = Vector3.zero;

    public override void DeltaUpdate(float dt)
    {
        base.DeltaUpdate(dt);
        if (active && (!level.finished))
        {
            UpdateAttack(dt);
        }
    }

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
        if (shepherd.active && (shepherd.isThreatening || (useAttack && (attackCooldownTimer > 0))))
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

        if (useAttack && (attackCooldownTimer > 0))
        {
            for (int i = 0; i < level.sheeps.Count; i++)
            {
                Sheep sheep = level.sheeps[i];
                if (sheep.active == false)
                {
                    continue;
                }
                float distance = Vector3.Distance(this.transform.position, sheep.transform.position);
                if (distance < influenceRadius)
                {
                    Vector3 diff = this.transform.position - sheep.transform.position;
                    diff.Normalize();
                    diff = diff / distance;
                    externalDirection += diff;
                }
            }
        }
        return externalDirection;
    }

    private void UpdateAttack(float dt)
    {
        if (useAttack)
        {
            if (attackCooldownTimer == 0)
            {
                for (int i = 0; i < level.sheeps.Count; i++)
                {
                    Sheep sheep = level.sheeps[i];
                    if (sheep.active == false)
                    {
                        continue;
                    }
                    float distance = Vector3.Distance(this.transform.position, sheep.transform.position);
                    if (distance < attackRadius)
                    {
                        level.SpawnHitEffect(sheep.transform.position, Quaternion.identity);
                        level.DestroySheep(sheep);
                        level.eatenSheeps++;
                        attackCooldownTimer = attackCoolDown;
                    }
                }
            }
            else
            {
                attackCooldownTimer -= dt;
                if (attackCooldownTimer <= 0)
                {
                    attackCooldownTimer = 0;
                }
            }
        }
    }
}
