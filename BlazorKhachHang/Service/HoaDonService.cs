using API.Models.DTO.BanHang;
using BlazorKhachHang.Service.IService;
using Data.Models;
using System.Net.Http.Json;
using static API.Models.DTO.BanHang.DatHang;

namespace BlazorKhachHang.Service
{
    public class HoaDonService : IHoaDonService
    {
        private readonly HttpClient _httpClient;

        public HoaDonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =====================================================
        // TẠO HÓA ĐƠN
        // =====================================================

        public async Task Add(HoaDon hoaDon)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/HoaDon",
                    hoaDon);

            response.EnsureSuccessStatusCode();
        }

        // =====================================================
        // LẤY TOÀN BỘ HÓA ĐƠN
        // =====================================================

        public async Task<List<HoaDon>> GetAll()
        {
            return await _httpClient
                .GetFromJsonAsync<List<HoaDon>>(
                    "api/HoaDon")
                ?? new List<HoaDon>();
        }

        // =====================================================
        // LẤY HÓA ĐƠN THEO ID
        // =====================================================

        public async Task<HoaDon?> GetById(Guid id)
        {
            return await _httpClient
                .GetFromJsonAsync<HoaDon>(
                    $"api/HoaDon/{id}");
        }

        // =====================================================
        // CẬP NHẬT HÓA ĐƠN
        // =====================================================

        public async Task Update(HoaDon hoaDon)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/HoaDon/{hoaDon.HoaDonId}",
                    hoaDon);

            response.EnsureSuccessStatusCode();
        }

        // =====================================================
        // XÓA HÓA ĐƠN
        // =====================================================

        public async Task Delete(Guid id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/HoaDon/{id}");

            response.EnsureSuccessStatusCode();
        }

        // =====================================================
        // ĐẶT HÀNG COD
        // =====================================================

        public async Task<bool> DatHangAsync(
            DatHangRequest request)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/HoaDon/dat-hang",
                    request);

            return response.IsSuccessStatusCode;
        }

        // =====================================================
        // LẤY PHÍ SHIP TỪ API
        // =====================================================

        public async Task<TinhPhiShipResponse>
            TinhPhiShipAsync(
                TinhPhiShipRequest request)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/HoaDon/tinh-phi-ship",
                    request);

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không tính được phí vận chuyển."
                        : error);
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<
                        TinhPhiShipResponse>();

            if (result == null)
            {
                throw new InvalidOperationException(
                    "API không trả về dữ liệu phí vận chuyển.");
            }

            return result;
        }

        // =====================================================
        // LẤY ĐƠN HÀNG CỦA KHÁCH HÀNG
        // =====================================================

        public async Task<List<HoaDonKhachHangDTO>>
            GetDonHangKhachHangAsync(
                Guid khachHangId)
        {
            return await _httpClient
                .GetFromJsonAsync<
                    List<HoaDonKhachHangDTO>>(
                    $"api/HoaDon/khach-hang/{khachHangId}")
                ?? new List<HoaDonKhachHangDTO>();
        }

        // =====================================================
        // LẤY ĐƠN HÀNG THEO TRẠNG THÁI
        // =====================================================

        public async Task<List<HoaDonKhachHangDTO>>
            GetDonHangKhachHangAsync(
                Guid khachHangId,
                string trangThai)
        {
            string encodedStatus =
                Uri.EscapeDataString(
                    trangThai ?? "");

            return await _httpClient
                .GetFromJsonAsync<
                    List<HoaDonKhachHangDTO>>(
                    $"api/HoaDon/khach-hang/" +
                    $"{khachHangId}" +
                    $"?trangThai={encodedStatus}")
                ?? new List<HoaDonKhachHangDTO>();
        }

        // =====================================================
        // CẬP NHẬT THÔNG TIN GIAO HÀNG
        // =====================================================

        public async Task
            CapNhatThongTinGiaoHangAsync(
                UpdateThongTinGiaoHangDTO dto)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    "api/HoaDon/cap-nhat-giao-hang",
                    dto);

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không cập nhật được thông tin giao hàng."
                        : error);
            }
        }

        // =====================================================
        // HỦY ĐƠN HÀNG
        // =====================================================

        public async Task HuyDonHangAsync(
            Guid hoaDonId,
            string lyDo)
        {
            string encodedReason =
                Uri.EscapeDataString(
                    lyDo ?? "");

            string url =
                $"api/HoaDon/huy-don/{hoaDonId}" +
                $"?lyDo={encodedReason}";

            var response =
                await _httpClient.PutAsync(
                    url,
                    null);

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không hủy được đơn hàng."
                        : error);
            }
        }

        // =====================================================
        // LẤY CHI TIẾT HÓA ĐƠN
        // =====================================================

        public async Task<HoaDonKhachHangDTO?>
            GetHoaDonByIdAsync(
                Guid hoaDonId)
        {
            return await _httpClient
                .GetFromJsonAsync<
                    HoaDonKhachHangDTO>(
                    $"api/HoaDon/chi-tiet/{hoaDonId}");
        }
    }
}