//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Adapted from https://www.oreilly.com/library/view/ai-for-game/0596005555/ch04.html

//public class Agent1 : MonoBehaviour
//{
//    public bool log;
//    public Vector3 Velocity { get; private set; }

//    ///// <summary>
//    ///// Whether the current flockmate under consideration is within this
//    ///// agent's field of view.
//    ///// </summary>
//    //private bool inView;

//    ////int currentNeighbour;

//    /// <summary>
//    /// Number of flockmates currently being considered in the steering force 
//    /// calculation.
//    /// </summary>
//    private int neighbourCount;

//    ///// <summary>
//    ///// Search radius, as a scale factor of this agent's length.
//    ///// </summary>
//    ///// NOTE: A scale factor is used here so distances scale according to the
//    ///// size of the game world.
//    //[SerializeField] private float radiusFactor;

//    ///// <summary>
//    ///// Search radius, calculated using radiusFactor.
//    ///// </summary>
//    //private float radius;

//    ///// <summary>
//    ///// Field of view.
//    ///// </summary>
//    //[SerializeField] private float fov;

//    ///// <summary>
//    ///// fov, halved. Used in field of view calculations where unsigned angles 
//    ///// between 0deg and 180deg are returned.
//    ///// </summary>
//    //private float fovHalved;

//    /// <summary>
//    /// Length of this agent.
//    /// </summary>
//    private float length;

//    ///// <summary>
//    ///// The influence of the cohesion rule, as a scale factor of this agent's
//    ///// length.
//    ///// </summary>
//    ///// NOTE: A scale factor is used here so distances scale according to the
//    ///// size of the game world.
//    //[SerializeField] private float cohesionFactor;

//    ///// <summary>
//    ///// The influence of the alignment rule, as a scale factor of this agent's
//    ///// length.
//    ///// </summary>
//    ///// NOTE: A scale factor is used here so distances scale according to the
//    ///// size of the game world.
//    //[SerializeField] private float alignmentFactor;

//    ///// <summary>
//    ///// The influence of the separation rule, as a scale factor of this agent's
//    ///// length.
//    ///// </summary>
//    ///// NOTE: A scale factor is used here so distances scale according to the
//    ///// size of the game world.
//    //[SerializeField] private float separationFactor;

//    /// <summary>
//    /// The minimum distance each agent should be from others, as a scale 
//    /// factor of this agent's length.
//    /// </summary>
//    [SerializeField] private float separationDistanceFactor;

//    /// <summary>
//    /// The strength of the force used to keep an agent on-screen.
//    /// </summary>
//    [SerializeField] private float screenRetentionFactor;

//    /// <summary>
//    /// Maximum speed at which this agent can travel.
//    /// </summary>
//    [SerializeField] private float maxSpeed;
//    [SerializeField] private float maxAngle;

//    /// <summary>
//    /// A running total of the positions of this agent's nearby flockmates.
//    /// </summary>
//    private Vector3 totalPosition;

//    /// <summary>
//    /// A running total of the velocities of this agent's nearby flockmates.
//    /// </summary>
//    private Vector3 totalVelocity;

//    ///// <summary>
//    ///// The average position of this agent's nearby flockmates.
//    ///// </summary>
//    //private Vector3 avgPosition;

//    ///// <summary>
//    ///// The average velocity of this agent's nearby flockmates.
//    ///// </summary>
//    //private Vector3 avgVelocity;

//    ///// <summary>
//    ///// The direction in which this agent is headed i.e., its velocity.
//    ///// </summary>
//    //private Vector3 heading;

//    ///// <summary>
//    ///// This agent's position, relative to the viewport.
//    ///// </summary>
//    //private Vector3 posRelativeToViewport;

//    ///// <summary>
//    ///// The net steering force to apply this frame.
//    ///// </summary>
//    //private Vector3 netSteeringForce;

//    /// <summary>
//    /// Distance between this agent and the current flockmate under 
//    /// consideration.
//    /// </summary>
//    private Vector3 distanceVector;

//    ///// <summary>
//    ///// Displacement between this agent and the average position of its 
//    ///// neighbours.
//    ///// </summary>
//    //private Vector3 displacement;
//    ////private Vector3 localDistance;

//    ///// <summary>
//    ///// The location on this agent's rigid body at which to apply the steering 
//    ///// force vector.
//    ///// </summary>
//    //private Vector3 sfApplicationPoint;


//    ///// <summary>
//    ///// Defines whether this agent should steer left or right this frame.
//    ///// </summary>
//    //private SteeringDirection steeringDirection;

//    ///// <summary>
//    ///// The rigid body component attached to this agent.
//    ///// </summary>
//    //public Rigidbody Rb { get; private set; }

//    /// <summary>
//    /// Dimensions of the scene in world coordinates.
//    /// </summary>
//    private Vector3 sceneDimensions;

//    [SerializeField] private float cohesionStep;
//    [SerializeField] private float cohesionWeight;
//    [SerializeField] private float alignmentWeight;
//    [SerializeField] private float separationWeight;

//    private Vector3 totalPositionAway;

//    // Start is called before the first frame update
//    void Start()
//    {
//        sceneDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 13));
//        Debug.Log(sceneDimensions.x + "x" + sceneDimensions.y);

//        transform.position = new Vector3(
//            Random.Range(-sceneDimensions.x / 2, sceneDimensions.x / 2),
//            Random.Range(-sceneDimensions.y / 2, sceneDimensions.y / 2),
//            -5);
//        transform.rotation = Quaternion.LookRotation(Velocity.normalized);

//        Velocity = new Vector3(Random.Range(1, 6), Random.Range(1, 6), 0);


//        //netSteeringForce.x = Random.Range(1, 6);
//        //netSteeringForce.y = Random.Range(1, 6);

//        ////netSteeringForce.x = netSteeringForce.y = netSteeringForce.z = 0;
//        //avgPosition.x = avgPosition.y = avgPosition.z = 0;
//        //avgVelocity.x = avgVelocity.y = avgVelocity.z = 0;
//        neighbourCount = 0;
//        length = GetComponent<Renderer>().bounds.size.y;
//        //radius = length * radiusFactor;
//        //Rb = GetComponent<Rigidbody>();

//        //// Offset sfApplicationPoint from centre of gravity to represent a
//        //// point on the very front and centreline of this agent.
//        //sfApplicationPoint.x = 0;
//        //sfApplicationPoint.y = length / 2;

//        //fovHalved = fov / 2;
//    }

//    private void CheckNeighbours()
//    {
//        ////float signedAngle;

//        //foreach (Agent1 a in GameManager.agents)
//        //{
//        //    //inView = false;

//        //    // Ensure the current agent is not being checked against itself.
//        //    if (a != this)
//        //    {
//        //        // Calculate the distance vector between this agent and the
//        //        // current flockmate under consideration.
//        //        distanceVector = /*a.transform.position - */this.transform.position - a.transform.position;
//        //        float distance = Vector3.Distance(transform.position, a.transform.position);
//        //        //localDistance = transform.InverseTransformDirection(globalDistance);

//        //        // Determine whether the flockmate under consideration is
//        //        // within this agent's field of view.
//        //        //
//        //        // If the angle between this agent's and the current
//        //        // flockmate's positions is less than or equal to fovHalved,
//        //        // then the current flockmate is considered "in view".
//        //        //
//        //        // fov is halved, as Vector3.Angle() returns unsigned angles
//        //        // between 0deg and 180deg.
//        //        //inView = Vector3.Angle(transform.position, a.transform.position) 
//        //        //    <= fovHalved;

//        //        //signedAngle = Vector3.SignedAngle(
//        //        //    transform.position,
//        //        //    a.transform.position,
//        //        //    Vector3.up);

//        //        // Check if the flockmate is within a specified distance from
//        //        // this agent, in addition to being in view. If both checks
//        //        // pass, it is "visible" and will be considered a neighbour for
//        //        // subsequent steering force calculations.
//        //        //if (inView)
//        //        //{
//        //        //    if (distance.magnitude <= length * radius)
//        //        //    {
//        //        //        totalPosition += a.transform.position;
//        //        //        totalVelocity += a.Rb.velocity;
//        //        //        neighbourCount++;
//        //        //    }

//        //        //    CalculateSeparation(signedAngle);
//        //        //}
//        //        //// Wide field-of-view
//        //        //if (fov > 180)
//        //        //{
//        //        //    inView = localDistance.y > 0 
//        //        //        || (localDistance.y < 0 && 
//        //        //}

//        //        if (Vector3.Distance(a.transform.position, this.transform.position) < length * separationDistanceFactor)
//        //        {
//        //            totalPosition += a.transform.position;
//        //            totalVelocity += a.Velocity;
//        //            totalPositionAway += Vector3.Normalize(transform.position - a.transform.position) / (distance * distance);
//        //            neighbourCount++;
//        //        }
//        //    }
//        //}
//    }

//    private Vector3 CalculateCohesion()
//    {
//        Vector3 avgPosition = Vector3.zero;

//        if (neighbourCount == 0 || cohesionStep < 1) return avgPosition;
        
//        // Average position is the vector sum of the positions of neighbours,
//        // divided by the total number of neighbors (a scalar).
//        avgPosition = totalPosition / neighbourCount;

//        return (avgPosition - transform.position) / cohesionStep * cohesionWeight;

//        //// Get the heading of this agent, normalised.
//        //heading = Rb.velocity.normalized;

//        //// Get displacement and normalise it.
//        //displacement = (avgPosition - transform.position).normalized;

//        //// Use the angle formed between this agent's position and avgPosition
//        //// relative to the world up vector (a signed angle) to determine where
//        //// to steer this agent. If the angle < 0, this agent should be steered
//        //// left, since avgPosition is to its left, and vice versa.
//        //if (Vector3.SignedAngle(transform.position, avgPosition, Vector3.up) < 0)
//        //    steeringDirection = SteeringDirection.Left;
//        //else steeringDirection = SteeringDirection.Right;

//        //// Check if the dot product between the unit vectors representing
//        //// heading and displacement are in the range (-1, 1). This must be
//        //// done, because the dot product and arccos are used under the hood to
//        //// calculate the angle between these two vectors, and the Acos method
//        //// takes an argument in the range (-1, 1).
//        //if (Mathf.Abs(Vector3.Dot(heading, displacement)) < 1)
//        //    // TODO: netsteeringforce.x/y/z?
//        //    // The steering force accumulated in netSteeringForce.x is a linear
//        //    // function of the angle between the current unit’s heading and the
//        //    // vector to the average position of its neighbors.
//        //    //
//        //    // Divide the resulting angle by pi to yield a scale factor that
//        //    // gets applied to the maximum steering force.
//        //    netSteeringForce.x += (int)steeringDirection 
//        //                        * cohesionFactor 
//        //                        * Vector3.Angle(heading, displacement) 
//        //                        / Mathf.PI;

//        //netSteeringForce.x += (avgPosition.x - transform.position.x) * cohesionFactor;
//        //netSteeringForce.y += (avgPosition.y - transform.position.y) * cohesionFactor;
//    }

//    private Vector3 CalculateAlignment()
//    {
//        // Obtain the average velocity/heading vector using the same logic
//        // applied in CalculateCohesion().
//        //avgVelocity = (totalVelocity / neighbourCount).normalized;
//        Vector3 avgVelocity = Vector3.zero;

//        if (neighbourCount == 0) return avgVelocity;
            
//        avgVelocity = totalVelocity / neighbourCount;

//        return (avgVelocity - Velocity) * alignmentWeight;
//        //netSteeringForce.x += (avgVelocity.x - Rb.velocity.x) * alignmentFactor;
//        //netSteeringForce.y += (avgVelocity.y - Rb.velocity.y) * alignmentFactor;
//        //// Get the heading of this agent, normalised.
//        //heading = Rb.velocity.normalized;

//        //// The following code is similar to that of CalculateCohesion().
//        //if (Vector3.SignedAngle(heading, avgVelocity, Vector3.up) < 0)
//        //    steeringDirection = SteeringDirection.Left;
//        //else steeringDirection = SteeringDirection.Right;

//        //// This agent should steer to match the average heading of its
//        //// neighbours.
//        //if (Mathf.Abs(Vector3.Dot(heading, avgVelocity)) < 1)
//        //    netSteeringForce.x += (int)steeringDirection
//        //                        * alignmentFactor
//        //                        * Vector3.Angle(heading, avgVelocity)
//        //                        / Mathf.PI;
//    }

//    private Vector3 CalculateSeparation(/*float signedAngle*/)
//    {
//        return totalPositionAway * separationWeight;

//        //// This method assumes the flockmate under consideration is in view.
//        //// If it is also within a set distance, the flockmate presents a
//        //// potential collision. Calculate and apply a steering correction.
//        //if (distance.magnitude <= length * separationDistanceFactor)
//        //{
//        //    //// steeringDirection takes on the opposite sense to the cohesion
//        //    //// and alignment calculations.
//        //    //if (signedAngle < 0) steeringDirection = SteeringDirection.Right;
//        //    //else steeringDirection = SteeringDirection.Left;

//        //    //// The corrective steering force is inversely proportional to the
//        //    //// separation distance.
//        //    //netSteeringForce.x += (int)steeringDirection
//        //    //                    * (length * separationFactor)
//        //    //                    / distance.magnitude;
//        //    netSteeringForce.x += distance.x * separationFactor;
//        //    netSteeringForce.y += distance.y * separationFactor;
//        //}
//    }

//    private void RetainOnScreen(Vector3 newVelocity)
//    {
//        //posRelativeToViewport = Camera.main.WorldToScreenPoint(transform.position);
//        //Debug.Log(posRelativeToViewport.x + "x" + posRelativeToViewport.y);

//        //if (posRelativeToViewport.x < Camera.main.WorldToViewportPoint(new Vector3(-Screen.width / 2, 0, 0)).x)
//        //    netSteeringForce.x += screenRetentionFactor;
//        //else if (posRelativeToViewport.x > Camera.main.WorldToViewportPoint(new Vector3(Screen.width / 2, 0, 0)).x)
//        //    netSteeringForce.x -= screenRetentionFactor;

//        //if (posRelativeToViewport.y < Camera.main.WorldToViewportPoint(new Vector3(0, -Screen.height / 2, 0)).y)
//        //    netSteeringForce.y += screenRetentionFactor;
//        //else if (posRelativeToViewport.y > Camera.main.WorldToViewportPoint(new Vector3(0, Screen.height / 2, 0)).y)
//        //    netSteeringForce.y -= screenRetentionFactor;



//        // ----- USE THIS -----
//        if (transform.position.x < -sceneDimensions.x / 2)
//            newVelocity.x += screenRetentionFactor;
//        else if (transform.position.x > sceneDimensions.x / 2)
//            newVelocity.x -= screenRetentionFactor;

//        if (transform.position.y < -sceneDimensions.y / 2)
//            newVelocity.y += screenRetentionFactor;
//        else if (transform.position.y > sceneDimensions.y / 2)
//            newVelocity.y -= screenRetentionFactor;



//        //// 7.2, 4
//        //if (transform.position.x < -7.2f)
//        //    netSteeringForce.x += screenRetentionFactor;
//        //else if (transform.position.x > 7.2f)
//        //    netSteeringForce.x -= screenRetentionFactor;

//        //if (transform.position.y < -4)
//        //    netSteeringForce.y += screenRetentionFactor;
//        //else if (transform.position.y > 4)
//        //    netSteeringForce.y -= screenRetentionFactor;
//    }

//    // Updates Position and Velocity of the Boid and moves the transform with the geometry attached.
//    private void UpdateAgent(Vector3 newVelocity, Vector3 newPosition)
//    {
//        transform.position = newPosition;
//        Velocity = newVelocity;
//        transform.rotation = Quaternion.LookRotation(newVelocity.normalized);
//    }

//    private Vector3 LimitSpeed(Vector3 newVelocity)
//    {
//        //Rb.velocity = Rb.velocity.normalized * maxSpeed;
//        if (newVelocity.magnitude > maxSpeed)
//            return newVelocity.normalized * maxSpeed;
//        else return newVelocity;
//    }

//    private Vector3 LimitRotation(Vector3 newVelocity)
//    {
//        return Vector3.RotateTowards(Velocity, newVelocity, maxAngle * Mathf.Deg2Rad, maxSpeed);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        // TODO: Wraparound???
//        CheckNeighbours();


//        if (neighbourCount > 0)
//        {
//            //CalculateCohesion();
//            //CalculateAlignment();
//            Vector3 cohesionVector = CalculateCohesion();
//            Vector3 alignmentVector = CalculateAlignment();
//            Vector3 separationVector = CalculateSeparation();

//            Vector3 newVelocity = cohesionVector + alignmentVector + separationVector;
//            newVelocity = LimitSpeed(newVelocity);
//            newVelocity = LimitRotation(newVelocity);
//            Vector3 newPosition = transform.position + newVelocity;
//            RetainOnScreen(newVelocity);

//            UpdateAgent(newVelocity, newPosition);
            
//            if (log)
//            {
//                Debug.Log("cohesion vector: " + cohesionVector);
//                Debug.Log("alignment vector: " + alignmentVector);
//                Debug.Log("separation vector: " + separationVector);
//            }
//        }



//        //sfApplicationPoint = new Vector3(transform.position.x, transform.position.y + length / 2, transform.position.z);
//        //Debug.Log("Net steering force: " + netSteeringForce);
//        //Debug.Log("Steering force applied at: " + sfApplicationPoint);
//        //Rb.AddForceAtPosition(netSteeringForce, sfApplicationPoint);
//        //Rb.AddForce(netSteeringForce);
//        //Rb.position += netSteeringForce;

//        // Regulate speed.
//        //if (Rb.velocity.magnitude > maxSpeed)
//        //    Rb.velocity = Rb.velocity.normalized * maxSpeed;
//    }

//    // TODO: max speed limiter
//}
