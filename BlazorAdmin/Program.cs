using API.IService;
using API.Models.DTO;
using BlazorAdmin.Components;
using BlazorAdmin.Service;
using BlazorAdmin.Service.IService;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazorBootstrap();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization
                .ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions
                .DefaultIgnoreCondition =
            System.Text.Json.Serialization
                .JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults
            .AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan =
            TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "RequireAdminRole",
        policy => policy.RequireRole("Admin"));
});

builder.Services.AddAuthorizationCore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string apiUrl =
    "https://localhost:7246/";

builder.Services
    .AddScoped<ApiAuthorizationHandler>();

builder.Services.AddScoped<HttpClient>(
    serviceProvider =>
    {
        var authorizationHandler =
            serviceProvider
                .GetRequiredService<
                    ApiAuthorizationHandler>();

        authorizationHandler.InnerHandler =
            new HttpClientHandler();

        return new HttpClient(
            authorizationHandler)
        {
            BaseAddress = new Uri(apiUrl)
        };
    });

builder.Services.AddScoped<
    IKhachHangVoucherService,
    KhachHangVoucherService>();

builder.Services.AddScoped<
    IXuLyDiaChi,
    XuLyDiaChi>();

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
    IDiaChiKhachHangService,
    DiaChiKhachHangService>();

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

builder.WebHost.UseWebRoot("wwwroot");

builder.Services
    .AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddScoped<
    CustomAuthenticationStateProvider>();

builder.Services.AddScoped<
    AuthenticationStateProvider>(
    serviceProvider =>
        serviceProvider.GetRequiredService<
            CustomAuthenticationStateProvider>());

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize =
        52_428_800;
});

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