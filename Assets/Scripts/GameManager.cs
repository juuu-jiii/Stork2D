using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cursorTrackerGO;
    private CursorTracker cursorTracker;

    public static Agent2[] agents;

    // Start is called before the first frame update
    void Start()
    {
        cursorTracker = cursorTrackerGO.GetComponent<CursorTracker>();
        agents = (Agent2[])FindObjectsOfType(typeof(Agent2));
    }

    // Update is called once per frame
    void Update()
    {
        // Screen wraparound code
        foreach (Agent2 agent in agents)
        {
            agent.CursorPosWorld = cursorTracker.TrackCursor();
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
