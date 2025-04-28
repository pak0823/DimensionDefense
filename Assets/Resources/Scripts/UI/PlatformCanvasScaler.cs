using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class PlatformCanvasScaler : MonoBehaviour
{
    CanvasScaler scaler;

    void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

#if UNITY_IOS || UNITY_ANDROID
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 1.0f;
#else
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
#endif
    }
}
