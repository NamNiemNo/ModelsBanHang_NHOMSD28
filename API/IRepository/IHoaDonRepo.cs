using Data.Models;

namespace API.IRepository
{
    public interface IHoaDonRepo
    {
        Task<List<HoaDon>> GetAll();
        Task<HoaDon> GetById(Guid id);
        Task Create(HoaDon hoaDon);
        Task Update(HoaDon hoaDon);
        Task<bool> Delete(Guid id);
        Task<List<HoaDon>> GetByKhachHangId(Guid khachHangId);// thêm 
        Task<bool> DatHangAsync(HoaDon hoaDon,List<HoaDonChiTiet> chiTiets, List<Guid> gioHangChiTietIds );// thêm
        Task<bool> HuyDonHangAsync(Guid hoaDonId, string lyDo); // thêm hủy đơn
        Task<bool> GiaoThanhCongAsync(Guid id);
        Task<(bool Success, string Message)> XacNhanDonAsync(Guid id);
        Task<bool> BatDauGiaoHangAsync(Guid id);
        Task<bool> DuyetHuyDonAsync(Guid hoaDonId, bool chapNhan);

    }
}
