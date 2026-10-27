using API.Models;

namespace API.IRepository
{
    public interface IThongBaoRepository
    {
        Task<List<ThongBao>> GetThongBaoMoiAsync();
        Task ThemThongBaoAsync(string noiDung);
    }
}
