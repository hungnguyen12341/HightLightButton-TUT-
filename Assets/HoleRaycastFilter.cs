// HoleRaycastFilter.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoleRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField] private bool debugLogs = false;

    private RaycastHole _raycastHole = new RaycastHole();

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        bool valid = _raycastHole.IsRaycastLocationValid(screenPoint, eventCamera);

        if (debugLogs)
        {
            // debug: in ra trạng thái và vị trí, helpful để kiểm tra xem hàm đang trả true/false như nào
            Debug.Log($"HoleRaycastFilter.IsRaycastLocationValid -> {valid} (screenPoint={screenPoint}, cam={(eventCamera == null ? "null" : eventCamera.name)})");
        }

        return valid;
    }

    // Public API để HighLightImage hay class khác set zone
    public void SetClickZone(RectTransform clickZone) => _raycastHole.SetClickZone(clickZone);
    public void SetClickZoneFromTarget(GameObject target) => _raycastHole.SetClickZoneFromTarget(target);
    public void ClearClickZone() => _raycastHole.ClearClickZone();
}
