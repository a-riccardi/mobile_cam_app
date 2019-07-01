using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorCorrectionData
{
    public ColorCorrectionPreset PresetName { get { return presetName; } }
    public Texture2D ShadowRamp { get { return shadowRamp; } }
    public Texture2D LightRamp { get { return lightRamp; } }
    public float ShadowIntensity { get { return shadowIntensity; } }
    public float LightIntensity { get { return lightIntensity; } }

    [SerializeField] ColorCorrectionPreset presetName;
    [SerializeField] Texture2D shadowRamp;
    [SerializeField] Texture2D lightRamp;
    [SerializeField] float shadowIntensity;
    [SerializeField] float lightIntensity;
}
