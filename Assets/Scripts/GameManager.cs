using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public enum SimulationStates
{
    InProgress,
    Completed,
    ObstaclePlacement
}

public class GameManager : MonoBehaviour
{
    private enum Themes
    {
        Artistic,
        Plain
    }


    [SerializeField] private GameObject cursorTrackerGO;
    [SerializeField] private int maxObstacles;

    #region Themes
    [SerializeField] private SpriteRenderer backdrop;
    [SerializeField] private GameObject obstacleTemplate;
    private List<Obstacle> obstacles;

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

    #region UI Panels
    [SerializeField] private GameObject simInProgressUI;
    [SerializeField] private GameObject simCompletedUI;
    [SerializeField] private GameObject obstaclePlacementUI;
    [SerializeField] private TextMeshProUGUI simulationProgress;
    private float maxWaitTime;
    #endregion

    #region Sliders
    [SerializeField] private Slider visualRange;
    [SerializeField] private Slider minDistance;
    [SerializeField] private Slider cohesion;
    [SerializeField] private Slider alignment;
    [SerializeField] private Slider separation;
    [SerializeField] private Slider avoidance;
    [SerializeField] private Slider maxSpeed;
    [SerializeField] private Slider maxSeeAhead;
    [SerializeField] private Slider lifespan;
    [SerializeField] private TextMeshProUGUI visualRangeValueLabel;
    [SerializeField] private TextMeshProUGUI minDistanceValueLabel;
    [SerializeField] private TextMeshProUGUI cohesionValueLabel;
    [SerializeField] private TextMeshProUGUI alignmentValueLabel;
    [SerializeField] private TextMeshProUGUI separationValueLabel;
    [SerializeField] private TextMeshProUGUI avoidanceValueLabel;
    [SerializeField] private TextMeshProUGUI maxSpeedValueLabel;
    [SerializeField] private TextMeshProUGUI maxSeeAheadValueLabel;
    [SerializeField] private TextMeshProUGUI lifespanValueLabel;
    #endregion

    [SerializeField] private GameObject trailColour;
    private TrailCustomisation trailCustomisation;
    private CustomisableData customisableData;
    private Gradient gradient;
    private GradientAlphaKey[] alphaKeys;

    private bool obstaclesHidden;
    private CursorTracker cursorTracker;
    private Themes currentTheme;
    public static SimulationStates CurrentState { get; private set; }

    public static List<Agent2> agents;

    // Start is called before the first frame update
    void Start()
    {
        CurrentState = SimulationStates.Completed;
        currentTheme = Themes.Artistic;
        cursorTracker = cursorTrackerGO.GetComponent<CursorTracker>();
        trailCustomisation = trailColour.GetComponent<TrailCustomisation>();
        alphaKeys = new GradientAlphaKey[3];

        obstacles = new List<Obstacle>();
        GameObject temp;
        for (int i = 0; i < maxObstacles; i++)
        {
            temp = Instantiate(obstacleTemplate);
            temp.SetActive(false);
            obstacles.Add(temp.GetComponent<Obstacle>());
        }

        obstaclesHidden = false;

        // Commit changes when slider values are changed.
        visualRange.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        minDistance.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        cohesion.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        alignment.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        separation.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        avoidance.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        maxSpeed.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        maxSeeAhead.onValueChanged.AddListener(delegate { SetCustomisableData(); });
        lifespan.onValueChanged.AddListener(delegate { SetCustomisableData(); });

        //// Populate the colour keys at the relative time 0 and 1 (0 and 100%).
        //colourKeys[0].color = segments[0].image.color;
        //colourKeys[0].time = 0.0f;
        //colourKeys[1].color = segments[1].image.color;
        //colourKeys[1].time = 0.5f;
        //colourKeys[2].color = segments[2].image.color;
        //colourKeys[2].time = 1.0f;

        

        //TrailGradient.SetKeys(colourKeys, alphaKeys);
    }

    private void SetAlphaZero()
    {
        // Populate the alpha keys at relative time 0 and 1 (0 and 100%).
        alphaKeys[0].alpha = 0;
        alphaKeys[0].time = 0.0f;
        alphaKeys[1].alpha = 0;
        alphaKeys[1].time = 0.5f;
        alphaKeys[2].alpha = 0;
        alphaKeys[2].time = 1.0f;
    }

    private void SetAlphaOne()
    {
        // Populate the alpha keys at relative time 0 and 1 (0 and 100%).
        alphaKeys[0].alpha = 1;
        alphaKeys[0].time = 0.0f;
        alphaKeys[1].alpha = 1;
        alphaKeys[1].time = 0.5f;
        alphaKeys[2].alpha = 1;
        alphaKeys[2].time = 1.0f;
    }

    private void SetCustomisableData()
    {
        customisableData = new CustomisableData(
            visualRange.value,
            minDistance.value,
            cohesion.value,
            alignment.value,
            separation.value,
            avoidance.value,
            maxSpeed.value,
            maxSeeAhead.value,
            lifespan.value);
    }

    private void Init(Gradient trailGradient)
    {
        //// Show/hide appropriate UI components.
        //simInProgressUI.SetActive(true);
        //simCompletedUI.SetActive(false);
        
        // FindObjectsOfType() returns Object[]. Cast to Agent2[], then use
        // LINQ to convert to a List.
        agents = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList();
        agentsMaster = ((Agent2[])FindObjectsOfType(typeof(Agent2))).ToList();

        SetCustomisableData();

        switch (currentTheme)
        {
            case Themes.Artistic:
                // Enable and initialise each agent as usual.
                foreach (Agent2 agent in agents)
                {
                    agent.enabled = true;
                    agent.Init(trailGradient);
                    agent.SetData(customisableData);
                }

                break;
            case Themes.Plain:
                // Enable and initialise each agent, but hide trails.
                foreach (Agent2 agent in agents)
                {
                    //trailCustomisation.TrailGradient.alphaKeys[0].alpha = 0;
                    //trailCustomisation.TrailGradient.alphaKeys[1].alpha = 0;
                    //trailCustomisation.TrailGradient.alphaKeys[2].alpha = 0;
                    SetAlphaZero();
                    trailCustomisation.TrailGradient.SetKeys(
                        trailCustomisation.TrailGradient.colorKeys,
                        alphaKeys);

                    agent.enabled = true;
                    agent.Init(trailGradient);
                    agent.SetData(customisableData);

                    // Update colour as necessary.
                    agent.GetComponent<SpriteRenderer>().color = agent.colourPlain;
                    //Color newColour = agent.GetComponent<TrailRenderer>().startColor;
                    //newColour.a = 0;
                    //agent.GetComponent<TrailRenderer>().startColor = newColour;
                    //agent.GetComponent<TrailRenderer>().colorGradient
                    //agent.GetComponent<TrailRenderer>().endColor = newColour;
                    //agent.GetComponent<TrailRenderer>().enabled = false;
                }

                break;
        }
    }

    /// <summary>
    /// Switch between "artistic" and "plain" themes.
    /// </summary>
    private void SwitchThemes()
    {
        // Flip-flop
        switch (currentTheme)
        {
            case Themes.Artistic:
                currentTheme = Themes.Plain;
                backdrop.color = colourArtistic;

                //// Use this to foreach through a parent GameObject's children.
                //// If using a for-loop, using transform.childCount and 
                //// transform.GetChild().
                //foreach (Obstacle obstacle in obstacles)
                //{
                //    Color newColour = obstacle.GetComponent<SpriteRenderer>().color;
                //    newColour.a = 0;
                //    obstacle.GetComponent<SpriteRenderer>().color = newColour;
                //    //obstacle.GetComponent<SpriteRenderer>().color = backdrop.color;
                //}

                if (agentsMaster != null)
                {
                    foreach (Agent2 agent in agentsMaster)
                    {
                        //agent.GetComponent<TrailRenderer>().enabled = false;
                        //Color newColour = agent.GetComponent<TrailRenderer>().startColor;
                        //newColour.a = 0;
                        //agent.GetComponent<TrailRenderer>().startColor = newColour;
                        //agent.GetComponent<TrailRenderer>().endColor = newColour;
                        //trailCustomisation.TrailGradient.alphaKeys[0].alpha = 0;
                        //trailCustomisation.TrailGradient.alphaKeys[1].alpha = 0;
                        //trailCustomisation.TrailGradient.alphaKeys[2].alpha = 0;
                        SetAlphaZero();
                        trailCustomisation.TrailGradient.SetKeys(
                            trailCustomisation.TrailGradient.colorKeys,
                            alphaKeys);
                        agent.Trail.colorGradient = trailCustomisation.TrailGradient;

                        agent.plain = true;
                        agent.GetComponent<SpriteRenderer>().color = agent.colourPlain;
                    }
                }

                break;
            case Themes.Plain:
                currentTheme = Themes.Artistic;
                backdrop.color = colourRealistic;

                //// Only show obstacles if they were not hidden before.
                //if (!obstaclesHidden)
                //{
                //    foreach (Obstacle obstacle in obstacles)
                //    {
                //        Color newColour = obstacle.GetComponent<SpriteRenderer>().color;
                //        newColour.a = 1;
                //        obstacle.GetComponent<SpriteRenderer>().color = newColour;
                //        //obstacle.GetComponent<SpriteRenderer>().color = obstacleColour;
                //    }
                //}

                if (agentsMaster != null)
                {
                    foreach (Agent2 agent in agentsMaster)
                    {
                        //agent.GetComponent<TrailRenderer>().enabled = true;
                        //Color newColour = agent.GetComponent<TrailRenderer>().startColor;
                        //newColour.a = 1;
                        //agent.GetComponent<TrailRenderer>().startColor = newColour;
                        //agent.GetComponent<TrailRenderer>().endColor = newColour;

                        //trailCustomisation.TrailGradient.alphaKeys[0].alpha = 1;
                        //trailCustomisation.TrailGradient.alphaKeys[1].alpha = 1;
                        //trailCustomisation.TrailGradient.alphaKeys[2].alpha = 1;
                        SetAlphaOne();
                        trailCustomisation.TrailGradient.SetKeys(
                            trailCustomisation.TrailGradient.colorKeys,
                            alphaKeys);
                        agent.Trail.colorGradient = trailCustomisation.TrailGradient;

                        agent.plain = false;
                        agent.GetComponent<SpriteRenderer>().color = agent.colourArtistic;
                    }
                }

                break;
        }
    }

    private void ToggleUI()
    {
        switch (CurrentState)
        {
            case SimulationStates.InProgress:
                if (simInProgressUI.activeInHierarchy) simInProgressUI.SetActive(false);
                else simInProgressUI.SetActive(true);
                break;
            case SimulationStates.Completed:
                if (simCompletedUI.activeInHierarchy) simCompletedUI.SetActive(false);
                else simCompletedUI.SetActive(true);
                break;
            case SimulationStates.ObstaclePlacement:
                if (obstaclePlacementUI.activeInHierarchy) obstaclePlacementUI.SetActive(false);
                else obstaclePlacementUI.SetActive(true);
                break;
        }
    }

    private void HideObstacles()
    {
        //// This code only runs in artistic mode.
        //if (currentTheme == Themes.Artistic)
        //{
            // Show
            if (obstaclesHidden)
            {
                foreach (Obstacle obstacle in obstacles)
                {
                    Color newColour = obstacle.GetComponent<SpriteRenderer>().color;
                    newColour.a = 1;
                    obstacle.GetComponent<SpriteRenderer>().color = newColour;
                }
            }
            // Hide
            else
            {
                foreach (Obstacle obstacle in obstacles)
                {
                    Color newColour = obstacle.GetComponent<SpriteRenderer>().color;
                    newColour.a = 0;
                    obstacle.GetComponent<SpriteRenderer>().color = newColour;
                }
            }

            // Flip flop
            obstaclesHidden = !obstaclesHidden;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SwitchThemes();
        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleUI();

        switch (CurrentState)
        {
            case SimulationStates.InProgress:
                if (agents.Count == 0)
                {
                    simulationProgress.text = "remaining: 0s";

                    // Show/hide appropriate UI components.
                    simInProgressUI.SetActive(false);
                    simCompletedUI.SetActive(true);
                    obstaclePlacementUI.SetActive(false);

                    CurrentState = SimulationStates.Completed;
                    //if (Input.GetKeyDown(KeyCode.Return)) Init();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.RightShift)) HideObstacles();

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
                        "remaining: {0}s", 
                        (int)maxWaitTime);
                }

                break;
            case SimulationStates.Completed:
                // Only allow single press of either key.
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    // Show/hide appropriate UI components.
                    simInProgressUI.SetActive(false);
                    simCompletedUI.SetActive(false);
                    obstaclePlacementUI.SetActive(true);

                    foreach (Agent2 agent in agentsMaster)
                        agent.Trail.Clear();

                    CurrentState = SimulationStates.ObstaclePlacement;
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    // Show/hide appropriate UI components.
                    simInProgressUI.SetActive(true);
                    simCompletedUI.SetActive(false);
                    obstaclePlacementUI.SetActive(false);

                    Init(trailCustomisation.TrailGradient);
                    CurrentState = SimulationStates.InProgress;
                }

                if (Input.GetKeyDown(KeyCode.RightShift)) HideObstacles();

                // Update label values each frame, normalising values so that
                // they show up between 0.00 and 1.00
                visualRangeValueLabel.text = ((visualRange.value - visualRange.minValue) / (visualRange.maxValue - visualRange.minValue)).ToString("F2");
                minDistanceValueLabel.text = ((minDistance.value - minDistance.minValue) / (minDistance.maxValue - minDistance.minValue)).ToString("F2");
                cohesionValueLabel.text = ((cohesion.value - cohesion.minValue) / (cohesion.maxValue - cohesion.minValue)).ToString("F2");
                alignmentValueLabel.text = ((alignment.value - alignment.minValue) / (alignment.maxValue - alignment.minValue)).ToString("F2");
                separationValueLabel.text = ((separation.value - separation.minValue) / (separation.maxValue - separation.minValue)).ToString("F2");
                avoidanceValueLabel.text = ((avoidance.value - avoidance.minValue) / (avoidance.maxValue - avoidance.minValue)).ToString("F2");
                maxSpeedValueLabel.text = ((maxSpeed.value - maxSpeed.minValue) / (maxSpeed.maxValue - maxSpeed.minValue)).ToString("F2");
                maxSeeAheadValueLabel.text = ((maxSeeAhead.value - maxSeeAhead.minValue) / (maxSeeAhead.maxValue - maxSeeAhead.minValue)).ToString("F2");
                lifespanValueLabel.text = lifespan.value.ToString("F0") + "s";

                break;
            case SimulationStates.ObstaclePlacement:
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    // Show/hide appropriate UI components.
                    simInProgressUI.SetActive(false);
                    simCompletedUI.SetActive(true);
                    obstaclePlacementUI.SetActive(false);

                    CurrentState = SimulationStates.Completed;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    foreach (Obstacle obstacle in obstacles)
                    {
                        if (!obstacle.InUse)
                        {
                            obstacle.gameObject.SetActive(true);
                            obstacle.Place();
                            break;
                        }
                    }
                }

                foreach (Obstacle obstacle in obstacles)
                    if (!obstacle.InUse && obstacle.gameObject.activeInHierarchy)
                    {
                        obstacle.gameObject.SetActive(false);
                        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        //obstacle.transform.position = new Vector3(
                        //    worldPos.x,
                        //    worldPos.y,
                        //    0);
                    }

                break;
        }

    }
}
