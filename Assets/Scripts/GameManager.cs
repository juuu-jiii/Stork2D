using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private enum Themes
    {
        Artistic,
        Realistic
    }

    private enum SimulationStates
    {
        InProgress,
        Completed
    }

    [SerializeField] private GameObject cursorTrackerGO;

    #region Themes
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

    #region UI
    [SerializeField] private GameObject simInProgressUI;
    [SerializeField] private GameObject simCompletedUI;
    [SerializeField] private TextMeshProUGUI simulationProgress;
    private float maxWaitTime;
    #endregion

    private CursorTracker cursorTracker;
    private Themes currentTheme;
    private SimulationStates currentState;

    public static List<Agent2> agents;

    // Start is called before the first frame update
    void Start()
    {
        currentState = SimulationStates.InProgress;
        currentTheme = Themes.Artistic;
        cursorTracker = cursorTrackerGO.GetComponent<CursorTracker>();

        Init();
    }

    private void Init()
    {
        // Show/hide appropriate UI components.
        simInProgressUI.SetActive(true);
        simCompletedUI.SetActive(false);
        
        // FindObjectsOfType() returns Object[]. Cast to Agent2[], then use
        // LINQ to convert to a List.
        agents = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList();
        agentsMaster = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList();

        // Enable and initialise each agent.
        foreach (Agent2 agent in agents)
        {
            agent.enabled = true;
            agent.Init();
        }
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

    private void ToggleUI()
    {
        switch (currentState)
        {
            case SimulationStates.InProgress:
                if (simInProgressUI.activeInHierarchy) simInProgressUI.SetActive(false);
                else simInProgressUI.SetActive(true);
                break;
            case SimulationStates.Completed:
                if (simCompletedUI.activeInHierarchy) simCompletedUI.SetActive(false);
                else simCompletedUI.SetActive(true);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SwitchThemes();
        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleUI();

        switch (currentState)
        {
            case SimulationStates.InProgress:
                if (agents.Count == 0)
                {
                    simulationProgress.text = string.Format(
                        "remaining: {0} of {1}\nmax wait time: {2}s",
                        agents.Count,
                        agentsMaster.Count,
                        0);

                    // Show/hide appropriate UI components.
                    simInProgressUI.SetActive(false);
                    simCompletedUI.SetActive(true);

                    currentState = SimulationStates.Completed;
                    //if (Input.GetKeyDown(KeyCode.Return)) Init();
                }
                else
                {
                    maxWaitTime = float.MinValue;

                    for (int i = agents.Count - 1; i >= 0; i--)
                    {
                        // Update remaining time. This must take place either before
                        // removal or in the else clause to avoid an
                        // IndexOutOfRangeException.
                        if (agents[i].RemainingSeconds > maxWaitTime)
                            maxWaitTime = agents[i].RemainingSeconds;

                        // Deactivate and remove agents that have reached the goal or died.
                        if (agents[i].ReachedGoal || !agents[i].Alive)
                        {
                            agents[i].enabled = false;
                            agents.RemoveAt(i);
                        }
                        // Otherwise, update cursor position.
                        // Must be placed in an else block to prevent an
                        // IndexOutOfRangeException from being thrown in the event the 
                        // agent at the last index gets removed and the program tries to
                        // access its script.
                        else agents[i].CursorPosWorld = cursorTracker.TrackCursor();
                    }

                    simulationProgress.text = string.Format(
                        "remaining: {0} of {1}\nmax wait time: {2}s",
                        agents.Count,
                        agentsMaster.Count,
                        (int)maxWaitTime);
                }

                break;
            case SimulationStates.Completed:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Init();
                    currentState = SimulationStates.InProgress;
                }
                break;
        }

    }
}
