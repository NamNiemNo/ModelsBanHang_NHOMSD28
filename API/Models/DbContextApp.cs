using API.Models;
using API.Models.BoxChatAI;
using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    public class DbContextApp : DbContext
    {
        public DbContextApp()
        {
        }

        public DbContextApp(DbContextOptions<DbContextApp> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Data Source=localhost\\SQLEXPRESS;" +
                    @"Database=TestCSLDs1;" +
                    @"Trusted_Connection=True;" +
                    @"Integrated Security=True;" +
                    @"TrustServerCertificate=True");
                ; 
            }
        }

        // Cấu hình mặc định cho tất cả thuộc tính decimal
        protected override void ConfigureConventions(
            ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder
                .Properties<decimal>()
                .HavePrecision(18, 2);

            configurationBuilder
                .Properties<decimal?>()
                .HavePrecision(18, 2);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // CẤU HÌNH KIỂU DỮ LIỆU TIỀN
            // =========================================================

            modelBuilder.Entity<GioHangChiTiet>()
                .Property(x => x.Gia)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HoaDonChiTiet>()
                .Property(x => x.Gia)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HoaDonChiTiet>()
                .Property(x => x.DonGia)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TraHang>()
                .Property(x => x.GiaTaiThoiDiemTra)
                .HasColumnType("decimal(18,2)");

            // =========================================================
            // QUAN HỆ GIÀY - ĐỢT GIẢM GIÁ
            // =========================================================

            modelBuilder.Entity<GiayDotGiamGia>()
                .HasKey(x => x.GiayDotGiamGiaId);

            modelBuilder.Entity<GiayDotGiamGia>()
                .HasOne(x => x.GiayChiTiet)
                .WithMany()
                .HasForeignKey(x => x.GiayChiTietId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GiayDotGiamGia>()
                .HasOne(x => x.GiamGia)
                .WithMany(x => x.GiayDotGiamGias)
                .HasForeignKey(x => x.GiamGiaId);

            // =========================================================
            // QUAN HỆ CHỨC VỤ - NHÂN VIÊN
            // =========================================================

            modelBuilder.Entity<ChucVu>()
                .HasMany(x => x.nhanViens)
                .WithOne(x => x.ChucVu)
                .HasForeignKey(x => x.ChucVuId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // SEED CHỨC VỤ
            // =========================================================

            modelBuilder.Entity<ChucVu>().HasData(
                new ChucVu
                {
                    ChucVuId = Guid.Parse(
                        "11111111-1111-1111-1111-111111111111"),

                    TenChucVu = "Admin",
                    MotaChucVu = "Quản trị hệ thống",
                    TrangThai = 1
                },
                new ChucVu
                {
                    ChucVuId = Guid.Parse(
                        "22222222-2222-2222-2222-222222222222"),

                    TenChucVu = "NhanVien",
                    MotaChucVu = "Nhân viên bán hàng",
                    TrangThai = 1
                });

            // =========================================================
            // SEED TÀI KHOẢN ADMIN
            // =========================================================

            var adminTaiKhoanId = Guid.Parse(
                "99999999-9999-9999-9999-999999999999");

            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan
                {
                    TaikhoanId = adminTaiKhoanId,
                    Username = "admin",
                    Password = "admin123",
                    Ngaytaotaikhoan = new DateTime(2026, 1, 1)
                });

            // =========================================================
            // SEED NHÂN VIÊN ADMIN
            // =========================================================

            modelBuilder.Entity<NhanVien>().HasData(
                new NhanVien
                {
                    NhanVienId = Guid.Parse(
                        "88888888-8888-8888-8888-888888888888"),

                    HoTen = "Nguyễn Văn Quản Trị",
                    SoDienThoai = "0987654321",
                    Email = "admin@shop.com",
                    NgaySinh = new DateTime(1995, 1, 1),
                    NgayCapNhatCuoiCung = new DateTime(2026, 1, 1),
                    TrangThai = true,
                    TaikhoanId = adminTaiKhoanId,

                    ChucVuId = Guid.Parse(
                        "11111111-1111-1111-1111-111111111111")
                });

            // =========================================================
            // SEED KÍCH CỠ 35 - 50
            // =========================================================

            var kichCoList = new List<KichCo>();

            for (int sizeValue = 35; sizeValue <= 50; sizeValue++)
            {
                var kichCoId = Guid.Parse(
                    $"00000000-0000-0000-0000-{sizeValue:D12}");

                kichCoList.Add(
                    new KichCo
                    {
                        KichCoId = kichCoId,
                        TenKichCo = $"Size {sizeValue}",
                        size = sizeValue,
                        MoTa = $"Cỡ giày {sizeValue}",
                        TrangThai = true
                    });
            }

            modelBuilder.Entity<KichCo>()
                .HasData(kichCoList.ToArray());

            // =========================================================
            // SEED MÀU SẮC
            // =========================================================

            modelBuilder.Entity<MauSac>().HasData(
                new MauSac
                {
                    MauSacId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333301"),

                    TenMau = "Đỏ",
                    Color = "#FF0000",
                    MoTa = "Màu đỏ cơ bản",
                    TrangThai = true
                },
                new MauSac
                {
                    MauSacId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333302"),

                    TenMau = "Xanh dương",
                    Color = "#0000FF",
                    MoTa = "Màu xanh dương cơ bản",
                    TrangThai = true
                },
                new MauSac
                {
                    MauSacId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333303"),

                    TenMau = "Xanh lá",
                    Color = "#00FF00",
                    MoTa = "Màu xanh lá cây",
                    TrangThai = true
                },
                new MauSac
                {
                    MauSacId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333304"),

                    TenMau = "Vàng",
                    Color = "#FFFF00",
                    MoTa = "Màu vàng",
                    TrangThai = true
                },
                new MauSac
                {
                    MauSacId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333305"),

                    TenMau = "Đen",
                    Color = "#000000",
                    MoTa = "Màu đen cơ bản",
                    TrangThai = true
                },
                new MauSac
                {
                    MauSacId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333306"),

                    TenMau = "Trắng",
                    Color = "#FFFFFF",
                    MoTa = "Màu trắng cơ bản",
                    TrangThai = true
                });

            // =========================================================
            // QUAN HỆ KHÁCH HÀNG - VOUCHER
            // =========================================================

            modelBuilder.Entity<KhachHangVoucher>()
                .HasIndex(x => new
                {
                    x.KhachHangId,
                    x.VoucherId
                })
                .IsUnique();

            modelBuilder.Entity<KhachHangVoucher>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.KhachHangVouchers)
                .HasForeignKey(x => x.KhachHangId);

            modelBuilder.Entity<KhachHangVoucher>()
                .HasOne(x => x.Voucher)
                .WithMany(x => x.KhachHangVouchers)
                .HasForeignKey(x => x.VoucherId);

            // =========================================================
            // QUAN HỆ HÓA ĐƠN - NHÂN VIÊN
            // =========================================================

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.nhanVien)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // QUAN HỆ HÓA ĐƠN - KHÁCH HÀNG
            // =========================================================

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.khachHang)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // QUAN HỆ HÓA ĐƠN - HÌNH THỨC THANH TOÁN
            // =========================================================

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.hinhThucThanhToan)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.HinhThucThanhToanId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // QUAN HỆ HÓA ĐƠN - VOUCHER
            // =========================================================

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.voucher)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================================================
            // QUAN HỆ KHÁCH HÀNG - TÀI KHOẢN
            // =========================================================

            modelBuilder.Entity<KhachHang>()
                .HasOne(x => x.TaiKhoan)
                .WithOne(x => x.KhachHang)
                .HasForeignKey<KhachHang>(x => x.TaiKhoanId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // QUAN HỆ GIỎ HÀNG CHI TIẾT - GIỎ HÀNG
            // =========================================================

            modelBuilder.Entity<GioHangChiTiet>()
                .HasOne(x => x.GioHang)
                .WithMany(x => x.GioHangChiTiets)
                .HasForeignKey(x => x.GioHangId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================================
            // QUAN HỆ GIỎ HÀNG CHI TIẾT - GIÀY CHI TIẾT
            // =========================================================

            modelBuilder.Entity<GioHangChiTiet>()
                .HasOne(x => x.GiayChiTiet)
                .WithMany()
                .HasForeignKey(x => x.GiayChiTietId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================================
            // CẤU HÌNH TRẢ HÀNG
            // =========================================================

            modelBuilder.Entity<TraHang>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.GiaTaiThoiDiemTra)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.HoaDon)
                    .WithMany()
                    .HasForeignKey(x => x.HoaDonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.GiayChiTiet)
                    .WithMany()
                    .HasForeignKey(x => x.GiayChiTietId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================================================
            // SEED HÌNH THỨC THANH TOÁN
            // =========================================================

            modelBuilder.Entity<HinhThucThanhToan>().HasData(
                new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.Parse(
                        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),

                    TenHinhThuc = "Thanh toán khi nhận hàng (COD)",
                    MoTa = "Khách thanh toán khi nhận hàng",
                    TrangThai = true
                },
                new HinhThucThanhToan
                {
                    HinhThucThanhToanId = Guid.Parse(
                        "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),

                    TenHinhThuc = "Momo",
                    MoTa = "Thanh toán online",
                    TrangThai = true
                });
            modelBuilder.Entity<CuocTroChuyenAI>(entity =>
            {
                entity.HasIndex(x => new
                {
                    x.KhachHangId,
                    x.NgayCapNhat
                });

                entity.HasIndex(x => new
                {
                    x.PhienKhachId,
                    x.NgayCapNhat
                });

                entity.HasOne(x => x.KhachHang)
                    .WithMany()
                    .HasForeignKey(x => x.KhachHangId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TinNhanAI>(entity =>
            {
                entity.Property(x => x.NoiDung)
                    .HasColumnType("nvarchar(max)");

                entity.HasIndex(x => new
                {
                    x.CuocTroChuyenAIId,
                    x.ThoiGian
                });

                entity.HasOne(x => x.CuocTroChuyen)
                    .WithMany(x => x.TinNhans)
                    .HasForeignKey(x => x.CuocTroChuyenAIId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // =============================================================
        // KHAI BÁO CÁC BẢNG
        // =============================================================

        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }

        public DbSet<Giay> Giays { get; set; }
        public DbSet<GiayChiTiet> GiayChiTiets { get; set; }
        public DbSet<ChatLieu> ChatLieus { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<KichCo> KichCos { get; set; }
        public DbSet<DeGiay> DeGiays { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }
        public DbSet<KieuDang> KieuDangs { get; set; }
        public DbSet<TheLoaiGiay> TheLoaiGiays { get; set; }
        public DbSet<Anh> Anhs { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<GiamGia> GiamGias { get; set; }
        public DbSet<GiayDotGiamGia> GiayDotGiamGias { get; set; }
        public DbSet<KhachHangVoucher> KhachHangVouchers { get; set; }

        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; }

        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<GioHangChiTiet> GioHangChiTiets { get; set; }

        public DbSet<HinhThucThanhToan> hinhThucThanhToans { get; set; }
        public DbSet<DiaChiKhachHang> diaChiKhachHangs { get; set; }

        public DbSet<ThongBao> ThongBaos { get; set; }
        public DbSet<GiayYeuThich> giayYeuThiches { get; set; }

        public DbSet<TraHang> TraHangs { get; set; }

        public DbSet<CuocTroChuyenAI> CuocTroChuyenAIs { get; set; }

        public DbSet<TinNhanAI> TinNhanAIs { get; set; }
    }
}