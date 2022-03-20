using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent2 : MonoBehaviour
{
    public bool log;
    private Vector3 velocity;
    public Vector3 Velocity { get { return velocity; }}
    private Sprite sprite;

    /// <summary>
    /// Length of this agent.
    /// </summary>
    private float length;

    /// <summary>
    /// Number of flockmates currently being considered in the steering force 
    /// calculation.
    /// </summary>
    private int neighbourCount;

    /// <summary>
    /// A running total of the positions of this agent's nearby flockmates.
    /// </summary>
    private Vector3 totalPosition;

    /// <summary>
    /// A running total of the velocities of this agent's nearby flockmates.
    /// </summary>
    private Vector3 totalVelocity;

    private Vector3 totalAvoidance;

    /// <summary>
    /// The average position of this agent's nearby flockmates.
    /// </summary>
    private Vector3 avgPosition;

    /// <summary>
    /// The average velocity of this agent's nearby flockmates.
    /// </summary>
    private Vector3 avgVelocity;

    /// <summary>
    /// Dimensions of the scene in world coordinates.
    /// </summary>
    private Vector3 sceneDimensions;

    public float visualRange;
    public float minDistance;

    public float cohesionFactor;
    public float alignmentFactor;
    public float separationFactor;

    /// <summary>
    /// The strength of the force used to keep an agent on-screen.
    /// </summary>
    [SerializeField] private float screenRetentionFactor;

    public float margin;
    public float maxSpeed;
    [SerializeField] private float maxAngle;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        length = GetComponent<Renderer>().bounds.size.y;
        sceneDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 13));
        transform.position = new Vector3(
            Random.Range(-sceneDimensions.x / 2, sceneDimensions.x / 2),
            Random.Range(-sceneDimensions.y / 2, sceneDimensions.y / 2),
            0);
        transform.rotation = Quaternion.LookRotation(Velocity.normalized);

        velocity = new Vector3(Random.Range(1, 6), Random.Range(1, 6), 0);
    }

    private void CheckNeighbours()
    {
        //float signedAngle;

        foreach (Agent2 a in GameManager.agents)
        {
            //inView = false;

            // Ensure the current agent is not being checked against itself.
            if (a != this)
            {
                // Calculate the distance vector between this agent and the
                // current flockmate under consideration.
                Vector3 distanceVector = /*a.transform.position - */transform.position - a.transform.position;
                float distance = Vector3.Distance(transform.position, a.transform.position);
                //localDistance = transform.InverseTransformDirection(globalDistance);

                // Determine whether the flockmate under consideration is
                // within this agent's field of view.
                //
                // If the angle between this agent's and the current
                // flockmate's positions is less than or equal to fovHalved,
                // then the current flockmate is considered "in view".
                //
                // fov is halved, as Vector3.Angle() returns unsigned angles
                // between 0deg and 180deg.
                //inView = Vector3.Angle(transform.position, a.transform.position) 
                //    <= fovHalved;

                //signedAngle = Vector3.SignedAngle(
                //    transform.position,
                //    a.transform.position,
                //    Vector3.up);

                // Check if the flockmate is within a specified distance from
                // this agent, in addition to being in view. If both checks
                // pass, it is "visible" and will be considered a neighbour for
                // subsequent steering force calculations.
                //if (inView)
                //{
                //    if (distance.magnitude <= length * radius)
                //    {
                //        totalPosition += a.transform.position;
                //        totalVelocity += a.Rb.velocity;
                //        neighbourCount++;
                //    }

                //    CalculateSeparation(signedAngle);
                //}
                //// Wide field-of-view
                //if (fov > 180)
                //{
                //    inView = localDistance.y > 0 
                //        || (localDistance.y < 0 && 
                //}

                if (distance < length * visualRange)
                {
                    totalPosition += a.transform.position;
                    totalVelocity += a.Velocity;
                    //totalPositionAway += Vector3.Normalize(transform.position - a.transform.position) / (distance * distance);
                    neighbourCount++;
                }

                if (distance < length * minDistance)
                {
                    totalAvoidance += distanceVector;
                }
            }
        }
    }

    private void CalculateCohesion()
    {
        avgPosition = totalPosition / neighbourCount;

        velocity.x += (avgPosition.x - transform.position.x) * cohesionFactor;
        velocity.y += (avgPosition.y - transform.position.y) * cohesionFactor;
    }

    private void CalculateAlignment()
    {
        avgVelocity = totalVelocity / neighbourCount;

        velocity.x += (avgVelocity.x - velocity.x) * alignmentFactor;
        velocity.y += (avgVelocity.y - velocity.y) * alignmentFactor;
    }

    private void CalculateSeparation()
    {
        velocity += totalAvoidance * separationFactor;
    }

    private void RetainOnScreen()
    {
        //// ----- USE THIS -----
        //if (transform.position.x < -sceneDimensions.x / 2)
        //    newVelocity.x += screenRetentionFactor;
        //else if (transform.position.x > sceneDimensions.x / 2)
        //    newVelocity.x -= screenRetentionFactor;

        //if (transform.position.y < -sceneDimensions.y / 2)
        //    newVelocity.y += screenRetentionFactor;
        //else if (transform.position.y > sceneDimensions.y / 2)
        //    newVelocity.y -= screenRetentionFactor;

        //screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (Camera.main.WorldToScreenPoint(transform.position - new Vector3(0, sprite.bounds.extents.y, 0)).y < margin)
        {
            velocity.y += screenRetentionFactor;
            //velocity.y *= -1;
            //velocity = -velocity;
        }
        if (Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, sprite.bounds.extents.y, 0)).y > Screen.height - margin)
        {
            velocity.y -= screenRetentionFactor;
            //velocity.y *= -1;
            //velocity = -velocity;
        }

        if (Camera.main.WorldToScreenPoint(transform.position - new Vector3(sprite.bounds.extents.x, 0, 0)).x < margin)
        {
            velocity.x += screenRetentionFactor;
            //velocity.x *= -1;
            //velocity = -velocity;
        }
        if (Camera.main.WorldToScreenPoint(transform.position + new Vector3(sprite.bounds.extents.x, 0, 0)).x > Screen.width - margin)
        {
            velocity.x -= screenRetentionFactor;
            //velocity.x *= -1;
            //velocity = -velocity;
        }
    }

    private void LimitSpeed()
    {
        //Rb.velocity = Rb.velocity.normalized * maxSpeed;
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;
    }

    //private Vector3 LimitRotation()
    //{
    //    return Vector3.RotateTowards(transform.position, velocity, maxAngle * Mathf.Deg2Rad, maxSpeed);
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        neighbourCount = 0;

        CheckNeighbours();

        if (neighbourCount > 0)
        {
            CalculateCohesion();
            CalculateAlignment();
        }
        
        CalculateSeparation();
        
        LimitSpeed();
        //LimitRotation();
        RetainOnScreen();
        
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);
    }
}

// TODO: reset neighbourCount
