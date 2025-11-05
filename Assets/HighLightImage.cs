using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HoleRaycastFilter
/// - Lọc raycast cho Canvas (vùng ClickZone là vùng cho phép click).
/// - Khi SetTarget(...) sẽ spawn decoy vào cuối sibling của target, copy sprite và set native size.
/// - Giả định DecoyPrefabs luôn có Image (không kiểm tra decoy Image).
/// </summary>
[RequireComponent(typeof(Image))]
public class HighLightImage : MonoBehaviour
{
    #region Inspector Fields

    [Header("Raycast Hole")]
    [Tooltip("Vùng được phép click (ví dụ: nút Play). Nếu con trỏ nằm trong ClickZone => cho phép raycast.")]
    public RectTransform ClickZone;

    [Header("Decoy / Highlight")]
    [Tooltip("Prefab dùng làm decoy (phải có Image component trên root hoặc child đầu).")]
    [SerializeField] private GameObject DecoyPrefabs;

    [Tooltip("Material stencil (nếu muốn áp dụng lên decoy Image). Có thể để null nếu không dùng.")]
    [SerializeField] private Material _StencilMaterial;

    #endregion

    #region Runtime State

    // Tham chiếu tới target hiện tại (đối tượng mà decoy copy hình ảnh)
    private GameObject _target;

    // Tham chiếu tới instance decoy đang spawn (nếu có)
    private GameObject _decoyInstance;

    // Filter cho tấm dim
    [SerializeField] private HoleRaycastFilter _filter; 
    #endregion



    #region Public API - Target / Decoy

    /// <summary>
    /// Gán target mới và spawn decoy dựa trên target đó.
    /// Nếu target null => xóa decoy hiện tại.
    /// </summary>
    /// <param name="target">GameObject chứa Image mà decoy sẽ copy sprite từ đó.</param>
    public void SetTarget(GameObject target)
    {
        // Nếu target null => xóa decoy hiện tại và clear target
        if (target == null)
        {
            RemoveDecoy();
            _target = null;
            return;
        }

        // Nếu target không đổi thì không làm gì
        if (_target == target) return;

        // Lưu target
        _target = target;
        _filter.SetClickZone(target.GetComponent<RectTransform>()); 
        // Spawn decoy mới (xóa cái cũ nếu tồn tại)
        RemoveDecoy();
        SpawnDecoyFromTarget();
    }

    /// <summary>
    /// Xóa decoy nếu tồn tại.
    /// Sử dụng Destroy để hoàn toàn remove object (không dùng SetActive).
    /// </summary>
    public void RemoveDecoy()
    {
        if (_decoyInstance != null)
        {
            Destroy(_decoyInstance);
            _decoyInstance = null;
        }
    }

    #endregion

    #region Internal Helpers - Spawn / Setup Decoy

    /// <summary>
    /// Spawn decoy dựa trên _target hiện tại.
    /// Decoy sẽ được đặt cùng parent với target và đảm bảo là last sibling.
    /// Copy Image.sprite và gọi SetNativeSize().
    /// Đồng thời copy RectTransform (anchoredPosition/size/scale) để khớp target.
    /// Giả định DecoyPrefabs luôn có Image => không kiểm tra decoy Image.
    /// </summary>
    private void SpawnDecoyFromTarget()
    {
        if (_target == null) return;
        if (DecoyPrefabs == null) return;

        // Lấy parent để spawn decoy dưới cùng parent của _target
        Transform parent = _target.transform.parent;

        // Instantiate decoy với parent (sẽ mặc định nằm cuối)
        _decoyInstance = Instantiate(DecoyPrefabs, parent);

        // Đặt tên phụ trợ
        _decoyInstance.name = DecoyPrefabs.name + "_Decoy";
        // Parent
        _decoyInstance.transform.SetParent(_target.transform);  
        // Đặt decoy nằm cuối sibling explicitly
        _decoyInstance.transform.SetAsLastSibling();

        // Copy RectTransform nếu có (để decoy trùng vị trí/size với target)
        RectTransform targetRect = _target.GetComponent<RectTransform>();
        RectTransform decoyRect = _decoyInstance.GetComponent<RectTransform>();
        if (decoyRect != null && targetRect != null)
        {
            decoyRect.anchorMin = targetRect.anchorMin;
            decoyRect.anchorMax = targetRect.anchorMax;
            decoyRect.pivot = targetRect.pivot;
            decoyRect.anchoredPosition = targetRect.anchoredPosition;
            decoyRect.sizeDelta = targetRect.sizeDelta;
            decoyRect.localScale = targetRect.localScale;
            decoyRect.localRotation = targetRect.localRotation;
        }

        // Lấy Image từ target (nếu target không có Image thì log warning)
        Image targetImage = _target.GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogWarning($"HoleRaycastFilter: Target '{_target.name}' không có Image component để copy sprite.");
            return;
        }

        // Giả định decoy luôn có Image (không kiểm tra)
        Image decoyImage = _decoyInstance.GetComponent<Image>();
        if (decoyImage == null)
        {
            // Nếu root decoy không có Image thì lấy child đầu (vẫn không kiểm tra tồn tại theo yêu cầu)
            decoyImage = _decoyInstance.GetComponentInChildren<Image>();
        }

        // Copy sprite và set native size
        decoyImage.sprite = targetImage.sprite;
        decoyImage.SetNativeSize();

        // Nếu có material stencil được cung cấp, gán cho decoy image
        if (_StencilMaterial != null)
        {
            decoyImage.material = _StencilMaterial;
        }
    }

    #endregion

    #region Editor / Utility (Tuỳ chọn)

#if UNITY_EDITOR
    private void OnValidate()
    {
       
    }
#endif

    #endregion
}
