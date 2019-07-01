using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] OptionPanel requestedPanel;

    public OptionPanel RequestedPanel { get { return requestedPanel; } }

    public void OnButtonPressed()
    {
        UIManager.OpenSettingsMenuPanel(requestedPanel);
    }
}
