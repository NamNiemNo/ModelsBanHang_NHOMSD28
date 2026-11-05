# Thiết lập phân hệ Voucher NIVO

## Quy tắc nghiệp vụ

- `Voucher.SoLuong` là tổng lượt sử dụng còn lại trên toàn hệ thống.
- Cấp/gửi voucher cho khách hàng không làm giảm `SoLuong`.
- Khi tạo hóa đơn có voucher, API trừ một lượt trong transaction. Cập nhật có điều kiện `SoLuong > 0` để ngăn sử dụng vượt hạn mức khi có nhiều đơn đồng thời.
- Voucher được cấp cho khách hàng được ghi nhận trong `KhachHangVoucher`; đơn hàng trực tuyến chỉ dùng voucher đã được cấp và chưa sử dụng.
- Khi đơn có voucher bị hủy hoặc hóa đơn tạm bị xóa, lượt sử dụng được hoàn lại và trạng thái cấp voucher được mở lại.
- Thời điểm bắt đầu/kết thúc dùng `DateTime`, giao diện nhập ngày và giờ đến phút.

## Cấu hình gửi email

Không lưu mật khẩu SMTP trong source code. Với Gmail, bật xác minh 2 bước và tạo App Password (không dùng mật khẩu đăng nhập Gmail thường). Tại thư mục dự án:

```bash
dotnet user-secrets set "Email:From" "your-email@gmail.com" --project API/API.csproj
dotnet user-secrets set "Email:Smtp:Username" "your-email@gmail.com" --project API/API.csproj
dotnet user-secrets set "Email:Smtp:Password" "YOUR_GMAIL_APP_PASSWORD" --project API/API.csproj
```

Các cấu hình mặc định đã có trong `API/appsettings.json`:

- Host: `smtp.gmail.com`
- Port: `587`
- EnableSsl: `true`

Thay đổi Host/Port nếu sử dụng nhà cung cấp email khác. Sau khi cấu hình, khởi động lại API.

## Quy trình trên Admin

1. Vào Quản lý Voucher và tạo voucher (không chọn khách hàng tại bước tạo).
2. Sau khi lưu, hệ thống chuyển sang trang Gửi voucher qua email.
3. Chọn gửi tất cả khách hàng đang hoạt động có email hoặc chọn từng khách hàng.
4. Bấm Gửi thông tin voucher. Trang hiển thị số email gửi thành công/thất bại.
5. Khách hàng đăng nhập và xem voucher đã được cấp tại `/vouchers`.

## Kiểm tra trước khi chạy

- Cần chạy migration/database hiện có để đảm bảo bảng `KhachHangVoucher` và unique index `(KhachHangId, VoucherId)` đã tồn tại. Thay đổi lần này không thêm cột database.
- Build cả API, BlazorAdmin và BlazorKhachHang bằng .NET 8.
- Kiểm tra gửi thử tới email nội bộ trước khi gửi hàng loạt.
