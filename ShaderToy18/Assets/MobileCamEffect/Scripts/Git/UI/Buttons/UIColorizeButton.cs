using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorizeButton : MonoBehaviour
{

    [SerializeField] Button button;
    [SerializeField] ColorCorrectionPreset requestedPreset;

    public ColorCorrectionPreset RequestedPanel { get { return requestedPreset; } }

    public void OnButtonPressed()
    {
        CameraManager.singleton.ChangeColorizePreset(requestedPreset);
    }
}
