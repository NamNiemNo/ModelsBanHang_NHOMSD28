using API.IRepository;
using API.IRepository.Repository;
using API.Models.DTO.BanHang;
using API.Models.Momo;
using API.Services.Shipping;
using API.Repository;
using API.Services.AI;
using API.Services.Momo;
using Data.IRepository;
using Data.Models;
using Data.Repositories;
using Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CẤU HÌNH JWT
// =====================================================

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Chưa cấu hình Jwt:Key trong appsettings.json.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Chưa cấu hình Jwt:Issuer trong appsettings.json.");

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Chưa cấu hình Jwt:Audience trong appsettings.json.");

// =====================================================
// CONTROLLER VÀ JSON
// =====================================================

builder.Services
    .AddControllers(options =>
    {
        options.Conventions.Add(
            new PublicApiConvention());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// =====================================================
// XÁC THỰC JWT
// =====================================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.FromMinutes(1),

                NameClaimType =
                    System.Security.Claims
                        .ClaimTypes.Name,

                RoleClaimType =
                    System.Security.Claims
                        .ClaimTypes.Role
            };
    });

builder.Services.AddAuthorization();

// =====================================================
// GIỚI HẠN YÊU CẦU CHAT AI
// =====================================================

builder.Services.AddRateLimiter(options =>
{
    // Trả về HTTP 429 khi vượt giới hạn
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.AddPolicy(
        "ai-chat",
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey:
                    httpContext.Connection
                        .RemoteIpAddress?
                        .ToString()
                    ?? "unknown",

                factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        // Mỗi địa chỉ IP được gửi
                        // tối đa 20 yêu cầu trong 1 phút
                        PermitLimit = 20,

                        Window =
                            TimeSpan.FromMinutes(1),

                        QueueProcessingOrder =
                            QueueProcessingOrder.OldestFirst,

                        // Không xếp hàng yêu cầu vượt giới hạn
                        QueueLimit = 0,

                        AutoReplenishment = true
                    }));
});

// =====================================================
// DATABASE
// =====================================================

builder.Services.AddDbContext<DbContextApp>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection"));
    });

// =====================================================
// SWAGGER
// =====================================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================================
// ĐĂNG KÝ REPOSITORY
// =====================================================

builder.Services.AddScoped<
    IKhachHangVoucherRepo,
    KhachHangVoucherRepo>();

builder.Services.AddScoped<
    IGiayDotGiamGiaRepository,
    GiayDotGiamGiaRepository>();

builder.Services.AddScoped<
    IAnhRepository,
    AnhRepository>();

builder.Services.AddScoped<
    IChatLieuRepository,
    ChatLieuRepository>();

builder.Services.AddScoped<
    IDeGiayRepository,
    DeGiayRepository>();

builder.Services.AddScoped<
    IGiamGiaRepository,
    GiamGiaRepository>();

builder.Services.AddScoped<
    IGiayRepository,
    GiayRepository>();

builder.Services.AddScoped<
    IThongBaoRepository,
    ThongBaoRepository>();

builder.Services.AddScoped<
    IDiaChiKhachHangRepository,
    DiaChiKhachHangRepository>();

builder.Services.AddScoped<
    IGiayChiTietRepository,
    GiayChiTietRepository>();

builder.Services.AddScoped<
    IGioHangRepository,
    GioHangRepository>();

builder.Services.AddScoped<
    IGioHangChiTietRepository,
    GioHangChiTietRepository>();

builder.Services.AddScoped<
    IHoaDonRepo,
    HoaDonRepo>();

builder.Services.AddScoped<
    IMauSacRepository,
    MauSacRepository>();

builder.Services.AddScoped<
    IKichCoRepository,
    KichCoRepository>();

builder.Services.AddScoped<
    INhanVienRepository,
    NhanVienRepository>();

builder.Services.AddScoped<
    IThuongHieuRepository,
    ThuongHieuRepository>();

builder.Services.AddScoped<
    IVoucherRepo,
    VoucherRepo>();

builder.Services.AddScoped<
    IKieuDangRepository,
    KieuDangRepository>();

builder.Services.AddScoped<
    ITheLoaiGiayRepository,
    TheLoaiGiayRepository>();

builder.Services.AddScoped<
    IChiTietHoaDonRepository,
    ChiTietHoaDonRepository>();

builder.Services.AddScoped<
    ITaiKhoanRepository,
    TaiKhoanRepository>();

builder.Services.AddScoped<
    IChucVuRepository,
    ChucVuRepository>();

builder.Services.AddScoped<
    IReturnRepository,
    ReturnRepository>();

builder.Services.AddScoped<
    IGiayYeuThichRepository,
    GiayYeuThichRepository>();

builder.Services.AddScoped<
    IKhachHangRepository,
    KhachHangRepository>();

builder.Services.AddScoped<
    IMomoService,
    MomoService>();

// =====================================================
// MOMO
// =====================================================

builder.Services.Configure<MomoOptionModel>(
    builder.Configuration.GetSection(
        "MomoAPI"));

builder.Services.AddScoped<
    IMomoService,
    MomoService>();

// =====================================================
// PHÍ VẬN CHUYỂN
// =====================================================

// Đọc biểu phí từ phần ShippingFees
// trong file appsettings.json.
builder.Services.Configure<ShippingFeeOptions>(
    builder.Configuration.GetSection(
        ShippingFeeOptions.SectionName));

// Đăng ký service tính phí ship để sử dụng
// trong HoaDonController và PaymentController.
builder.Services.AddScoped<
    IShippingFeeService,
    ShippingFeeService>();

// =====================================================
// GEMINI AI
// =====================================================

builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection(
        GeminiOptions.SectionName));

// Cho phép lấy Gemini API key từ biến môi trường.
// Biến môi trường sẽ ghi đè User Secrets.
builder.Services.PostConfigure<GeminiOptions>(
    options =>
    {
        var apiKey =
            Environment.GetEnvironmentVariable(
                "GEMINI_API_KEY");

        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            options.ApiKey = apiKey;
        }
    });

// Đăng ký HttpClient để gọi Gemini
builder.Services.AddHttpClient(
    "Gemini",
    (serviceProvider, httpClient) =>
    {
        var geminiOptions =
            serviceProvider
                .GetRequiredService<
                    IOptions<GeminiOptions>>()
                .Value;

        if (string.IsNullOrWhiteSpace(
            geminiOptions.BaseUrl))
        {
            throw new InvalidOperationException(
                "Chưa cấu hình Gemini:BaseUrl.");
        }

        var baseUrl =
            geminiOptions.BaseUrl.TrimEnd('/') + "/";

        httpClient.BaseAddress =
            new Uri(baseUrl);

        httpClient.Timeout =
            TimeSpan.FromSeconds(90);
    });


// Chỉ đăng ký AIChatService một lần
builder.Services.AddScoped<
    IAIChatService,
    AIChatService>();

// =====================================================
// BUILD APP
// =====================================================

var app = builder.Build();

// =====================================================
// HTTP PIPELINE
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.MapGet(
        "/startup-id",
        () => StartupState.StartupId.ToString())
    .AllowAnonymous();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "Uploads")),

        RequestPath = "/Uploads"
    });

app.UseHttpsRedirection();

app.UseRouting();

// UseRateLimiter phải đặt sau UseRouting
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.Run();

// =====================================================
// PHÂN QUYỀN API
// =====================================================

internal sealed class PublicApiConvention
    : IActionModelConvention
{
    /*
     * Các Controller được phép gọi GET
     * mà không cần đăng nhập.
     */
    private static readonly HashSet<string>
        PublicReadControllers =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "AnhApi",
                "ChatLieu",
                "DeGiay",
                "GiamGia",
                "GiayChiTiet",
                "Giay",
                "KichCo",
                "KieuDang",
                "MauSac",
                "TheLoaiGiay",
                "ThuongHieu",
                "Voucher",
                "WeatherForecast"
            };

    /*
     * Các Controller quản trị yêu cầu
     * quyền Admin hoặc NhanVien.
     */
    private static readonly HashSet<string>
        ManagementControllers =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "AnhApi",
                "ChatLieu",
                "DeGiay",
                "GiamGia",
                "GiayChiTiet",
                "Giay",
                "KichCo",
                "KieuDang",
                "MauSac",
                "NhanViens",
                "TaiKhoans",
                "TheLoaiGiay",
                "ThongBao",
                "ThuongHieu",
                "Voucher"
            };

    public void Apply(
        ActionModel action)
    {
        var controllerName =
            action.Controller.ControllerName;

        var actionName =
            action.ActionName;

        var isPublicCatalogRead =
            PublicReadControllers
                .Contains(controllerName)
            &&
            action.ActionMethod.IsDefined(
                typeof(HttpGetAttribute),
                true);

        var isCustomerRegistration =
            controllerName.Equals(
                "KhachHang",
                StringComparison.OrdinalIgnoreCase)
            &&
            actionName.Equals(
                "CreateKhachHang",
                StringComparison.OrdinalIgnoreCase);

        /*
         * MoMo gọi endpoint Notify
         * từ máy chủ bên ngoài.
         */
        var isMomoNotify =
            controllerName.Equals(
                "Payment",
                StringComparison.OrdinalIgnoreCase)
            &&
            actionName.Equals(
                "MomoNotify",
                StringComparison.OrdinalIgnoreCase);

        if (isPublicCatalogRead
            || isCustomerRegistration
            || isMomoNotify)
        {
            action.Filters.Add(
                new AllowAnonymousFilter());

            foreach (var selector
                     in action.Selectors)
            {
                selector.EndpointMetadata.Add(
                    new AllowAnonymousAttribute());
            }

            return;
        }

        if (ManagementControllers
            .Contains(controllerName))
        {
            foreach (var selector
                     in action.Selectors)
            {
                selector.EndpointMetadata.Add(
                    new AuthorizeAttribute
                    {
                        Roles =
                            "Admin,NhanVien"
                    });
            }
        }
    }
}