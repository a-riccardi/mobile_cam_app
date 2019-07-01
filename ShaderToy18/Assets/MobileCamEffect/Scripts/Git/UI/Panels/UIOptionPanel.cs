using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionPanel : UIPanel
{
    [SerializeField] Button nextOptButton;
    [SerializeField] Button previousOptButton;

    [SerializeField] UIPanel[] optionSubpanel;

    int subpanelIndex;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Initialize();
        CheckButtonState();
    }

    void Initialize()
    {
        subpanelIndex = 0;

        optionSubpanel[0].gameObject.SetActive(true);
        OptionSubpanelLeft(optionSubpanel[0]);
        optionSubpanel[0].Open(true);

        for (int i = 1; i < optionSubpanel.Length; i++)
        {
            optionSubpanel[i].gameObject.SetActive(true);
            OptionSubpanelRight(optionSubpanel[i]);
            optionSubpanel[i].Close(true);
        }
    }

    void CheckButtonState()
    {
        if (optionSubpanel.Length <= 1)
        {
            previousOptButton.interactable = false;
            nextOptButton.interactable = false;
            return;
        }

        if (subpanelIndex == 0)
        {
            previousOptButton.interactable = false;
            nextOptButton.interactable = true;
        }
        else if (subpanelIndex == optionSubpanel.Length - 1)
        {
            previousOptButton.interactable = true;
            nextOptButton.interactable = false;
        }
        else
        {
            previousOptButton.interactable = true;
            nextOptButton.interactable = true;
        }
    }

    void OptionSubpanelLeft(UIPanel subPanel)
    {
        subPanel.SetupPositions(Vector3.zero, new Vector3(-UIManager.ScreenWidth, 0.0f, 0.0f));
    }

    void OptionSubpanelRight(UIPanel subPanel)
    {
        subPanel.SetupPositions(Vector3.zero, new Vector3(UIManager.ScreenWidth, 0.0f, 0.0f));
    }

    void ChangeSubpanelIndex(int index)
    {
        subpanelIndex += index;

        if (subpanelIndex < 0)
            subpanelIndex = 0;
        else if (subpanelIndex > optionSubpanel.Length - 1)
            subpanelIndex = optionSubpanel.Length - 1;

        optionSubpanel[subpanelIndex].gameObject.SetActive(true);
        optionSubpanel[subpanelIndex].Open();

        CheckButtonState();
    }

    #region button methods

    public void NextOptionPanel()
    {
        OptionSubpanelLeft(optionSubpanel[subpanelIndex]);
        optionSubpanel[subpanelIndex].Close();
        optionSubpanel[subpanelIndex].RegisterCallback((AnimDirection) => { optionSubpanel[subpanelIndex -1].gameObject.SetActive(false); });
        ChangeSubpanelIndex(1);
    }

    public void PreviousOptionPanel()
    {
        OptionSubpanelRight(optionSubpanel[subpanelIndex]);
        optionSubpanel[subpanelIndex].Close();
        optionSubpanel[subpanelIndex].RegisterCallback((AnimDirection) => { optionSubpanel[subpanelIndex + 1].gameObject.SetActive(false); });
        ChangeSubpanelIndex(-1);
    }

    #endregion

}
