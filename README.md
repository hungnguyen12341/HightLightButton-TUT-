# 🕳️ UI Highlight System (HoleRaycastFilter + HighLightImage)

Hệ thống UI Highlight giúp tạo **hiệu ứng làm tối màn hình và chỉ chừa lại một vùng được click** — thường dùng cho **tutorial, onboarding hoặc hướng dẫn người chơi**.  
Bao gồm hai phần chính:

---

## ⚙️ Thành phần chính

### 1. `HoleRaycastFilter.cs`
- Gắn trên **tấm overlay Image** (thường là panel đen mờ).
- Dùng để **lọc raycast**: chặn toàn bộ click, **chừa lại "lỗ" (hole)** để click xuyên qua đúng vùng cho phép.
- Tự implement `ICanvasRaycastFilter`.

**Các chức năng chính:**
- Tự động cho phép raycast xuyên qua khi người dùng click trong vùng `ClickZone`.
- Có thể set `ClickZone` động bằng:
  ```csharp
  holeRaycastFilter.SetClickZoneFromTarget(target);
