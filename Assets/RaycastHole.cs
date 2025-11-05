// RaycastHole.cs
using UnityEngine;

public class RaycastHole
{
    private RectTransform _clickZone;

    public void SetClickZone(RectTransform clickZone) => _clickZone = clickZone;
    public void SetClickZoneFromTarget(GameObject target)
    {
        _clickZone = target == null ? null : target.GetComponent<RectTransform>();
    }
    public void ClearClickZone() => _clickZone = null;

    /// <summary>
    /// Trả về true nếu graphic nên nhận raycast tại điểm này.
    /// IMPORTANT: Để "cho click xuyên qua lỗ", khi điểm nằm trong ClickZone => phải trả FALSE.
    /// </summary>
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (_clickZone == null) return true; // không có hole => graphic nhận raycast bình thường (chặn)

        // Sử dụng RectangleContainsScreenPoint để xử lý tốt hơn cho các chế độ Canvas khác nhau
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(_clickZone, screenPoint, eventCamera);

        // Nếu inside == true => điểm nằm trong lỗ => KHÔNG nhận raycast tại vị trí này => trả false
        return !inside;
    }
}
