using API.Models.DTO.BanHang;
using Data.Models;

namespace BlazorAdmin.Service.IService
{
    public interface IHoaDonService
    {
        Task<HoaDon?> Add(HoaDon hoaDon);

        Task<List<HoaDon>> GetAll();

        Task<HoaDon?> GetById(Guid id);

        Task Update(HoaDon hoaDon);

        Task Delete(Guid id);

        // Lấy phí vận chuyển từ API.
        Task<DatHang.TinhPhiShipResponse>
            TinhPhiShipAsync(
                DatHang.TinhPhiShipRequest request);

        Task<(bool Success, string Message)>
            XacNhanDon(Guid id);

        Task BatDauGiao(Guid id);

        Task GiaoThanhCong(Guid id);

        Task HuyDon(Guid id);
    }
}