using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer {

    public enum FilterType { Simple, Ridged };

    public bool enabled = true;
    public FilterType filterType;
    public bool useFirstLayerAsMask;
    public float strength = 1;
    [Range(1, 8)]
    public int octaveCount = 1;
    public float baseFrequency = 1;
    public float lacunarity = 2;
    public float persistence = 0.5f;
    public Vector3 centre;
    public float minNoise;

    [ConditionalHide("filterType", 1, true)]
    public float weightMultiplier = 0.8f;
}