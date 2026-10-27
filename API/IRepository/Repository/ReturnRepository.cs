using API.IRepository;
using API.Models;
using API.Models.DTO.TraHang;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly DbContextApp _context;

        public ReturnRepository(DbContextApp context)
        {
            _context = context;
        }
        public async Task<List<HoaDon>> GetHoaDonWithChiTietAsync(string maHoaDon = null, string tenKhachHang = null, string sdt = null, DateTime? ngayTao = null, string trangThai = null)
        {
            var query = _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .ThenInclude(hdc => hdc.GiayChiTiet)
                   .Include(h => h.nhanVien)
                 .Include(h => h.khachHang)
                .Include(h => h.hinhThucThanhToan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(maHoaDon))
                query = query.Where(h => h.HoaDonId.ToString().Contains(maHoaDon));

            if (ngayTao.HasValue)
                query = query.Where(h => h.NgayTao.Date == ngayTao.Value.Date);
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(h => h.TrangThai.ToLower().Contains(trangThai.ToLower()));

            return await query.OrderByDescending(h => h.NgayTao).ToListAsync();
        }

        public async Task<HoaDon?> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            return await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .ThenInclude(hdc => hdc.GiayChiTiet)
                .ThenInclude(gct => gct.Giay)
                .Include(h => h.nhanVien)
                 .Include(h => h.khachHang)
                .Include(h => h.hinhThucThanhToan)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);
        }

        public async Task<List<HoaDonChiTiet>> GetLichSuTraHangAsync(Guid hoaDonId)
        {
            return await _context.HoaDonChiTiets
                .Where(hdc => hdc.HoaDonId == hoaDonId && hdc.TrangThai == true)
                .Include(hdc => hdc.GiayChiTiet)
                  .ThenInclude(gct => gct.Giay)
                .OrderByDescending(hdc => hdc.NgayTraHang)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetSoLuongConLaiChoTraAsync(Guid hoaDonId)
        {
            // Lấy tất cả chi tiết hóa đơn (dòng mua hàng)
            var chiTiets = await _context.HoaDonChiTiets
                .Where(hdc => hdc.HoaDonId == hoaDonId && hdc.TrangThai == false)
                .ToListAsync();

            // Lấy tất cả các yêu cầu trả hàng liên quan (Chờ duyệt hoặc Đã duyệt)
            var traHangs = await _context.TraHangs
                .Where(t => t.HoaDonId == hoaDonId && (t.TrangThai == 1 || t.TrangThai == 2))
                .ToListAsync();

            var result = new Dictionary<Guid, int>();
            foreach (var group in chiTiets.GroupBy(hdc => hdc.GiayChiTietId))
            {
                var soLuongMua = group.Sum(hdc => hdc.SoLuongSanPham);

                var soLuongDangTra = traHangs
                    .Where(t => t.GiayChiTietId == group.Key)
                    .Sum(t => t.SoLuong);

                result[group.Key] = soLuongMua - soLuongDangTra;
            }
            return result;
        }

        public async Task<bool> KiemTraConHanTraHangAsync(Guid hoaDonId)
        {
            var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);
            if (hoaDon == null) return false;
            return (DateTime.UtcNow - hoaDon.NgayTao).TotalDays <= 7;
        }

        public async Task<bool> SanPhamNamTrongHoaDon(Guid hoaDonId, Guid giayId)
        {
            return await _context.HoaDonChiTiets
                .AnyAsync(hdc => hdc.HoaDonId == hoaDonId && hdc.GiayChiTietId == giayId);
        }

        public async Task<bool> KiemTraDuocPhepTra(Guid hoaDonId, Guid giayId, int soLuongMuonTra)
        {
            var soLuongConLai = await GetSoLuongConLaiChoTraAsync(hoaDonId);
            return soLuongConLai.ContainsKey(giayId) && soLuongConLai[giayId] >= soLuongMuonTra;
        }

        public async Task<bool> TraHangAsync(Guid hoaDonId, List<ChiTietTraHangDTO> items, Guid taiKhoanId)
        {
            if (!await KiemTraConHanTraHangAsync(hoaDonId)) return false;

            foreach (var item in items)
            {
                // Kiểm tra số lượng dựa trên logic mới (tránh trả vượt quá số lượng đã mua)
                if (!await KiemTraDuocPhepTra(hoaDonId, item.GiayChiTietId, item.SoLuong)) return false;

                var giaGoc = (await _context.HoaDonChiTiets
                    .FirstAsync(hdc => hdc.HoaDonId == hoaDonId && hdc.GiayChiTietId == item.GiayChiTietId && !hdc.TrangThai)).Gia;

                var yeuCauTra = new TraHang
                {
                    Id = Guid.NewGuid(),
                    HoaDonId = hoaDonId,
                    GiayChiTietId = item.GiayChiTietId,
                    SoLuong = item.SoLuong,
                    GiaTaiThoiDiemTra = giaGoc,
                    NgayYeuCau = DateTime.UtcNow,
                    GhiChu = item.GhiChu,
                    TrangThai = 1 // Chờ xác nhận
                };
                _context.TraHangs.Add(yeuCauTra);
            }

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task CapNhatTrangThaiHoaDonNeuTraHet(Guid hoaDonId)
        {
            var soLuongConLai = await GetSoLuongConLaiChoTraAsync(hoaDonId);
            if (soLuongConLai.Values.All(sl => sl == 0))
            {
                var hoaDon = await _context.HoaDons.FirstAsync(h => h.HoaDonId == hoaDonId);
                hoaDon.TrangThai = "DaTraHet";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<HoaDonChiTiet>> GetDanhSachSanPhamTrongHoaDon(Guid hoaDonId)
        {
            return await _context.HoaDonChiTiets
                .Where(hdc => hdc.HoaDonId == hoaDonId)
              .Include(hdc => hdc.GiayChiTiet)
                .ThenInclude(gct => gct.Giay)
                .OrderByDescending(hdc => hdc.NgayTraHang ?? DateTime.MaxValue)
                .ToListAsync();
        }

        public async Task<bool> HuyTraHangAsync(Guid hoaDonChiTietId)
        {
            var chiTiet = await _context.HoaDonChiTiets
                .FirstOrDefaultAsync(hdc => hdc.HoaDonChiTietId == hoaDonChiTietId && hdc.TrangThai == true);
            if (chiTiet == null) return false;

            _context.HoaDonChiTiets.Remove(chiTiet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CapNhatGhiChuTraHang(Guid hoaDonChiTietId, string ghiChuMoi)
        {
            var chiTiet = await _context.HoaDonChiTiets
                .FirstOrDefaultAsync(hdc => hdc.HoaDonChiTietId == hoaDonChiTietId && hdc.TrangThai == true);
            if (chiTiet == null) return false;

            chiTiet.GhiChu = ghiChuMoi;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetTenGiayAsync(Guid giayId)
        {
            var giay = await _context.Giays.FirstOrDefaultAsync(g => g.GiayId == giayId);
            return giay?.TenGiay ?? "Unknown";
        }
        public async Task<bool> XacNhanTraHangAsync(Guid traHangId)
        {
            var request = await _context.TraHangs.FindAsync(traHangId);
            if (request == null || request.TrangThai != 1) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                request.TrangThai = 2; // Đã xác nhận

                // Cập nhật tồn kho
                var kho = await _context.GiayChiTiets.FindAsync(request.GiayChiTietId);
                if (kho != null) kho.SoLuongCon += request.SoLuong;

                await _context.SaveChangesAsync();

                // Kiểm tra nếu tất cả SP trong HD đã được trả hết (Trạng thái 2)
                await CapNhatTrangThaiHoaDonNeuTraHet(request.HoaDonId);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // THÊM MỚI: Phương thức từ chối yêu cầu trả hàng
        public async Task<bool> TuChoiTraHangAsync(Guid traHangId, string lyDo)
        {
            var request = await _context.TraHangs.FindAsync(traHangId);
            if (request == null || request.TrangThai != 1) return false;

            request.TrangThai = 3;
            request.LyDoTuChoi = lyDo;

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<List<TraHang>> GetYeuCauTraHangTheoHoaDonAsync(Guid hoaDonId)
        {
            return await _context.TraHangs
                .Where(x => x.HoaDonId == hoaDonId)
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(x => x.Giay)
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(x => x.MauSac)
                .Include(x => x.GiayChiTiet)
                    .ThenInclude(x => x.KichCo)
                .OrderByDescending(x => x.NgayYeuCau)
                .ToListAsync();
        }
    }
}