using API.IService;
using API.Models.DTO;
using Blazored.LocalStorage;
using BlazorClient.Services;
using BlazorKhachHang.Components;
using BlazorKhachHang.Service;
using BlazorKhachHang.Service.IService;
using BlazorKhachHang.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// RAZOR COMPONENTS
// =====================================================

builder.Services.AddBlazorBootstrap();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// =====================================================
// LOCAL STORAGE
// Bắt buộc cho ChatAIClientService
// =====================================================

builder.Services.AddBlazoredLocalStorage();

// =====================================================
// AUTHENTICATION
// =====================================================

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();

// =====================================================
// API URL
// =====================================================

const string apiUrl = "https://localhost:7246/";

builder.Services.AddScoped<ApiAuthorizationHandler>();

builder.Services.AddScoped<HttpClient>(serviceProvider =>
{
    var authorizationHandler =
        serviceProvider.GetRequiredService<ApiAuthorizationHandler>();

    authorizationHandler.InnerHandler =
        new HttpClientHandler();

    return new HttpClient(authorizationHandler)
    {
        BaseAddress = new Uri(apiUrl)
    };
});

// =====================================================
// SERVICE CỦA GIAO DIỆN KHÁCH HÀNG
// =====================================================

builder.Services.AddScoped<
    IGioHangService,
    GioHangService>();

builder.Services.AddScoped<GioHangService>();

builder.Services.AddScoped<
    IDiaChiKhachHangService,
    DiaChiKhachHangService>();

builder.Services.AddScoped<
    IHoaDonChiTietService,
    HoaDonChiTietService>();

builder.Services.AddScoped<
    INhanVienService,
    NhanVienService>();

builder.Services.AddScoped<
    IAnhService,
    AnhService>();

builder.Services.AddScoped<
    IChatLieuService,
    ChatLieuService>();

builder.Services.AddScoped<
    IChiTietHoaDonService,
    ChiTietHoaDonService>();

builder.Services.AddScoped<
    IDeGiayService,
    DeGiayService>();

builder.Services.AddScoped<
    IGiamGiaService,
    GiamGiaService>();

builder.Services.AddScoped<
    IGiayChiTietService,
    GiayChiTietService>();

builder.Services.AddScoped<
    IGiayService,
    GiayService>();

builder.Services.AddScoped<
    IHoaDonService,
    HoaDonService>();

builder.Services.AddScoped<
    ITheLoaiGiayService,
    TheLoaiGiayService>();

builder.Services.AddScoped<
    IThuongHieuService,
    ThuongHieuService>();

builder.Services.AddScoped<
    IVoucherService,
    VoucherService>();

builder.Services.AddScoped<
    ITaiKhoanService,
    TaiKhoanService>();

builder.Services.AddScoped<
    IKieuDangService,
    KieuDangService>();

builder.Services.AddScoped<
    IMauSacService,
    MauSacService>();

builder.Services.AddScoped<
    IKichCoService,
    KichCoService>();

builder.Services.AddScoped<
    IKhachHangService,
    KhachHangService>();

builder.Services.AddScoped<
    IReturnService,
    ReturnService>();

builder.Services.AddScoped<
    IThongBaoService,
    ThongBaoService>();

builder.Services.AddScoped<
    IGiayYeuThichService,
    GiayYeuThichService>();

builder.Services.AddScoped<
    IXuLyDiaChi,
    XuLyDiaChi>();

builder.Services.AddScoped<
    IHoaDonService,
    HoaDonService>();

// =====================================================
// AI CHAT
// Chỉ đăng ký đúng một lần
// =====================================================

builder.Services.AddScoped<
    IChatAIClientService,
    ChatAIClientService>();

// =====================================================
// STATE VÀ SERVICE PHỤ
// =====================================================

builder.Services.AddScoped<LoginPhanQuyen>();
builder.Services.AddScoped<CartState>();
builder.Services.AddScoped<FavoriteState>();

// =====================================================
// BUILD APPLICATION
// =====================================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();