using UnityEngine;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    private void Awake()
    {
        Initialize();

        int targetWIdth = (int)(1920 * 0.5f);
        int targetHeight = (int)(1080 * 0.5f);

        Screen.SetResolution(targetWIdth, targetHeight, false);
    }

    protected virtual void Initialize()
    {
        SetCanvas();
    }

    private void SetCanvas()
    {
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null )
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }
        CanvasScaler canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
        if ( canvasScaler != null )
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }
    }
}
