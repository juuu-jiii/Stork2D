using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomisableSegment : MonoBehaviour
{
    [SerializeField] private TrailSegments thisSegment;

    public void SetCurrentSegment()
    {
        TrailCustomisation.currentSegment = thisSegment;
    }
}
