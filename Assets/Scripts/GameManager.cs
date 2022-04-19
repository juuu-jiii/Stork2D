using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum Themes
    {
        Artistic,
        Realistic
    }

    [SerializeField] private GameObject cursorTrackerGO;

    #region Theme-Related Components
    [SerializeField] private SpriteRenderer backdrop;
    [SerializeField] private GameObject obstacles;

    [SerializeField] private Color colourRealistic;
    [SerializeField] private Color colourArtistic;
    [SerializeField] private Color agentColourRealistic;
    [SerializeField] private Color agentColourArtistic;
    [SerializeField] private Color obstacleColour;

    /// <summary>
    /// Backing List so trails can be enabled/disabled correctly.
    /// </summary>
    private List<Agent2> agentsMaster;
    #endregion

    private CursorTracker cursorTracker;
    private Themes currentTheme;

    public static List<Agent2> agents;

    // Start is called before the first frame update
    void Start()
    {
        currentTheme = Themes.Artistic;
        cursorTracker = cursorTrackerGO.GetComponent<CursorTracker>();

        // FindObjectsOfType() returns Object[]. Cast to Agent2[], then use
        // LINQ to convert to a List.
        agents = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList<Agent2>();
        agentsMaster = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList<Agent2>();
    }

    /// <summary>
    /// Switch between "artistic" and "realistic" themes.
    /// </summary>
    private void SwitchThemes()
    {
        // Flip-flop
        switch (currentTheme)
        {
            case Themes.Artistic:
                currentTheme = Themes.Realistic;
                backdrop.color = colourArtistic;

                // Use this to foreach through a parent GameObject's children.
                // If using a for-loop, using transform.childCount and 
                // transform.GetChild().
                foreach (Transform obstacle in obstacles.transform)
                {
                    Color newColour = obstacle.GetComponent<SpriteRenderer>().color;
                    newColour.a = 0;
                    obstacle.GetComponent<SpriteRenderer>().color = newColour;
                }
                    //obstacle.GetComponent<SpriteRenderer>().color = new Color(
                    //    obstacleColour.r,
                    //    obstacleColour.g,
                    //    obstacleColour.b,
                    //    0);

                foreach (Agent2 agent in agentsMaster)
                { 
                    //agent.GetComponent<TrailRenderer>().enabled = false;
                    Color newColour = agent.GetComponent<TrailRenderer>().startColor;
                    newColour.a = 0;
                    agent.GetComponent<TrailRenderer>().startColor = newColour;
                    agent.GetComponent<TrailRenderer>().endColor = newColour;

                    agent.realistic = true;
                    agent.GetComponent<SpriteRenderer>().color = agent.colourRealistic;
                }

                break;
            case Themes.Realistic:
                currentTheme = Themes.Artistic;
                backdrop.color = colourRealistic;

                foreach (Transform obstacle in obstacles.transform)
                {
                    Color newColour = obstacle.GetComponent<SpriteRenderer>().color;
                    newColour.a = 1;
                    obstacle.GetComponent<SpriteRenderer>().color = newColour;
                }
                    //obstacle.GetComponent<SpriteRenderer>().color = new Color(
                    //    obstacleColour.r,
                    //    obstacleColour.g,
                    //    obstacleColour.b,
                    //    1);

                foreach (Agent2 agent in agentsMaster)
                {
                    //agent.GetComponent<TrailRenderer>().enabled = true;
                    Color newColour = agent.GetComponent<TrailRenderer>().startColor;
                    newColour.a = 1;
                    agent.GetComponent<TrailRenderer>().startColor = newColour;
                    agent.GetComponent<TrailRenderer>().endColor = newColour;

                    agent.realistic = false;
                    agent.GetComponent<SpriteRenderer>().color = agent.colourArtistic;
                }

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchThemes();
        }

        // Screen wraparound code
        //foreach (Agent2 agent in agents)
        for (int i = agents.Count - 1; i >= 0; i--)
        {
            // Destroy and remove and agents that have reached the goal.
            if (agents[i].ReachedGoal)
            {
                //Destroy(agents[i]);
                agents[i].enabled = false;
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
