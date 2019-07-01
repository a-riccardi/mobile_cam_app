using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum OptionPanel { RGB_DISTORTION, FISHEYE, COLOR_CORRECTION, LUMINANCE_CORRECTION, RESOLUTION }

public enum AnimDirection { BACKWARD = -1, FORWARD = 1 }

public enum ColorCorrectionPreset { SUNSET, WOODS }

public enum ResolutionQuality { LOW, MEDIUM, HIGH }

public class PanelCallback : UnityEvent<AnimDirection> { }

public delegate void VoidDelegate();

public class Globals : MonoBehaviour
{
    public static Globals singleton;

    [Header("Frame Preset")]
    [SerializeField] List<PhotoFrame> photoFrames;
    public static List<PhotoFrame> PhotoFrames { get { return singleton.photoFrames; } }

    [Header("Materials")]
    [SerializeField] Material postProductionMaterial;
    public static Material PostProductionMaterial { get { return singleton.postProductionMaterial; } }
    [SerializeField] Material photoAssemblerMaterial;
    public static Material PhotoAssemblerMaterial { get { return singleton.photoAssemblerMaterial; } }

    [Header("Color Correction Preset")]
    [SerializeField] List<ColorCorrectionData> colorCorrectionPreset;
    public static List<ColorCorrectionData> ColorCorrectionPreset { get { return singleton.colorCorrectionPreset; } }

    [Header("Resolution List")]
    [SerializeField] List<Resolution> resolutions;
    public static List<Resolution> Resolutions { get { return singleton.resolutions; } }

#if UNITY_EDITOR
    [Header("Editor Debug Data")]
    [SerializeField] Texture2D testTexture;
    public static Texture2D TestTexture { get { return singleton.testTexture; } }
#endif

    //[Header("Button Text")]
    //[SerializeField] string rgbButton
    void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
            return;
        }

        singleton = this;
    }
}
