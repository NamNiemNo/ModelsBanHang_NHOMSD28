using API.Models.DTO;
using BlazorKhachHang.Components.Pages.TaiKhoan;
using BlazorKhachHang.Service.IService;
using Data.Models;
using System.Net.Http.Json;

namespace BlazorKhachHang.Service
{
    public class GioHangService : IGioHangService
    {
        private readonly HttpClient _http;

        public GioHangService(HttpClient http)
        {
            _http = http;
        }

        public async Task AddToCartAsync(
            Guid khachHangId,
            Guid giayChiTietId,
            int soLuong)
        {
            // 1. Lấy hoặc tạo giỏ
            var allCarts = await _http
                .GetFromJsonAsync<List<GioHang>>("api/GioHangs/GetAllGioHang");

            var userCart = allCarts?
                .FirstOrDefault(x => x.KhachHangId == khachHangId);

            if (userCart == null)
            {
                var createPayload = new
                {
                    GioHangId = Guid.NewGuid(),
                    KhachHangId = khachHangId,
                    NgayTaoGioHang = DateTime.Now,
                    NgayCapNhatCuoiCung = DateTime.Now,
                    TrangThai = true
                };

                var res = await _http.PostAsJsonAsync(
                    "api/GioHangs/Create",
                    createPayload);

                if (!res.IsSuccessStatusCode)
                    throw new Exception("Không thể tạo giỏ hàng");

                userCart = new GioHang
                {
                    GioHangId = createPayload.GioHangId,
                    KhachHangId = khachHangId
                };
            }

            // 2. Lấy chi tiết giày
            var chiTiet = await _http
                .GetFromJsonAsync<GiayChiTiet>($"api/GiayChiTiet/{giayChiTietId}");

            if (chiTiet == null)
                throw new Exception("Không tìm thấy sản phẩm");

            // 3. GỬI ADD TO CART
            var payload = new
            {
                GioHangId = userCart.GioHangId,
                GiayChiTietId = giayChiTietId,
                GiayId = chiTiet.GiayId,   // 🔥 DÒNG QUAN TRỌNG
                SoLuongSanPham = soLuong,
                Gia = chiTiet.Gia,
                TrangThai = true
            };

            var response = await _http.PostAsJsonAsync(
                "api/GioHangChiTiet/AddToCart",
                payload
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
        }

        public async Task<GioHangDTO> LayTheoTaiKhoanAsync(Guid taiKhoanId)
        {
            var dto = await _http.GetFromJsonAsync<GioHangDTO>(
                $"api/GioHangs/khach-hang/{taiKhoanId}"
            );

            return dto ?? new GioHangDTO();
        }

        public async Task CapNhatSoLuong(
     Guid chiTietId,
     int soLuong)
        {
            var response = await _http.PutAsync(
                $"api/GioHangs/cap-nhat-so-luong?id={chiTietId}&soLuong={soLuong}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();

                throw new Exception(msg);
            }
        }

        public async Task XoaSanPham(Guid chiTietId)
        {
            await _http.DeleteAsync($"api/GioHangChiTiet/{chiTietId}");
        }
    }
}