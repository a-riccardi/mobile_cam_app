using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas mainCanvas;
    [SerializeField] Text debugText;
    [SerializeField] RawImage mainImage;

    [SerializeField] bool doDebug;
    
    [SerializeField] UIPanel mainPanel;
    [SerializeField] UIPanel viewPanel;

    [SerializeField] UIPanel photoPanel;
    [SerializeField] UIPanel optionPanel;
    [SerializeField] UIPanel rgbPanel;
    [SerializeField] UIPanel luminancePanel;
    [SerializeField] UIPanel fisheyePanel;
    [SerializeField] UIPanel colorizePanel;
    [SerializeField] UIPanel resolutionPanel;

    static UIManager singleton;

    public static float ScreenWidth { get { return singleton.mainCanvas.pixelRect.width; } }

	void Awake ()
    {
        if (singleton != null)
        {
            Destroy(this);
            return;
        }

#if DEBUG
        debugText.gameObject.SetActive(doDebug);
        debugText.enabled = true;
        debugText.text = "";
#else
        Destroy(debugText.gameObject);
#endif

        singleton = this;
	}

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        mainPanel.gameObject.SetActive(true);
        viewPanel.gameObject.SetActive(true);

        photoPanel.gameObject.SetActive(true);
        optionPanel.gameObject.SetActive(true);

        mainPanel.SetupPositions(Vector3.zero, new Vector3(-ScreenWidth, 0.0f, 0.0f));
        viewPanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));

        photoPanel.SetupPositions(Vector3.zero, new Vector3(-ScreenWidth, 0.0f, 0.0f));
        optionPanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));

        mainPanel.Open(true);
        viewPanel.Close(true);

        photoPanel.Open(true);
        optionPanel.Close(true);

        InitializeOptionPanels();

        float side = Mathf.Min(mainImage.rectTransform.sizeDelta.x, mainImage.rectTransform.sizeDelta.y);
        mainImage.rectTransform.sizeDelta = new Vector2(side, side);
    }

    void InitializeOptionPanels()
    {
        rgbPanel.gameObject.SetActive(true);
        luminancePanel.gameObject.SetActive(true);
        fisheyePanel.gameObject.SetActive(true);
        colorizePanel.gameObject.SetActive(true);
        resolutionPanel.gameObject.SetActive(true);

        rgbPanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));
        luminancePanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));
        fisheyePanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));
        colorizePanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));
        resolutionPanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));

        rgbPanel.Close(true);
        luminancePanel.Close(true);
        fisheyePanel.Close(true);
        colorizePanel.Close(true);
        resolutionPanel.Close(true);
    }

    public static void PrintDebugText(string text)
    {
#if DEBUG
        if (singleton == null)
        {
            Debug.LogError("UIManager.singleton was null when PrintDebugText(" + text + ") was called. Aborting");
            return;
        }

        singleton._PrintDebugText(text);
#endif
    }

    void _PrintDebugText(string text)
    {
        if (debugText.text.Length > 3000)
            debugText.text = "";

        Debug.Log(text);
        debugText.text += "\r\n" + text;
    }

    public static void SetMainImageBinding(Texture mainTex)
    {
        singleton._SetMainImageBinding(mainTex);
    }

    void _SetMainImageBinding(Texture mainTex)
    {
        mainImage.texture = mainTex;
    }

    public static void OpenViewPanel(Texture2D snappedPic)
    {
        if (singleton == null)
        {
            Debug.LogError("UIManager.singleton was null when OpenViewPanel(" + snappedPic.name + ") was called. Aborting");
            return;
        }

        singleton._OpenViewPanel(snappedPic);
    }

    void _OpenViewPanel(Texture2D snappedPic)
    {
        (viewPanel as UIViewPanel).SetupPhotoImage(snappedPic);
        mainPanel.Close();
        photoPanel.Close();
        viewPanel.Open();
    }

    public static void CloseViewPanel()
    {
        if (singleton == null)
        {
            Debug.LogError("UIManager.singleton was null when CloseViewPanel() was called. Aborting");
            return;
        }

        singleton._CloseViewPanel();
    }

    void _CloseViewPanel()
    {
        viewPanel.Close();
        mainPanel.Open();
        photoPanel.Open();
    }

    public static void OpenSettingsMenuPanel(OptionPanel requestedPanel)
    {
        if (singleton == null)
        {
            Debug.LogError("UIManager.singleton was null when OpenSettingsMenuPanel(" + requestedPanel.ToString() + ") was called. Aborting");
            return;
        }

        singleton._OpenSettingsMenuPanel(requestedPanel);
    }

    void _OpenSettingsMenuPanel(OptionPanel requestedPanel)
    {
        switch (requestedPanel)
        {
            case OptionPanel.RGB_DISTORTION:
                OpenRGBMenu();
                break;
            case OptionPanel.LUMINANCE_CORRECTION:
                OpenLuminanceMenu();
                break;
            case OptionPanel.FISHEYE:
                OpenFisheyePanel();
                break;
            case OptionPanel.COLOR_CORRECTION:
                OpenColorizePanel();
                break;
            case OptionPanel.RESOLUTION:
                OpenResolutionPanel();
                break;
        }
    }

    void OnOptionPanelAnimationIsOver(AnimDirection direction)
    {
        if (direction == AnimDirection.FORWARD)
            OptionPanelRight();
        else
            OptionPanelLeft();
    }

    void OptionPanelLeft()
    {
        optionPanel.SetupPositions(Vector3.zero, new Vector3(-ScreenWidth, 0.0f, 0.0f));
    }

    void OptionPanelRight()
    {
        optionPanel.SetupPositions(Vector3.zero, new Vector3(ScreenWidth, 0.0f, 0.0f));
    }

    //BUTTON METHODS-----------------------------------------------
    void OpenSettingsMenu()
    {
        photoPanel.Close();
        optionPanel.Open();
    }

    public void CloseSettingsMenu()
    {
        optionPanel.RegisterCallback((AnimDirection) => { (optionPanel as UIOptionPanel).Reset(); });
        optionPanel.Close();
        photoPanel.Open();
    }

    void OpenRGBMenu()
    {
        OptionPanelLeft();

        optionPanel.Close();
        rgbPanel.Open();
    }

    public void CloseRGBMenu()
    {
        optionPanel.RegisterCallback(OnOptionPanelAnimationIsOver);
        
        rgbPanel.Close();
        optionPanel.Open();
    }

    void OpenLuminanceMenu()
    {
        OptionPanelLeft();

        optionPanel.Close();
        luminancePanel.Open();
    }

    public void CloseLuminanceMenu()
    {
        optionPanel.RegisterCallback(OnOptionPanelAnimationIsOver);

        luminancePanel.Close();
        optionPanel.Open();
    }

    void OpenFisheyePanel()
    {
        OptionPanelLeft();

        optionPanel.Close();
        fisheyePanel.Open();
    }

    public void CloseFisheyeMenu()
    {
        optionPanel.RegisterCallback(OnOptionPanelAnimationIsOver);

        fisheyePanel.Close();
        optionPanel.Open();
    }

    void OpenColorizePanel()
    {
        OptionPanelLeft();

        optionPanel.Close();
        colorizePanel.Open();
    }

    public void CloseColorizeMenu()
    {
        optionPanel.RegisterCallback(OnOptionPanelAnimationIsOver);

        colorizePanel.Close();
        optionPanel.Open();
    }

    void OpenResolutionPanel()
    {
        OptionPanelLeft();

        optionPanel.Close();
        resolutionPanel.Open();
    }

    public void CloseResolutionPanel()
    {
        optionPanel.RegisterCallback(OnOptionPanelAnimationIsOver);

        resolutionPanel.Close();
        optionPanel.Open();
    }

    public void DefaultPressFeedback()
    {
        ApplicationManager.Vibrate();
    }
    //-------------------------------------------------------------
}
