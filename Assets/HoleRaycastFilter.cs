using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoleRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    public RectTransform ClickZone; // vùng được phép click (ví dụ nút Play)

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (ClickZone == null) return true;

        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            ClickZone, screenPoint, eventCamera, out localPoint
        );


        if (ClickZone.rect.Contains(localPoint))
            return false;
        else
            return true; 
    }
}
