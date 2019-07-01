using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResolutionButton : MonoBehaviour
{
    [SerializeField] ResolutionQuality requestedQuality;

    public void RequestQuality()
    {
        CameraManager.singleton.SetResolution(requestedQuality);
    }
}
