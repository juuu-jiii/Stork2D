using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Customisable segments of the trail.
/// </summary>
public enum TrailSegments
{
    Start,
    Middle,
    End
}

public class ColourProfile
{
    /// <summary>
    /// Dictionary mapping enum of customisables to colours.
    /// </summary>
    public Dictionary<TrailSegments, Color> profile;

    /// <summary>
    /// Constructor initialising customisable colours to black.
    /// </summary>
    public ColourProfile()
    {
        profile = new Dictionary<TrailSegments, Color>()
        {
            { TrailSegments.Start, Color.black },
            { TrailSegments.Middle, Color.black },
            { TrailSegments.End, Color.black }
        };
    }
}
