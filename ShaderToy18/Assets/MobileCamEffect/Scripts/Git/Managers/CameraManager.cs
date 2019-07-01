using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class CameraManager : MonoBehaviour
{
    public static CameraManager singleton;

    RenderTexture croppedCamTexture;
    WebCamTexture camTexture;
    bool doCropCamTexture;

    WebCamDevice[] cameraDevices;
    int deviceIndex = -1;

    ColorCorrectionData currentCCPreset;
    PhotoFrame currentPhotoFrame;

    int squareSide;

    int redOffsetID;
    int blueOffsetID;
    int fisheyeFactorID;
    int mainTextureID;
    int frameTextureID;
    int screenTexelSizeID;
    int flipUVID;
    int frameUVID;
    int lightRampID;
    int shadowRampID;
    int lightIntensityID;
    int shadowIntensityID;
    int luminanceStrengthID;
    int colorCorrectionStrengthID;

    void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
            return;
        }

        singleton = this;

        CollectShaderID();
    }

    void Start()
    {
        StartCoroutine(CheckPermission());

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
            UIManager.PrintDebugText(WebCamTexture.devices[i].name + " -- " + WebCamTexture.devices[i].isFrontFacing.ToString());

        Vector2 currentRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        UIManager.PrintDebugText("currentRes is: " + currentRes.ToString());

        Globals.PostProductionMaterial.SetVector(flipUVID, new Vector4(1, 0, 0, 0));
        Globals.PostProductionMaterial.SetVector(screenTexelSizeID, new Vector4(1.0f / currentRes.x, 1.0f / currentRes.y, currentRes.x, currentRes.y));

        //bulk initialization, rethink
        currentCCPreset = Globals.ColorCorrectionPreset[0];
        currentPhotoFrame = Globals.PhotoFrames[0];
        currentResolution = Globals.Resolutions[1];

        InitializeFisheye();
        InitializeRGBDistortion();
        SetupColorCorrection();
        InitializeLuminanceFilter();

        squareSide = (int)Mathf.Min(currentRes.x, currentRes.y);

        InitializeCameraDevice();
    }

    void CollectShaderID()
    {
        redOffsetID = Shader.PropertyToID("_RedOffset");
        blueOffsetID = Shader.PropertyToID("_BlueOffset");
        fisheyeFactorID = Shader.PropertyToID("_FisheyeF");
        mainTextureID = Shader.PropertyToID("_MainTex");
        frameTextureID = Shader.PropertyToID("_FrameTex");
        screenTexelSizeID = Shader.PropertyToID("_ScreenTextureTexelSize");
        flipUVID = Shader.PropertyToID("_FlipUV");
        frameUVID = Shader.PropertyToID("_FrameTex_UV");
        lightIntensityID = Shader.PropertyToID("_LightStrength");
        shadowIntensityID = Shader.PropertyToID("_ShadowStrength");
        lightRampID = Shader.PropertyToID("_LightMask");
        shadowRampID = Shader.PropertyToID("_ShadowMask");
        luminanceStrengthID = Shader.PropertyToID("_LuminanceCorrectionF");
        colorCorrectionStrengthID = Shader.PropertyToID("_ColorCorrectionF");
    }

    void InitializeCameraDevice()
    {
        cameraDevices = WebCamTexture.devices;

        ChangeCameraDevice(0);

#if UNITY_EDITOR
        Globals.PostProductionMaterial.EnableKeyword("UNITY_EDITOR");
        Globals.PostProductionMaterial.SetTexture(mainTextureID, Globals.TestTexture);
        UIManager.SetMainImageBinding(Globals.TestTexture);
#else
        Globals.PostProductionMaterial.DisableKeyword("UNITY_EDITOR");
        Globals.PostProductionMaterial.SetTexture(mainTextureID, camTexture);
        UIManager.SetMainImageBinding(camTexture);
#endif

        UIManager.PrintDebugText(camTexture.width.ToString() + " - " + camTexture.height.ToString());
    }

    void InitializeFisheye()
    {
        Globals.PostProductionMaterial.SetFloat(fisheyeFactorID, 0.0f);
        Globals.PostProductionMaterial.EnableKeyword("FISHEYE");
    }

    void InitializeRGBDistortion()
    {
        Globals.PostProductionMaterial.SetFloat(redOffsetID, 7.0f);
        Globals.PostProductionMaterial.SetFloat(blueOffsetID, -7.0f);
        Globals.PostProductionMaterial.EnableKeyword("RGB_DISTORTION");
    }

    void InitializeLuminanceFilter()
    {
        Globals.PostProductionMaterial.EnableKeyword("LUMINANCE_FILTER");
    }

    void SetupColorCorrection()
    {
        Globals.PostProductionMaterial.SetTexture(lightRampID, currentCCPreset.LightRamp);
        Globals.PostProductionMaterial.SetTexture(shadowRampID, currentCCPreset.ShadowRamp);
        Globals.PostProductionMaterial.SetFloat(lightIntensityID, currentCCPreset.LightIntensity);
        Globals.PostProductionMaterial.SetFloat(shadowIntensityID, currentCCPreset.ShadowIntensity);
        Globals.PostProductionMaterial.EnableKeyword("COLOR_CORRECTION");
    }

    IEnumerator CheckPermission()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone))
        {
            UIManager.PrintDebugText("Permission Granted");
        }
        else
        {
            UIManager.PrintDebugText("Permission Denied");
        }
    }

    void ChangeCameraDevice(int newDevice)
    {
        if (newDevice == deviceIndex)
            return;

        if (newDevice >= cameraDevices.Length)
            newDevice = 0;
        else if (newDevice < 0)
            newDevice = cameraDevices.Length - 1;

        deviceIndex = newDevice;

        if (camTexture != null && camTexture.isPlaying)
            camTexture.Stop();
        camTexture = new WebCamTexture(cameraDevices[deviceIndex].name, squareSide, squareSide, 45);
        camTexture.Play();

        UIManager.PrintDebugText(cameraDevices[deviceIndex].isFrontFacing.ToString());

        if (cameraDevices[deviceIndex].isFrontFacing)
            Globals.PostProductionMaterial.SetVector(flipUVID, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        else
            Globals.PostProductionMaterial.SetVector(flipUVID, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));

        squareSide = Mathf.Min(camTexture.width, camTexture.height);

        UIManager.PrintDebugText("Cam width: " + camTexture.width.ToString() + ", Cam height: " + camTexture.height.ToString());

        if (camTexture.width != camTexture.height)
        {
            UIManager.PrintDebugText("Preparing camera cropping texture, size = " + squareSide.ToString() + " x " + squareSide.ToString());
            croppedCamTexture = new RenderTexture(squareSide, squareSide, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            doCropCamTexture = true;
            StartCoroutine(CropCameraTexture());
            UIManager.SetMainImageBinding(croppedCamTexture);
        }
        else
        {
            doCropCamTexture = false;
            StopCoroutine(CropCameraTexture());
#if UNITY_EDITOR
            UIManager.SetMainImageBinding(Globals.TestTexture);
#else
            UIManager.SetMainImageBinding(camTexture);
#endif
        }
    }

    IEnumerator CropCameraTexture()
    {
        while (doCropCamTexture)
        {
#if UNITY_EDITOR
            Graphics.Blit(Globals.TestTexture, croppedCamTexture, photoCropperMaterial);
#else
            Graphics.Blit(camTexture, croppedCamTexture, photoCropperMaterial);
#endif
            yield return null;
        }
    }

    IEnumerator DebugCamCoroutine(int frameCounter)
    {
        if (frameCounter < 60)
        {
            frameCounter++;
            yield return null;
        }

        StartCoroutine(CheckPermission());

        camTexture = new WebCamTexture(cameraDevices[deviceIndex].name, squareSide, squareSide, 45);
        camTexture.Play();


        while (true)
        {
            UIManager.PrintDebugText("frame n°" + frameCounter.ToString() + ", camera desired res: " + camTexture.requestedWidth.ToString() + " x " + camTexture.requestedHeight.ToString() + "; camera actual res: " + camTexture.width.ToString() + " x " + camTexture.height.ToString());
            frameCounter++;
            yield return null;
        }
    }

    void OnDestroy()
    {
        camTexture.Stop();
    }

    #region button methods
    public void OnLuminanceFilterToggled(bool doApply)
    {
        if (doApply)
            Globals.PostProductionMaterial.EnableKeyword("LUMINANCE_FILTER");
        else
            Globals.PostProductionMaterial.DisableKeyword("LUMINANCE_FILTER");
    }

    public void OnFisheyeValueChanged(float fisheyeFactor)
    {
        Globals.PostProductionMaterial.SetFloat(fisheyeFactorID, fisheyeFactor);
    }

    public void OnRGBDistortionToggled(bool doApply)
    {
        if (doApply)
            Globals.PostProductionMaterial.EnableKeyword("RGB_DISTORTION");
        else
            Globals.PostProductionMaterial.DisableKeyword("RGB_DISTORTION");
    }

    public void OnRGBValueChanged(float rgbFactor)
    {
        Globals.PostProductionMaterial.SetFloat(redOffsetID, rgbFactor);
        Globals.PostProductionMaterial.SetFloat(blueOffsetID, -rgbFactor);
    }

    public void ResetRGBEffect()
    {
        Globals.PostProductionMaterial.SetFloat(redOffsetID, 5.0f);
        Globals.PostProductionMaterial.SetFloat(blueOffsetID, -5.0f);
    }

    public void OnLuminanceValueChanged(float luminanceFactor)
    {
        Globals.PostProductionMaterial.SetFloat(luminanceStrengthID, luminanceFactor);
    }

    public void ResetLuminanceEffect()
    {
        Globals.PostProductionMaterial.SetFloat(luminanceStrengthID, 0.8f);
    }

    public void OnColorCorrectionValueChanged(float colorCorrectionFactor)
    {
        Globals.PostProductionMaterial.SetFloat(colorCorrectionStrengthID, colorCorrectionFactor);
    }

    public void ResetColorCorrectionEffect()
    {
        Globals.PostProductionMaterial.SetFloat(colorCorrectionStrengthID, 0.8f);
    }

    public void OnFisheyeToggled(bool doApply)
    {
        if (doApply)
            Globals.PostProductionMaterial.EnableKeyword("FISHEYE");
        else
            Globals.PostProductionMaterial.DisableKeyword("FISHEYE");
    }

    public void ResetFisheyeEffect()
    {
        Globals.PostProductionMaterial.SetFloat(fisheyeFactorID, 0.0f);
    }

    public void OnColorizeToggled(bool doApply)
    {
        if (doApply)
            Globals.PostProductionMaterial.EnableKeyword("COLOR_CORRECTION");
        else
            Globals.PostProductionMaterial.DisableKeyword("COLOR_CORRECTION");
    }

    public void ChangeColorizePreset(ColorCorrectionPreset requestedPreset)
    {
        for (int i = 0; i < Globals.ColorCorrectionPreset.Count; i++)
        {
            if (Globals.ColorCorrectionPreset[i].PresetName == requestedPreset)
            {
                currentCCPreset = Globals.ColorCorrectionPreset[i];
                SetupColorCorrection();
                break;
            }
        }
    }

    public void SwitchCameraDevice()
    {
        ChangeCameraDevice(deviceIndex + 1);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void SetResolution(ResolutionQuality newResQuality)
    {
        Debug.Log("Requested " + newResQuality.ToString() + " quality!");
        currentResolution = Globals.Resolutions.Find(x => x.Quality == newResQuality);
    }

    #endregion

    Resolution currentResolution;

    [SerializeField] Material photoCropperMaterial;

    public void OnTakePhoto()
    {
        int finalWidth = currentResolution.Width;
        int finalHeight = currentResolution.Height;

        RenderTexture postproduced = new RenderTexture(squareSide, squareSide, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        RenderTexture newPhoto = new RenderTexture(finalWidth, finalHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

        Texture2D final = new Texture2D(finalWidth, finalHeight, TextureFormat.RGB24, false);
        
        if (doCropCamTexture)
        {
            Graphics.Blit(croppedCamTexture, postproduced, Globals.PostProductionMaterial);
        }
        else
        {
#if UNITY_EDITOR
            Graphics.Blit(Globals.TestTexture, postproduced, Globals.PostProductionMaterial);
#else
            Graphics.Blit(camTexture, postproduced, Globals.PostProductionMaterial);
#endif
        }

        Globals.PhotoAssemblerMaterial.SetTexture(frameTextureID, currentPhotoFrame.Frame);
        Globals.PhotoAssemblerMaterial.SetVector(frameUVID, currentPhotoFrame.UV);

        Graphics.Blit(postproduced, newPhoto, Globals.PhotoAssemblerMaterial);

        RenderTexture.active = newPhoto;

        final.ReadPixels(new Rect(0, 0, finalWidth, finalHeight), 0, 0);
        final.Apply();

        UIManager.OpenViewPanel(final);
    }
}
