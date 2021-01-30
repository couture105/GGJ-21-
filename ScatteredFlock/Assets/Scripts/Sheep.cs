using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : GameEntity
{
    public enum SheepType
    {
        Red,
        Green,
        Blue
    }

    public bool useAverageDirection = true;
    public bool useAvoidDirection = true;
    public bool useExternalDirection = true;
    public bool useCohesion = true;
    public bool useOneTypeFlock = true;

    public float scanRadius = 4.0f;
    public float cohesionAmount = 0.5f;
    public float penRadius = 4.0f;

    public float avergaeDirectionMultiplier = 5f;
    public float avoidDirectionMultiplier = 1f;
    public float externalDirectionMultiplier = 5f;
    public float penDirectionMultiplier = 5f;

    public float maxWanderDistance = 256;
    public SheepType type;
    public bool respawns = true;

    public override void Start()
    {
        base.Start();
        /*
        switch (type)
        {
            case SheepType.Red:
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
                break;
            }

            case SheepType.Green:
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.green;
                }
                break;
            }

            case SheepType.Blue:
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.blue;
                }
                break;
            }
        }
        */
    }

    protected override Vector3 MoveDirection(float dt)
    {
        Vector3 moveDirection = Vector3.zero;

        moveDirection += GetAverageDirection() * avergaeDirectionMultiplier;
        moveDirection += GetAvoidDirection() * avoidDirectionMultiplier;
        moveDirection += GetCohesionDirection(cohesionAmount);

        Vector3 externalDirection = GetExternalDirection();
        moveDirection += externalDirection * externalDirectionMultiplier;

        /*
        if ((externalDirection == Vector3.zero) && ((startPos - transform.position).magnitude > maxWanderDistance))
        {
            moveDirection += (startPos - transform.position).normalized;
        }
        */

        moveDirection += GetAvoidObjectsDirection(dt);

        return moveDirection.normalized;
    }

    private Vector3 GetAverageDirection()
    {
        if (!useAverageDirection)
            return Vector3.zero;

        Vector3 averageDirection = Vector2.zero;
        int count = 0;
        for (int i = 0; i < level.sheeps.Count; i++)
        {
            Sheep sheep = level.sheeps[i];
            if (sheep == this || (sheep.active == false) || (useOneTypeFlock && (sheep.type != type)))
            {
                continue;
            }
            float distance = Vector3.Distance(this.transform.position, sheep.transform.position);
            if (distance < scanRadius)
            {
                Vector3 velocity = sheep.currentHeading.normalized;
                velocity = velocity / distance;
                //add unit's direction to overall average. 
                averageDirection += velocity;
                count++;
            }
        }
        if (count > 0)
        {
            return averageDirection / count;
        }
        else return averageDirection;
    }

    private Vector3 GetAvoidDirection()
    {
        if (!useAvoidDirection)
            return Vector3.zero;

        Vector3 avoidDirection = Vector3.zero;
        for (int i = 0; i < level.sheeps.Count; i++)
        {
            Sheep sheep = level.sheeps[i];
            if (sheep == this || (sheep.active == false))
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, sheep.transform.position);
            if (distance <= avoidRadius)
            {
                Vector3 diff = this.transform.position - sheep.transform.position;
                diff.Normalize();
                diff = diff / distance;
                avoidDirection += diff;
            }

            if (useOneTypeFlock && (sheep.type != type))
            {
                if (distance <= scanRadius)
                {
                    Vector3 diff = this.transform.position - sheep.transform.position;
                    diff.Normalize();
                    diff = diff / distance;
                    avoidDirection += diff;
                }
            }
        }

        for (int i = 0; i < level.wolfs.Count; i++)
        {
            Wolf wolf = level.wolfs[i];
            if (wolf.active == false)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, wolf.transform.position);
            if (distance <= avoidRadius)
            {
                Vector3 diff = this.transform.position - wolf.transform.position;
                diff.Normalize();
                diff = diff / distance;
                avoidDirection += diff;
            }
        }

        for (int i = 0; i < level.rams.Count; i++)
        {
            Ram ram = level.rams[i];
            if (ram.active == false)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, ram.transform.position);
            if (distance <= ram.avoidRadius)
            {
                Vector3 diff = this.transform.position - ram.transform.position;
                diff.Normalize();
                diff = diff / distance;
                avoidDirection += diff;
            }
        }

        Shepherd shepherd = level.shepherd;
        if (shepherd.active)
        {
            float distance = Vector3.Distance(transform.position, shepherd.transform.position);
            if (distance <= avoidRadius)
            {
                Vector3 diff = this.transform.position - shepherd.transform.position;
                diff.Normalize();
                diff = diff / distance;
                avoidDirection += diff;
            }
        }

        return avoidDirection;
    }

    private Vector3 GetCohesionDirection(float cohesion)
    {
        if (!useCohesion)
            return Vector3.zero;

        Vector3 directionToCentre = Vector3.zero;
        Vector3 centrePosition = Vector3.zero;
        float count = 0;

        for (int i = 0; i < level.sheeps.Count; i++)
        {
            Sheep sheep = level.sheeps[i];
            if (sheep == this || (sheep.active == false) || (useOneTypeFlock && (sheep.type != type)))
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, sheep.transform.position);
            if (distance <= scanRadius)
            {
                centrePosition += sheep.transform.position;
                count++;
            }
        }

        
        if (count > 0)
        {
            centrePosition = centrePosition / count;
            directionToCentre = centrePosition - transform.position;
        }
        return directionToCentre * cohesion;
    }

    private Vector3 GetExternalDirection()
    {
        if (!useExternalDirection)
            return Vector3.zero;

        Vector3 externalDirection = Vector3.zero;
        
        Shepherd shepherd = level.shepherd;
        if (shepherd.active && (shepherd.isAtracting || shepherd.isThreatening))
        {
            float distance = Vector3.Distance(transform.position, shepherd.transform.position);
            if (distance <= shepherd.influenceRadius)
            {
                Vector3 diff = Vector3.zero;
                if (shepherd.isThreatening)
                { 
                    diff = this.transform.position - shepherd.transform.position;
                }
                else
                {
                    diff = shepherd.transform.position - this.transform.position;
                }
                diff.Normalize();
                diff = diff / distance;
                externalDirection += diff;
            }
        }

        for (int i = 0; i < level.wolfs.Count; i++)
        {
            Wolf wolf = level.wolfs[i];
            if (wolf.active == false)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, wolf.transform.position);
            if (distance <= wolf.influenceRadius)
            {
                Vector3 diff = this.transform.position - wolf.transform.position;
                diff.Normalize();
                diff = diff / distance;
                externalDirection += diff;
            }
        }

        for (int i = 0; i < level.rams.Count; i++)
        {
            Ram ram = level.rams[i];
            if (ram.active == false)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, ram.transform.position);
            if (distance <= ram.influenceRadius)
            {
                Vector3 diff = ram.transform.position - this.transform.position;
                diff.Normalize();
                diff = diff / distance;
                externalDirection += diff;
            }
        }

        for (int i = 0; i < level.pens.Count; i++)
        {
            Pen pen = level.pens[i];
            if (pen.acceptedSheepType != type)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, pen.transform.position);
            if (distance <= penRadius)
            {
                Vector3 diff = pen.transform.position - this.transform.position;
                diff.Normalize();
                diff = diff / distance;
                externalDirection += diff * penDirectionMultiplier;
            }
        }


        return externalDirection;
    }

    protected override void SetLayerMask()
    {
        base.SetLayerMask();
        switch (type)
        {
            case SheepType.Red:
            {
                collsionMask ^= LayerMask.GetMask("RedPortal");
                break;
            }

            case SheepType.Green:
            {
                collsionMask ^= LayerMask.GetMask("GreenPortal");
                break;
            }

            case SheepType.Blue:
            {
                collsionMask ^= LayerMask.GetMask("BluePortal");
                break;
            }
        }
    }
}
