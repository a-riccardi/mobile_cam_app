using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PhotoFrame
{
    public string Name { get { return name; } }
    public Texture2D Frame { get { return frame; } }
    public Vector4 UV { get { return uv; } }

    [SerializeField] string name;
    [SerializeField] Texture2D frame;
    [SerializeField] Vector4 uv;
}
