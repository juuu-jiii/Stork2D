using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CustomisableData
{
    public float VisualRange { get; private set; }
    public float MinDistance { get; private set; }
    public float Cohesion { get; private set; }
    public float Alignment { get; private set; }
    public float Separation { get; private set; }
    public float Avoidance { get; private set; }
    public float MaxSpeed { get; private set; }
    public float MaxSeeAhead { get; private set; }
    public float Lifespan { get; private set; }

    public CustomisableData(
         float visualRange,
         float minDistance,
         float cohesion,
         float alignment,
         float separation,
         float avoidance,
         float maxSpeed,
         float maxSeeAhead,
         float lifespan)
    {
        VisualRange = visualRange;
        MinDistance = minDistance;
        Cohesion = cohesion;
        Alignment = alignment;
        Separation = separation;
        Avoidance = avoidance;
        MaxSpeed = maxSpeed;
        MaxSeeAhead = maxSeeAhead;
        Lifespan = lifespan;
    }
}
