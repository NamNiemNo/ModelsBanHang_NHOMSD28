using API.Models.DTO.BanHang;
using BlazorAdmin.Service.IService;
using Data.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorAdmin.Service
{
    public class HoaDonService : IHoaDonService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions
            JsonOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

        public HoaDonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =====================================================
        // TẠO HÓA ĐƠN
        // =====================================================

        public async Task<HoaDon?> Add(HoaDon hoaDon)
        {
            try
            {
                var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/HoaDon",
                        hoaDon);

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Content
                            .ReadAsStringAsync();

                    Console.WriteLine(
                        $"Lỗi tạo hóa đơn: {error}");

                    return null;
                }

                string content =
                    await response.Content
                        .ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<HoaDon>(
                    content,
                    JsonOptions);
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    $"Lỗi tạo hóa đơn: " +
                    $"{exception.Message}");

                return null;
            }
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

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không cập nhật được hóa đơn."
                        : error);
            }
        }

        // =====================================================
        // XÓA HÓA ĐƠN
        // =====================================================

        public async Task Delete(Guid id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/HoaDon/{id}");

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không xóa được hóa đơn."
                        : error);
            }
        }

        // =====================================================
        // LẤY PHÍ VẬN CHUYỂN TỪ API
        // =====================================================

        public async Task<DatHang.TinhPhiShipResponse>
            TinhPhiShipAsync(
                DatHang.TinhPhiShipRequest request)
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
                        DatHang.TinhPhiShipResponse>();

            if (result == null)
            {
                throw new InvalidOperationException(
                    "API không trả về dữ liệu phí vận chuyển.");
            }

            return result;
        }

        // =====================================================
        // XÁC NHẬN ĐƠN
        // =====================================================

        public async Task<(bool Success, string Message)>
            XacNhanDon(Guid id)
        {
            var response =
                await _httpClient.PutAsync(
                    $"api/HoaDon/xac-nhan/{id}",
                    null);

            string message =
                await response.Content
                    .ReadAsStringAsync();

            return (
                response.IsSuccessStatusCode,
                message
            );
        }

        // =====================================================
        // BẮT ĐẦU GIAO HÀNG
        // =====================================================

        public async Task BatDauGiao(Guid id)
        {
            var response =
                await _httpClient.PutAsync(
                    $"api/HoaDon/bat-dau-giao/{id}",
                    null);

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không chuyển được trạng thái đơn."
                        : error);
            }
        }

        // =====================================================
        // GIAO HÀNG THÀNH CÔNG
        // =====================================================

        public async Task GiaoThanhCong(Guid id)
        {
            var response =
                await _httpClient.PutAsync(
                    $"api/HoaDon/giao-thanh-cong/{id}",
                    null);

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Content
                        .ReadAsStringAsync();

                throw new InvalidOperationException(
                    string.IsNullOrWhiteSpace(error)
                        ? "Không hoàn thành được đơn hàng."
                        : error);
            }
        }

        // =====================================================
        // HỦY ĐƠN
        // =====================================================

        public async Task HuyDon(Guid id)
        {
            var response =
                await _httpClient.PutAsync(
                    $"api/HoaDon/huy-don/{id}",
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
    }
}