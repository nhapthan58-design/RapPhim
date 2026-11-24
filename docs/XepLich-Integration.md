# Hướng dẫn tích hợp Module Xếp Lịch Chiếu (Hùng)

Tài liệu ngắn gọn dành cho các thành viên trong nhóm để gắn các module còn lại vào module **Xếp lịch chiếu phim** mà không bị xung đột code.

---

## 1. Nguyên — Trang chủ (Menu chính)

`Form1` hiện tại chỉ là launcher tạm để chạy riêng module Xếp lịch. Khi Nguyên dựng Trang chủ chính thức, chỉ cần mở 2 form của Hùng bằng:

```csharp
// Mở màn hình Lên lịch chiếu mới
using (var f = new FormXepLich())
{
    f.ShowDialog(this);
}

// Mở màn hình Danh sách lịch chiếu (Xem / Lọc / Sửa / Xóa / Copy Day / Timeline)
using (var f = new FormDanhSachLich())
{
    f.ShowDialog(this);
}
```

---

## 2. Kiệt — Phân quyền (Đăng nhập)

Hai màn hình `FormXepLich` và `FormDanhSachLich` chỉ nên hiển thị / bật nút cho vai trò **Quản lý (Manager / Admin)**. Vai trò **Nhân viên bán vé (Staff)** chỉ mở màn hình Bán vé của Sỹ Anh.

---

## 3. Sỹ Anh — Màn hình Bán vé

Sử dụng các hàm đọc sẵn trong `RapPhim.Services.LichChieuService` (luôn trả về **bản sao an toàn**, đã lọc suất chưa chiếu và sắp xếp tăng dần theo giờ bắt đầu):

```csharp
using RapPhim.Services;

// 1. Lấy danh sách phim còn suất sắp chiếu trong ngày để đổ vào ComboBox chọn phim
var phimHomNay = LichChieuService.LayPhimCoSuatTrongNgay(DateTime.Today);

// 2. Lấy các suất sắp chiếu trong ngày của 1 bộ phim (hoặc truyền null để lấy tất cả phim)
var suatChieu = LichChieuService.LaySuatSapChieu(DateTime.Today, maPhim: 1);

// 3. Lấy chi tiết 1 suất chiếu theo mã
var chiTiet = LichChieuService.LayTheoMa(maLichChieu);
```

> **Lưu ý:** Không sửa trực tiếp các object trả về (đây là bản sao để bảo vệ dữ liệu).

---

## 4. Khôi — Thanh toán & Trạng thái ghế 'Đã bán'

Module Xếp lịch tự động **khóa Sửa/Xóa** mọi suất chiếu đã có khách mua vé (hoặc đã bắt đầu chiếu).

- **Khi chưa nối DB vé:** Gọi `LichChieuService.GhiNhanBanVe(maLichChieu, soVe)` hoặc `LichChieuService.HuyVe(maLichChieu, soVe)` (không tự sửa `LichChieu.SoVeDaBan` trực tiếp).
- **Khi đã nối DB có bảng Ghế/Vé 'Đã bán':**
  1. Tạo class implement `IVeDaBanProvider` (`int DemVeDaBan(int maLichChieu)` — chạy `SELECT COUNT(*) FROM ... WHERE MaLichChieu = @id AND TrangThai = N'Đã bán'`).
  2. Đổi **1 dòng duy nhất** trong `RapPhim/Data/CauHinhLuuTru.cs`:
     ```csharp
     public static IVeDaBanProvider TaoVeDaBanProvider() => new SqlVeDaBanProvider(connStr);
     ```

---

## 5. Mạnh — Cơ sở dữ liệu & Quản lý Phim / Phòng chiếu

Module Xếp lịch **chỉ đọc** danh sách Phim/Phòng qua `IDanhMucProvider` và lưu Lịch chiếu qua `ILichChieuRepository` (CRUD từng bản ghi: `LoadAll`, `Insert`, `Update`, `Delete`, `HasData`).

Các cột cần có ở bảng `LichChieu` trong DB:

| Cột | Kiểu gợi ý | Ghi chú |
|---|---|---|
| `MaLichChieu` | `INT IDENTITY PRIMARY KEY` | DB tự tăng, trả về qua `Insert` |
| `MaPhim` | `INT FOREIGN KEY` | Trỏ sang bảng Phim |
| `MaPhong` | `INT FOREIGN KEY` | Trỏ sang bảng Phòng chiếu |
| `GioBatDau` | `DATETIME2` | Luôn cắt giây (`:00`), trong khung `08:00–23:30` |
| `GioKetThuc` | `DATETIME2` | `GioBatDau + ThoiLuongPhim` (có thể qua nửa đêm) |

**Cách cắm DB SQL Server (không phải sửa bất kỳ Form hay Service nào):**
1. Implement `ILichChieuRepository` (dùng transaction `SERIALIZABLE` khi `Insert`/`Update` để chống trùng giờ khi nhiều máy chạy cùng lúc).
2. Implement `IDanhMucProvider` đọc danh sách Phim & Phòng từ DB.
3. Đổi 2 dòng trong `RapPhim/Data/CauHinhLuuTru.cs`.

---

## 6. Kiệt — Mẹo nhập nhanh 50 lịch chiếu mẫu

1. Mở **Schedule Showtime** (`FormXepLich`):
   - Giữ tick **Auto-jump to next free slot after adding**.
   - Bấm **Enter** liên tiếp để xếp 1 ngày cho từng phòng — không cần bấm OK hộp thoại, giờ tự động nhảy sang khung trống kế tiếp (+15 phút dọn phòng).
2. Mở **View Schedule List** (`FormDanhSachLich`) → bấm **📑 Copy Day**:
   - Sao chép toàn bộ lịch của 1 ngày đã xếp sang các ngày tiếp theo chỉ bằng vài cú click. Suất mới tự động có `0` vé bán và tự bỏ qua các khung bị trùng (có báo cáo chi tiết).
