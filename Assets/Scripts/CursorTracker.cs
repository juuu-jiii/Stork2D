using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTracker : MonoBehaviour
{
    Vector3 cursorPosScreen;
    Vector3 cursorPosWorld;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TrackCursor();
    }

    public Vector3 TrackCursor()
    {
        // Input.mousePosition's z-value is always 0. Set it to reflect the
        // Camera's height from the ground for accurate tracking of mouse
        // click positions.
        cursorPosScreen = Input.mousePosition;
        cursorPosScreen.z = Camera.main.transform.position.z;

        // Input.mousePosition returns data in terms of screen coordinates.
        // Convert this to world coordinates.
        cursorPosWorld = Camera.main.ScreenToWorldPoint(cursorPosScreen);

        return cursorPosWorld;

        //Debug.Log(cursorPosWorld);
    }
}
