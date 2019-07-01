using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;
using System.IO;
using System.Collections;

public class ApplicationManager : MonoBehaviour
{
    static ApplicationManager singleton;

    string tempPhotoReferral;

    void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
            return;
        }

        singleton = this;
    }

    public static void SavePhoto()
    {
        if (singleton == null)
        {
            Debug.LogError("ApplicationManager.singleton was null when SavePhoto() was called. Aborting.");
            return;
        }

        singleton._SavePhoto();
    }

    void _SavePhoto()
    {
        NativeGallery.SaveImageToGallery(tempPhotoReferral, "PolaroidSnaps", GetName());
    }

    public static void SharePhoto()
    {
        if (singleton == null)
        {
            Debug.LogError("ApplicationManager.singleton was null when SharePhoto() was called. Aborting.");
            return;
        }

        singleton._SharePhoto();
    }

    void _SharePhoto()
    {
        new NativeShare().AddFile(tempPhotoReferral).Share();
    }

    public static void Vibrate()
    {
        if (singleton == null)
        {
            return;
        }

        singleton._Vibrate();
    }

    void _Vibrate()
    {
        Vibration.CreateOneShot(5, 30);
    }

    public static void CachePhoto(Texture2D photo, VoidDelegate onPhotoCachedCallback)
    {
        if (singleton == null)
        {
            Debug.LogError("ApplicationManager.singleton was null when CachePhoto(" + photo.name + ", " + onPhotoCachedCallback.ToString() + ") was called. Aborting.");
            return;
        }

        singleton._CachePhoto(photo, onPhotoCachedCallback);
    }

    int photoWidth;
    int photoHeight;
    
    static ArrayFlipJob flipJob;
    static JobHandle flipJobHandle;

    void _CachePhoto(Texture2D photo, VoidDelegate photoCachedCallback)
    {
        tempPhotoReferral = Application.temporaryCachePath + "/" + GetName(); // Path.Combine(Application.temporaryCachePath, GetName());
        onPhotoCachedCallback = photoCachedCallback;

        photoWidth = photo.width;
        photoHeight = photo.height;

        byte[] rawTexData = photo.GetRawTextureData();

        Debug.Log("RawTexData = " + rawTexData.Length.ToString() + "bytes");

        flipJob = new ArrayFlipJob
        {
            imageWidth = photoWidth,
            imageHeight = photoHeight,
            source = new NativeArray<byte>(rawTexData.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
            destination = new NativeArray<byte>(rawTexData.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
        };

        flipJob.source.CopyFrom(rawTexData);
        flipJobHandle = flipJob.Schedule();

        StartCoroutine(FlipJobCoroutine());
    }

    IEnumerator FlipJobCoroutine()
    {
        while (!flipJobHandle.IsCompleted)
        {
            UIManager.PrintDebugText("flipping array...");
            yield return null;
        }

        flipJobHandle.Complete();

        UIManager.PrintDebugText("Calling DLL: WriteJPEGfile");

        try
        {
            JPEGcodec.WriteJPEGfile(flipJob.destination.ToArray(), photoWidth, photoHeight, tempPhotoReferral, 90);
        }
        catch (Exception e)
        {
            UIManager.PrintDebugText("!!!!!! Exception: " + e.Message);
        }

        flipJob.destination.Dispose();

        onPhotoCachedCallback?.Invoke();
        onPhotoCachedCallback = null;
    }

    string GetName()
    {
        return "snap_" + GetDate() + "_{0}.jpeg";
    }

    string GetDate()
    {
        return DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "__" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString();
    }

    VoidDelegate onPhotoCachedCallback;

    private void OnDestroy()
    {
        if (!flipJobHandle.IsCompleted)
            flipJobHandle.Complete();

        if(flipJob.destination.IsCreated)
            flipJob.destination.Dispose();
    }
}
