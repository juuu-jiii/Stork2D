using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cursorTrackerGO;
    private CursorTracker cursorTracker;

    public static List<Agent2> agents;

    // Start is called before the first frame update
    void Start()
    {
        cursorTracker = cursorTrackerGO.GetComponent<CursorTracker>();

        // FindObjectsOfType() returns Object[]. Cast to Agent2[], then use
        // LINQ to convert to a List.
        agents = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList<Agent2>();
    }

    // Update is called once per frame
    void Update()
    {
        // Screen wraparound code
        //foreach (Agent2 agent in agents)
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            // Destroy and remove and agents that have reached the goal.
            if (agents[i].ReachedGoal)
            {
                Destroy(agents[i]);
                agents.RemoveAt(i);
            }
            // Otherwise, update cursor position.
            // Must be placed in an else block to prevent an
            // IndexOutOfRangeException from being thrown in the event the 
            // agent at the last index gets removed and the program tries to
            // access its script.
            else agents[i].CursorPosWorld = cursorTracker.TrackCursor();


            //// Along x-axis
            //if (agent.transform.position.x > Screen.width) 
            //    agent.transform.position = new Vector3(
            //        0, 
            //        agent.transform.position.y, 
            //        agent.transform.position.z);
            //else if (agent.transform.position.x < 0)
            //    agent.transform.position = new Vector3(
            //        Screen.width,
            //        agent.transform.position.y,
            //        agent.transform.position.z);

            //// Along y-axis
            //if (agent.transform.position.y > Screen.height)
            //    agent.transform.position = new Vector3(
            //       agent.transform.position.x,
            //       0,
            //       agent.transform.position.z);
            //else if (agent.transform.position.y < 0)
            //    agent.transform.position = new Vector3(
            //        agent.transform.position.x,
            //        Screen.height,
            //        agent.transform.position.z);
        }
    }
}
