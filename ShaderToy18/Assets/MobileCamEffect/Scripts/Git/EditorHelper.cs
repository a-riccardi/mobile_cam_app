#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHelper : MonoBehaviour
{
    void OnDestroy()
    {
        Debug.LogWarning("Performing custom material cleanup before shutDown.");

        Globals.PostProductionMaterial.SetTexture("_MainTex", null);
        Globals.PhotoAssemblerMaterial.SetTexture("_MainTex", null);
    }
}

#endif
