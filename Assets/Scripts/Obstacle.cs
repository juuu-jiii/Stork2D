using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool InUse { get; private set; }

    private void Start()
    {
        InUse = false;
    }

    public void Place()
    {
        InUse = true;
    }

    private void OnMouseDown()
    {
        if (GameManager.CurrentState == SimulationStates.ObstaclePlacement 
            && Input.GetMouseButtonDown(1))
            InUse = false;
    }
}
