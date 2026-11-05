using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{

    public class HoaDonRepo : IHoaDonRepo
    {
        private readonly DbContextApp _context;

        public HoaDonRepo(DbContextApp context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<HoaDon>> GetAll()
        {
            return await _context.HoaDons
             .AsNoTracking()
             .Include(h => h.voucher)
             .Include(h => h.nhanVien)
             .Include(h => h.hinhThucThanhToan)
             .Include(h => h.khachHang)
             .OrderByDescending(h => h.NgayTao)
             .ToListAsync();
        }

        public async Task<HoaDon> GetById(Guid id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.voucher)
                .Include(h => h.nhanVien)
                .Include(h => h.hinhThucThanhToan)
                .Include(h => h.khachHang)

                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(g => g.Giay)

                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(g => g.KichCo)

                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(g => g.MauSac)

                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(g => g.Anhs)

                .FirstOrDefaultAsync(h => h.HoaDonId == id);

            return hoaDon ?? throw new KeyNotFoundException("Hóa đơn không tồn tại với ID đã cho.");
        }

        public async Task Create(HoaDon hoaDon)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            if (hoaDon.VoucherId.HasValue)
            {
                var assigned = await _context.KhachHangVouchers.AnyAsync(x =>
                    x.VoucherId == hoaDon.VoucherId.Value && x.KhachHangId == hoaDon.KhachHangId);
                if (assigned)
                {
                    var markedUsed = await _context.KhachHangVouchers
                        .Where(x => x.VoucherId == hoaDon.VoucherId.Value
                            && x.KhachHangId == hoaDon.KhachHangId && !x.DaSuDung)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DaSuDung, true));
                    if (markedUsed != 1)
                        throw new InvalidOperationException("Khách hàng đã sử dụng voucher này.");
                }

                var affected = await _context.Vouchers
                    .Where(v => v.VoucherId == hoaDon.VoucherId.Value && v.SoLuong > 0)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(v => v.SoLuong, v => v.SoLuong - 1));
                if (affected != 1)
                    throw new InvalidOperationException("Voucher đã hết lượt sử dụng.");
            }

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task Update(HoaDon hoaDon)

        {
            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null) return false;

            if (hoaDon.VoucherId.HasValue)
            {
                await _context.Vouchers
                    .Where(v => v.VoucherId == hoaDon.VoucherId.Value)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(v => v.SoLuong, v => v.SoLuong + 1));

                var customerVoucher = await _context.KhachHangVouchers
                    .FirstOrDefaultAsync(x => x.VoucherId == hoaDon.VoucherId.Value
                        && x.KhachHangId == hoaDon.KhachHangId);
                if (customerVoucher != null) customerVoucher.DaSuDung = false;
            }

            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        // thêm 
        public async Task<List<HoaDon>> GetByKhachHangId(Guid khachHangId)
        {
            return await _context.HoaDons
                .AsNoTracking()
                .Where(h => h.KhachHangId == khachHangId)
                .Include(h => h.voucher)
                .Include(h => h.hinhThucThanhToan)
                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.Giay)
                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.KichCo)
                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.MauSac)
                .Include(h => h.HoaDonChiTiets)
                    .ThenInclude(ct => ct.GiayChiTiet)
                        .ThenInclude(gct => gct.Anhs)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();
        }
        // thêm 
        public async Task<bool> DatHangAsync(
        HoaDon hoaDon,
        List<HoaDonChiTiet> chiTiets,
        List<Guid> gioHangIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Trừ lượt sử dụng nguyên tử trong cùng transaction với việc tạo hóa đơn.
                // Điều kiện SoLuong > 0 ngăn hai đơn đồng thời dùng vượt hạn mức.
                if (hoaDon.VoucherId.HasValue)
                {
                    // Online checkout chỉ dùng voucher đã cấp cho đúng khách hàng.
                    var markedUsed = await _context.KhachHangVouchers
                        .Where(x => x.VoucherId == hoaDon.VoucherId.Value
                            && x.KhachHangId == hoaDon.KhachHangId && !x.DaSuDung)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DaSuDung, true));

                    if (markedUsed != 1)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    var affected = await _context.Vouchers
                        .Where(v => v.VoucherId == hoaDon.VoucherId.Value && v.SoLuong > 0)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(v => v.SoLuong, v => v.SoLuong - 1));

                    if (affected != 1)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                var giayChiTietIds = chiTiets.Select(x => x.GiayChiTietId).ToList();

                var giayChiTiets = await _context.GiayChiTiets
                    .Where(x => giayChiTietIds.Contains(x.GiayChiTietId))
                    .ToDictionaryAsync(x => x.GiayChiTietId);




                await _context.HoaDons.AddAsync(hoaDon);

                foreach (var ct in chiTiets)
                {
                    ct.HoaDonId = hoaDon.HoaDonId;
                }

                await _context.HoaDonChiTiets.AddRangeAsync(chiTiets);

                // xóa giỏ
                if (gioHangIds != null && gioHangIds.Any())
                {
                    var gioHangs = await _context.GioHangChiTiets
                        .Where(x => gioHangIds.Contains(x.GioHangChiTietId))
                        .ToListAsync();

                    _context.GioHangChiTiets.RemoveRange(gioHangs);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        // thêm hủy đơn
        public async Task<bool> HuyDonHangAsync(Guid hoaDonId, string? lyDo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (hoaDon == null) return false;

            var status = hoaDon.TrangThai?.Trim();

            if (status == "Chờ xác nhận")
            {
                hoaDon.TrangThai = "Đã hủy";
                hoaDon.NgayHuy = DateTime.Now;
                hoaDon.LyDoHuy = string.IsNullOrWhiteSpace(lyDo)
                    ? "Admin hủy"
                    : lyDo;
            }
            else if (status == "Chờ giao hàng" || status == "Đang giao hàng")
            {
                hoaDon.TrangThai = "Đã hủy"; // 👉 admin thì cho hủy luôn
                hoaDon.NgayHuy = DateTime.Now;
                hoaDon.LyDoHuy = string.IsNullOrWhiteSpace(lyDo)
                    ? "Admin hủy"
                    : lyDo;
            }
            else
            {
                throw new Exception($"Không thể hủy đơn ở trạng thái: {hoaDon.TrangThai}");
            }

            if (hoaDon.VoucherId.HasValue)
            {
                await _context.Vouchers
                    .Where(v => v.VoucherId == hoaDon.VoucherId.Value)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(v => v.SoLuong, v => v.SoLuong + 1));

                var customerVoucher = await _context.KhachHangVouchers
                    .FirstOrDefaultAsync(x => x.VoucherId == hoaDon.VoucherId.Value
                        && x.KhachHangId == hoaDon.KhachHangId);
                if (customerVoucher != null) customerVoucher.DaSuDung = false;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }

        public async Task<bool> GiaoThanhCongAsync(Guid id)
        {
            var hoaDon = await _context.HoaDons
                .FirstOrDefaultAsync(h => h.HoaDonId == id);

            if (hoaDon == null)
                return false;

            if (hoaDon.TrangThai != "Đang giao hàng")
                return false;

            // ❌ KHÔNG ĐỤNG TỚI KHO NỮA

            hoaDon.TrangThai = "Hoàn thành";
            hoaDon.HoanThanh = DateTime.Now;
            hoaDon.TrangThaiThanhToan = "Đã thanh toán";

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<(bool Success, string Message)> XacNhanDonAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDonChiTiets)
                        .ThenInclude(ct => ct.GiayChiTiet)
                            .ThenInclude(g => g.Giay)
                    .FirstOrDefaultAsync(x => x.HoaDonId == id);

                if (hoaDon == null)
                    return (false, "Không tìm thấy hóa đơn");

                if (hoaDon.TrangThai != "Chờ xác nhận")
                    return (false, "Đơn không ở trạng thái chờ xác nhận");

                // ✅ CHECK TỒN KHO TRƯỚC
                foreach (var ct in hoaDon.HoaDonChiTiets)
                {
                    var giay = await _context.GiayChiTiets
                        .Include(x => x.Giay)
                        .FirstOrDefaultAsync(x => x.GiayChiTietId == ct.GiayChiTietId);

                    if (giay == null)
                    {
                        return (false, "Sản phẩm không tồn tại");
                    }

                    // ❌ KHÔNG TỰ HỦY ĐƠN
                    // chỉ thông báo tồn kho hiện tại
                    if (giay.SoLuongCon < ct.SoLuongSanPham)
                    {
                        return (
                            false,
                            $"Sản phẩm '{giay.Giay.TenGiay}' hiện chỉ còn {giay.SoLuongCon} sản phẩm trong kho, " +
                            $"không đủ số lượng khách đặt {ct.SoLuongSanPham}."
                        );
                    }
                }

                // ✅ ĐỦ HÀNG MỚI TRỪ KHO
                foreach (var ct in hoaDon.HoaDonChiTiets)
                {
                    var giay = await _context.GiayChiTiets
                        .FirstOrDefaultAsync(x => x.GiayChiTietId == ct.GiayChiTietId);

                    if (giay != null)
                    {
                        giay.SoLuongCon -= ct.SoLuongSanPham;
                    }
                }

                // ✅ CHUYỂN TRẠNG THÁI
                hoaDon.TrangThai = "Chờ giao hàng";
                hoaDon.TGXacNhan = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Xác nhận đơn thành công");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return (false, ex.Message);
            }
        }

        public async Task<bool> BatDauGiaoHangAsync(Guid id)
        {
            var hoaDon = await _context.HoaDons
                .FirstOrDefaultAsync(x => x.HoaDonId == id);

            if (hoaDon == null)
                return false;

            if (hoaDon.TrangThai != "Chờ giao hàng")
                throw new Exception("Sai trạng thái");

            hoaDon.TrangThai = "Đang giao hàng";
            hoaDon.DangGiao = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DuyetHuyDonAsync(Guid hoaDonId, bool chapNhan)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);

            if (hoaDon == null || hoaDon.TrangThai != "Yêu cầu hủy") return false;

            if (chapNhan)
            {
                foreach (var ct in hoaDon.HoaDonChiTiets)
                {
                    var giay = await _context.GiayChiTiets
                        .FirstOrDefaultAsync(x => x.GiayChiTietId == ct.GiayChiTietId);

                    if (giay != null)
                    {

                        hoaDon.TrangThai = "Đã hủy";
                        hoaDon.NgayHuy = DateTime.Now;
                    }
                }
                hoaDon.TrangThai = "Đã hủy";
                hoaDon.NgayHuy = DateTime.Now;
            }
            else
            {
                hoaDon.TrangThai = "Chờ giao hàng";
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }

}
