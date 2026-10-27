using API.Models.DTO.BanHang;
using BlazorKhachHang.Service.IService;

namespace BlazorKhachHang.Service
{
    public class XuLyDiaChi : IXuLyDiaChi
    {
        public async Task<List<DiaChi>> ParseDiaChiAsync(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            var danhSach = new List<DiaChi>();
            DiaChi? currentTinh = null;
            DiaChi? currentHuyen = null;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Contains("-----------"))
                    continue;

                int indent = line.TakeWhile(c => c == ' ').Count();

                if (indent == 0)
                {
                    currentTinh = new DiaChi { Ten = trimmed };
                    danhSach.Add(currentTinh);
                }
                else if (indent >= 2 && indent < 8 && currentTinh != null)
                {
                    currentHuyen = new DiaChi { Ten = trimmed };
                    currentTinh.Con.Add(currentHuyen);
                }
                else if (indent >= 8 && currentHuyen != null)
                {
                    currentHuyen.Con.Add(new DiaChi { Ten = trimmed });
                }
            }

            return danhSach;
        }
    }
}
