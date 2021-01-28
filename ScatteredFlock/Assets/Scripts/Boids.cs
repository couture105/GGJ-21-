using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{

    //predefined array to keep unity GC low
    private Collider2D[] surroundingCollidersNonAlloc = new Collider2D[10];
    private int surroundingColliderNonAllocLength = 0;
    //filtered array of surrounding units, predefinied to keep GC low.
    private Boids[] surroundingUnits = new Boids[10];
    private int surroundingUnitsLength = 0;

    //adjustable control variables
    public float scanRadius = 1f;
    public float scanDistance = 0.5f;
    public float avoidRadius = 0.15f;
    public float avoidObjectsScanDistance = 1.0f;
    public float cohesionAmount = 0.5f;
    public float moveSpeed = 3f;
    public float maxSpeed = 5f;
    public float avoidObjectDirectionFallOffSpeed = 20f;
    public float avoidObjectMultiplier = 20f;
    public float avergaeDirectionMultiplier = 5f;
    public float avoidDirectionMultiplier = 1f;

    //controls to enable or disable aspects of Boids algorithm
    public bool useAverageDirection;
    public bool useAvoidUnitsDirection;
    public bool useAvoidObjects;
    public bool useCohesion;

    //other variables
    private Vector2 lastPos = Vector2.zero;
    private Vector2 randomStartDirection;
    private Vector2 avoidObjectDirection = Vector2.zero;
    private void Start()
    {
        Random.InitState(Mathf.RoundToInt(Time.frameCount));
        currentHeading = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }
    void Update()
    {
        Move();
        CalcHeading();
    }
    /// <summary>
    /// Scan for surrounding flockmates using a NonAllocOverlapCircle. NonAlloc used to reduce memory usage and GC collection.
    /// </summary>
    /// <param name="scanRadius"></param>
    void GetSurroundingUnits(float scanRadius)
    {
        surroundingColliderNonAllocLength = Physics2D.OverlapCircleNonAlloc(this.transform.position, scanRadius, surroundingCollidersNonAlloc, 1 << LayerMask.NameToLayer("NPCS"));
        surroundingUnitsLength = 0;
        for (int i = 0; i < surroundingColliderNonAllocLength; i++)
        {
            //prevent including self
            if (surroundingCollidersNonAlloc[i].gameObject != this.gameObject)
            {
                surroundingUnits[surroundingUnitsLength] = surroundingCollidersNonAlloc[i].GetComponent<Boids>();
                surroundingUnitsLength++;
            }
        }
    }
    /// <summary>
    /// Gets the average direction of local flockmates.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetAverageDirection(float scanRadius)
    {
        if (!useAverageDirection)
            return Vector2.zero;

        Vector2 averageDirection = Vector2.zero;
        //use for loop instead of foreach, for loops are more optimized in Unity for memory management at high frame rate.
        for (int i = 0; i < surroundingUnitsLength; i++)
        {
            //get distance between this unit and surrounding unit
            float distance = Vector2.Distance(this.transform.position, surroundingUnits[i].transform.position);
            Vector2 velocity = surroundingUnits[i].currentHeading.normalized;//.thisRigidbody2D.velocity.normalized;
            velocity = velocity / distance;
            //add unit's direction to overall average. 
            averageDirection += velocity;
        }
        if (surroundingUnitsLength > 0)
            return averageDirection / surroundingUnitsLength;
        else return averageDirection;
    }
    /// <summary>
    /// Gets direction opposite to avoid crowding and overlap of flockmates.
    /// </summary>
    /// <param name="avoidRadius">Radius to scan for clockmates to avoid</param>
    /// <returns></returns>
    private Vector2 GetAvoidDirection(float avoidRadius)
    {
        if (!useAvoidUnitsDirection)
            return Vector2.zero;

        Vector2 avoidDirection = Vector2.zero;
        //use for loop instead of foreach, for loops are more optimized in Unity for memory management at high frame rate.
        for (int i = 0; i < surroundingUnitsLength; i++)
        {
            float distance = Vector2.Distance(this.transform.position, surroundingUnits[i].transform.position);
            if (distance <= avoidRadius)
            {
                Vector2 diff = this.transform.position - surroundingUnits[i].transform.position;
                diff.Normalize();
                diff = diff / distance;//weight based on distance.
                avoidDirection += diff;
            }
        }
        return avoidDirection;
    }
    public Vector2 currentHeading;
    /// <summary>
    /// Calculate the current direction we're heading.
    /// </summary>
    void CalcHeading()
    {
        //good old trig math to calculate current direction based on last position.
        //Vector2 dir = (this.transform.position.xy() - lastPos) / Time.deltaTime;
        //currentHeading = dir.normalized;
        //lastPos = this.transform.position.xy();
    }
    public Vector2 scanAngle = new Vector2(75, -75);
    public float currentAvoidForce = 0;
    public float avoidForceProximityMultiplier = 100;
    /// <summary>
    /// Gets direction opposite to avoid bumping into map objects (i.e. walls)
    /// </summary>
    /// <returns></returns>
    Vector2 ScanForObjectsToAvoid()
    {
        Vector2 avgDirection = Vector2.zero;
        Vector3 longestOpenPathDirection = Vector3.zero;
        Vector3 longestClosedPathDirection = Vector3.zero;
        bool hitSomething = false;
        currentAvoidForce = 1;
        float lastHitDistance = 0;

        //get angle of current heading
        float angle = Mathf.Atan2(currentHeading.y, currentHeading.x) * Mathf.Rad2Deg;
        //upward relative angle to direction of movement.
        Vector2 dirOne = DegreeToVector2(scanAngle.x + angle);
        dirOne = (currentHeading + dirOne).normalized * avoidObjectsScanDistance;
        //downward relative angle to direction of movement.
        Vector2 dirTwo = DegreeToVector2(scanAngle.y + angle);
        dirTwo = (currentHeading + dirTwo).normalized * avoidObjectsScanDistance;

        //we're doing 3 raycasts
        for (int t = 0; t < 3; t++)
        {
            RaycastHit2D raycastHit2DResult;
            Vector2 currentRayDir = Vector2.zero;
            if (t == 0)
            {
                currentRayDir = currentHeading * (avoidObjectsScanDistance + 0.5f);
                raycastHit2DResult = Physics2D.Raycast(this.transform.position, currentRayDir, avoidObjectsScanDistance + 0.5f, 1 << LayerMask.NameToLayer("Map"));
                Debug.DrawRay(this.transform.position, currentHeading * (avoidObjectsScanDistance + 0.5f), Color.blue, 0.1f);
            }
            else if (t == 1)
            {
                currentRayDir = dirOne;
                raycastHit2DResult = Physics2D.Raycast(this.transform.position, dirOne, avoidObjectsScanDistance, 1 << LayerMask.NameToLayer("Map"));
                Debug.DrawRay(this.transform.position, dirOne, Color.red, 0.1f);
            }
            else
            {
                currentRayDir = dirTwo;
                raycastHit2DResult = Physics2D.Raycast(this.transform.position, dirTwo, avoidObjectsScanDistance, 1 << LayerMask.NameToLayer("Map"));
                Debug.DrawRay(this.transform.position, dirTwo, Color.green, 0.1f);
            }
            //check if we hit anything
            if (raycastHit2DResult.collider != null)
            {
                hitSomething = true;
                Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                float dist = Vector2.Distance(pos, raycastHit2DResult.point);
                if (dist > lastHitDistance)
                {
                    lastHitDistance = dist;
                    longestClosedPathDirection = currentRayDir;
                }
            }
            //if the raycast didn't hit anything then that is currently longest open path
            else
            {
                longestOpenPathDirection = currentRayDir;
            }

        }
        //if no raycasts hit anything, we should return nothing and keep heading in current direction.
        if (!hitSomething)
        {
            return Vector2.zero;
        }
        else if (longestOpenPathDirection == Vector3.zero)
        {
            return longestClosedPathDirection;
        }
        else// if(longestOpenPathDirection == Vector3.zero)
        {
            return longestOpenPathDirection;
        }

        //    if (longestOpenPathDirection.xy() != Vector2.zero)
        //     DrawArrow.ForDebug(this.transform.position, longestOpenPathDirection* currentAvoidForce * 0.5f, Color.cyan, 0.5f);

        //   return longestOpenPathDirection;
    }
    /// <summary>
    /// Gets direction towards average position of local flockmates. 
    /// Can be used to keep boids closer or further apart.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetCohesionDirection(float cohesion)
    {
        if (!useCohesion)
            return Vector2.zero;

        Vector2 directionToCentre = Vector2.zero;
        Vector2 centrePosition = Vector2.zero;
        for (int i = 0; i < surroundingUnitsLength; i++)
        {
            float distance = Vector2.Distance(this.transform.position, surroundingUnits[i].transform.position);
            if (distance <= scanRadius)
            {
                Vector2 pos = new Vector2(surroundingUnits[i].transform.position.x, surroundingUnits[i].transform.position.y);
                centrePosition += pos;

            }
        }
        if (surroundingUnitsLength > 0)
        {
            //Debug.DrawRay(this.transform.position, centrePosition.normalized * 2, Color.cyan, 0.2f);
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            centrePosition = centrePosition / surroundingUnitsLength;
            directionToCentre = centrePosition - pos;
            //   Debug.DrawRay(this.transform.position, directionToCentre.normalized * 2,Color.blue, 0.2f);
        }
        return directionToCentre * cohesion;
    }
    /// <summary>
    /// Move this boid transform to target position based on MoveDirection calculation
    /// </summary>
    private void Move()
    {
        //move our boid transform.
        Vector3 moveDirection = (MoveDirection() + currentHeading).normalized;
        if (moveDirection.magnitude == 0)
            moveDirection = currentHeading;
        this.transform.position = Vector2.MoveTowards(this.transform.position, this.transform.position + moveDirection, moveSpeed * Time.deltaTime);


    }
    /// <summary>
    /// Calculate the direction to move taking into account other surrounding flockmates and objects
    /// </summary>
    /// <returns></returns>
    private Vector2 MoveDirection()
    {
        Vector2 moveDirection = Vector2.zero;

        //check if any surrounding units.
        GetSurroundingUnits(scanRadius);
        //get direction this unit should move based on surrounding units.
        moveDirection += GetAverageDirection(scanDistance) * avergaeDirectionMultiplier;
        moveDirection += GetAvoidDirection(avoidRadius) * avoidDirectionMultiplier;

        //scan for objects to avoid and then smoothly adjust to prevent sudden hit in direction change.
        var avoidObjectDirectionT = ScanForObjectsToAvoid().normalized * avoidObjectMultiplier;
        if (avoidObjectDirectionT.magnitude > 0)
            avoidObjectDirection = avoidObjectDirectionT;
        avoidObjectDirection = Vector2.Lerp(avoidObjectDirection, Vector2.zero, Time.deltaTime * avoidObjectDirectionFallOffSpeed);
        if (avoidObjectDirection.magnitude < 0.01f)
        {
            avoidObjectDirection = Vector2.zero;
        }
        moveDirection += avoidObjectDirection;
        moveDirection += GetCohesionDirection(cohesionAmount);

        return moveDirection.normalized;
    }
    /// <summary>
    /// Convert radian to Vector2
    /// </summary>
    /// <param name="radian"></param>
    /// <returns></returns>
    public Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    /// <summary>
    /// Convert degree to Vector2
    /// </summary>
    /// <param name="degree"></param>
    /// <returns></returns>
    public Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
}
