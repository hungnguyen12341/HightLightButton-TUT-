Hole Highlight UI System (Unity)
================================

GIỚI THIỆU
-----------
Hệ thống Hole Highlight UI được sử dụng để làm mờ (dim) toàn bộ giao diện và chỉ highlight một vùng cụ thể — ví dụ như trong tutorial, khi cần hướng dẫn người chơi nhấn vào một nút hoặc vùng UI cụ thể.

Cấu trúc hệ thống bao gồm:
- HoleRaycastFilter: Lọc vùng raycast hợp lệ.
- HighlightImageController: Quản lý hiển thị hole và decoy.
- Stencil Shader: Tạo hiệu ứng đục lỗ (hole) trong overlay UI.

------------------------------------------------------------
THÀNH PHẦN CHÍNH
----------------

1. HoleRaycastFilter.cs
   - Gắn lên Image dim (panel overlay).
   - Xác định vùng được phép click (ClickZone).
   - Trả về true/false cho raycast tùy vị trí click.

   Nếu click nằm trong vùng ClickZone → cho phép raycast.
   Nếu click nằm ngoài → bị chặn bởi overlay.

2. HighlightImageController.cs
   - Quản lý Target (vùng cần highlight).
   - Spawn Decoy (hình ảnh giả lập vùng sáng) tại vị trí Target.
   - Đảm bảo Decoy nằm cuối sibling (hiển thị đúng thứ tự).
   - Các hàm chính:
       • SetTarget(GameObject target)
       • RemoveDecoy()

   Khi SetTarget được gọi:
   1. Spawn một DecoyPrefab ở cuối hierarchy.
   2. Lấy sprite từ target để set cho decoy.
   3. Gọi SetNativeSize() để khớp kích thước.
   4. Gán ClickZone cho HoleRaycastFilter để chỉ cho phép click đúng vùng đó.

3. Stencil Shader (Hole Mask)
   - Dùng để tạo "lỗ" (hole) trên lớp overlay.
   - Shader sử dụng Stencil Buffer để xác định vùng hiển thị.

   Cơ chế hoạt động:
   1. Overlay Image dùng Material có Stencil Shader.
   2. Shader kiểm tra vùng được Decoy vẽ và "đục" lỗ tại đó.
   3. Vùng lỗ trong suốt, vùng còn lại mờ, tạo hiệu ứng focus.

   Ví dụ stencil block trong shader:

   Stencil
   {
       Ref 1
       Comp notequal
       Pass keep
   }

   Nghĩa là: chỉ render những pixel không trùng vùng Decoy → tạo vùng trong suốt.

------------------------------------------------------------
QUY TRÌNH HOẠT ĐỘNG
-------------------

1. Khi muốn highlight một đối tượng:
   highlightController.SetTarget(playButton.gameObject);

2. HighlightImageController sẽ:
   - Spawn Decoy → set sprite của target.
   - Gán ClickZone vào HoleRaycastFilter.

3. Shader tạo vùng trong suốt (hole).
4. HoleRaycastFilter chỉ cho phép click đúng vùng hole.

------------------------------------------------------------
KHI KẾT THÚC TUTORIAL
---------------------
   highlightController.RemoveDecoy();

→ Xóa decoy và reset overlay về trạng thái bình thường.

------------------------------------------------------------
YÊU CẦU
--------
- Canvas ở chế độ Screen Space - Overlay.
- Overlay Image có Material dùng Stencil Shader.
- Target UI có Image component.
- Decoy Prefab có Image và Material stencil phù hợp.

------------------------------------------------------------
VÍ DỤ SỬ DỤNG
--------------
[SerializeField] private HighlightImageController highlight;

void StartTutorialStep()
{
    highlight.SetTarget(playButton.gameObject);
}

void EndTutorialStep()
{
    highlight.RemoveDecoy();
}

------------------------------------------------------------
ƯU ĐIỂM
--------
- Không cần tách UI phức tạp.
- Hoạt động với mọi Canvas.
- Cho phép tương tác chính xác trong vùng hole.
- Dễ mở rộng cho tutorial, onboarding, focus UI, v.v.

------------------------------------------------------------

