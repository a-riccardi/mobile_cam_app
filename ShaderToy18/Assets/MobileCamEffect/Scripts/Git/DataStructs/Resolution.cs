using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Resolution
{
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public ResolutionQuality Quality { get { return quality; } }
    
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] ResolutionQuality quality;
}
