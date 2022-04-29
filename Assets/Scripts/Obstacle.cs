using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool InUse { get; private set; }

    //private void Start()
    //{
    //    InUse = false;
    //}

    public void Place()
    {
        InUse = true;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(
            worldPos.x,
            worldPos.y,
            0);
    }

    private void OnMouseDown()
    {
        if (GameManager.CurrentState == SimulationStates.ObstaclePlacement
            && Input.GetMouseButtonDown(0))
            InUse = false;
    }
}
