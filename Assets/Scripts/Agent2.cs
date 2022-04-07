using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adapted from:
/// - https://github.com/beneater/boids/
/// - https://github.com/zklinger2000/unity-boids
/// - https://www.oreilly.com/library/view/ai-for-game/0596005555/ch04.html
/// </summary>
public class Agent2 : MonoBehaviour
{
    //public bool log;

    // Property separate from the field because Vector3's are structs. This one
    // in particular needs to be modified within the class, and so a returned
    // copy (if an auto-property were used) is not very helpful.

    /// <summary>
    /// This agent's velocity vector.
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Returns a copy of this agent's velocity vector.
    /// </summary>
    public Vector3 Velocity { get { return velocity; }}

    /// <summary>
    /// Sprite used to represent this agent onscreen.
    /// </summary>
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

    /// <summary>
    /// Running total of distance vectors from this agent to its neighbours 
    /// that are too close by.
    /// </summary>
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

    /// <summary>
    /// A vector describing this agent's line of sight. Defines how far the 
    /// agent can "see" ahead of it.
    /// </summary>
    private Vector3 ahead;

    /// <summary>
    /// A vector describing this agent's line of sight. Equal to ahead, halved.
    /// </summary>
    private Vector3 aheadHalved;

    /// <summary>
    /// Radius of this agent's field of view.
    /// </summary>
    [SerializeField] private float visualRange;

    /// <summary>
    /// The minimum distance this agent should strive to be from all others in
    /// its vicinity. 
    /// Expressed as a factor of the agent's length.
    /// </summary>
    [SerializeField] private float minDistance;

    /// <summary>
    /// Influence of the cohesion rule.
    /// </summary>
    [SerializeField] private float cohesionFactor;

    /// <summary>
    /// Influence of the alignment rule.
    /// </summary>
    [SerializeField] private float alignmentFactor;

    /// <summary>
    /// Influence of the separation rule.
    /// </summary>
    [SerializeField] private float separationFactor;

    /// <summary>
    /// The strength of the force used to keep an agent on-screen.
    /// </summary>
    [SerializeField] private float screenRetentionFactor;

    /// <summary>
    /// Distance from the edge of the screen the agent must keep within.
    /// </summary>
    [SerializeField] private float margin;

    /// <summary>
    /// The maximum speed this agent can travel at.
    /// </summary>
    [SerializeField] private float maxSpeed;

    /// <summary>
    /// The maximum distance the agent can see ahead of itself, collinear to 
    /// its velocity vector. The greater the value, the earlier the agent will
    /// begin avoiding an incoming obstacle.
    /// </summary>
    [SerializeField] private float maxSeeAhead;

    // Deprecated
    [SerializeField] private float maxAngle;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>().sprite;
        length = GetComponent<Renderer>().bounds.size.y;

        // Spawn agents at random locations and velocities onscreen.
        sceneDimensions = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, 13));
        transform.position = new Vector3(
            Random.Range(-sceneDimensions.x / 2, sceneDimensions.x / 2),
            Random.Range(-sceneDimensions.y / 2, sceneDimensions.y / 2),
            0);
        velocity = new Vector3(Random.Range(1, 6), Random.Range(1, 6), 0);
        // Idk
        //transform.rotation = Quaternion.LookRotation(Velocity.normalized);
    }

    /// <summary>
    /// Neighbour-checking logic.
    /// </summary>
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

                // If in visual range, the agent is considered a neighbour to
                // this one, and included in subsequent calculations.
                // Using a function of the agent's length so the simulation
                // scales along with world size.
                if (distance < length * visualRange)
                {
                    totalPosition += a.transform.position;
                    totalVelocity += a.Velocity;
                    //totalPositionAway += Vector3.Normalize(transform.position - a.transform.position) / (distance * distance);
                    neighbourCount++;
                }

                // If close enough, the agent presents a potential collision,
                // and is included in separation calculations.
                if (distance < length * minDistance)
                {
                    totalAvoidance += distanceVector;
                }
            }
        }
    }

    #region The Three Rules
    /// <summary>
    /// Applies cohesion rule calculations.
    /// </summary>
    private void CalculateCohesion()
    {
        // Obtain average position and alter velocity with the help of 
        // cohesionFactor.
        avgPosition = totalPosition / neighbourCount;

        velocity.x += (avgPosition.x - transform.position.x) * cohesionFactor;
        velocity.y += (avgPosition.y - transform.position.y) * cohesionFactor;
    }

    /// <summary>
    /// Applies alignment rule calculations.
    /// </summary>
    private void CalculateAlignment()
    {
        // Obtain average velocity and alter velocity with the help of 
        // alignmentFactor.
        avgVelocity = totalVelocity / neighbourCount;

        velocity.x += (avgVelocity.x - velocity.x) * alignmentFactor;
        velocity.y += (avgVelocity.y - velocity.y) * alignmentFactor;
    }

    /// <summary>
    /// Applies separation rule calculations.
    /// </summary>
    private void CalculateSeparation()
    {
        velocity += totalAvoidance * separationFactor;
    }
    #endregion

    #region Collision Avoidance
    private void CheckForObstacles()
    {

    }

    private void AvoidCollisions()
    {
        // Define the vector describing the agent's line of sight.
        ahead = transform.position + Vector3.Normalize(velocity) * maxSeeAhead;

        RaycastHit2D hit;
        //Physics.Raycast(transform.position, transform.forward, out hit, maxSeeAhead);
        hit = Physics2D.Raycast(transform.position, transform.forward, maxSeeAhead);

        Debug.DrawLine(transform.position, transform.forward * maxSeeAhead, Color.white);

        if (hit) Debug.Log("hit");
        //Debug.Log(hit);
    }
    #endregion

    #region Housekeeping
    /// <summary>
    /// Keeps the agent onscreen.
    /// </summary>
    private void RetainOnScreen()
    {
        // Viewing the sprite as an AABB, obtain its four bounds.
        float spriteUpperBound = Camera.main.WorldToScreenPoint(
            transform.position - new Vector3(0, sprite.bounds.extents.y, 0)).y;
        float spriteLowerBound = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(0, sprite.bounds.extents.y, 0)).y;
        float spriteLeftBound = Camera.main.WorldToScreenPoint(
            transform.position - new Vector3(sprite.bounds.extents.x, 0, 0)).x;
        float spriteRightBound = Camera.main.WorldToScreenPoint(
            transform.position + new Vector3(sprite.bounds.extents.x, 0, 0)).x;
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

        // Screen coordinates are 2D and measured in pixels. They start in the
        // lower left corner (0, 0) and go to (Screen.width, Screen.height).
        if (spriteUpperBound < margin)
        {
            velocity.y += screenRetentionFactor;
            //velocity.y *= -1;
            //velocity = -velocity;
        }
        if (spriteLowerBound > Screen.height - margin)
        {
            velocity.y -= screenRetentionFactor;
            //velocity.y *= -1;
            //velocity = -velocity;
        }

        if (spriteLeftBound < margin)
        {
            velocity.x += screenRetentionFactor;
            //velocity.x *= -1;
            //velocity = -velocity;
        }
        if (spriteRightBound > Screen.width - margin)
        {
            velocity.x -= screenRetentionFactor;
            //velocity.x *= -1;
            //velocity = -velocity;
        }
    }

    /// <summary>
    /// Prevents the agent from travelling too fast.
    /// </summary>
    private void LimitSpeed()
    {
        //Rb.velocity = Rb.velocity.normalized * maxSpeed;
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;
    }
    #endregion

    //private Vector3 LimitRotation()
    //{
    //    return Vector3.RotateTowards(transform.position, velocity, maxAngle * Mathf.Deg2Rad, maxSpeed);
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        // Reset counter variables at the start of each update loop iteration.
        neighbourCount = 0;
        totalPosition = Vector3.zero;
        totalVelocity = Vector3.zero;
        totalAvoidance = Vector3.zero;

        CheckNeighbours();

        // Cohesion and alignment rules depend on the presence of neighbours.
        if (neighbourCount > 0)
        {
            CalculateCohesion();
            CalculateAlignment();
        }
        
        // Separation does not necessarily depend on the presence of neighbours;
        // visual range != minimum distance.
        CalculateSeparation();
        
        // Housekeeping
        LimitSpeed();
        //LimitRotation();
        RetainOnScreen();

        AvoidCollisions();

        // Apply calculated changes to this agent.
        transform.position = new Vector3(
            transform.position.x + velocity.x,
            transform.position.y + velocity.y,
            transform.position.z);

        // 2D-specific implementation that rotates the sprite in the direction
        // of movement. 
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, transform.forward);

        //transform.Rotate(transform.forward, angle);
        //transform.rotation = Quaternion.LookRotation(velocity);
        //transform.rotation = Quaternion.LookRotation(transform.forward);
    }
}

// TODO: rotate agent
