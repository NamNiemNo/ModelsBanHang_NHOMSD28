using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using API.Models.BoxChatAI;
using API.Models.DTO.ChatAI;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API.Services.AI
{
    public class AIChatService : IAIChatService
    {
        private const int SoTinNhanNguCanh = 14;
        private const int SoSanPhamGoiY = 5;

        private static readonly HashSet<string> TuDung =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "ai", "anh", "ban", "biet", "cac", "can", "cho",
                "co", "cua", "dang", "de", "di", "duoc", "gia",
                "giay", "giup", "hay", "khong", "la", "lam",
                "minh", "mot", "muon", "nao", "nhung", "pham",
                "phu", "san", "tai", "tam", "the", "thi", "tim",
                "toi", "tu", "van", "voi", "xin"
            };

        private static readonly string[] CumTuGoiY =
        {
            "tu van", "goi y", "phu hop", "san pham nao",
            "giay nao", "di hoc", "di lam", "chay bo",
            "tap gym", "ban chay", "moi nhat", "chon size",
            "huong dan"
        };

        private readonly DbContextApp _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GeminiOptions _geminiOptions;
        private readonly ILogger<AIChatService> _logger;

        public AIChatService(
            DbContextApp dbContext,
            IHttpClientFactory httpClientFactory,
            IOptions<GeminiOptions> openAIOptions,
            ILogger<AIChatService> logger)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
            _geminiOptions = openAIOptions.Value;
            _logger = logger;
        }

        public async Task<ChatResponseDTO> GuiTinNhanAsync(
            ChatRequestDTO request,
            Guid? khachHangId,
            CancellationToken cancellationToken)
        {
            if (request.PhienKhachId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Phiên khách không hợp lệ.");
            }

            var noiDung = request.NoiDung?.Trim();

            if (string.IsNullOrWhiteSpace(noiDung))
            {
                throw new ArgumentException(
                    "Nội dung tin nhắn không được để trống.");
            }

            if (noiDung.Length > 2000)
            {
                throw new ArgumentException(
                    "Nội dung tin nhắn tối đa 2000 ký tự.");
            }

            CuocTroChuyenAI? cuocTroChuyen = null;

            if (request.CuocTroChuyenId.HasValue)
            {
                cuocTroChuyen =
                    await TimCuocTroChuyenAsync(
                        request.CuocTroChuyenId.Value,
                        request.PhienKhachId,
                        khachHangId,
                        cancellationToken);

                if (cuocTroChuyen == null)
                {
                    throw new KeyNotFoundException(
                        "Không tìm thấy cuộc trò chuyện.");
                }
            }

            if (cuocTroChuyen == null)
            {
                cuocTroChuyen = new CuocTroChuyenAI
                {
                    CuocTroChuyenAIId = Guid.NewGuid(),
                    KhachHangId = khachHangId,
                    PhienKhachId = request.PhienKhachId,
                    TieuDe = TaoTieuDe(noiDung),
                    NgayTao = DateTime.UtcNow,
                    NgayCapNhat = DateTime.UtcNow,
                    DaXoa = false
                };

                _dbContext.CuocTroChuyenAIs.Add(
                    cuocTroChuyen);
            }

            if (khachHangId.HasValue &&
                cuocTroChuyen.KhachHangId == null)
            {
                cuocTroChuyen.KhachHangId =
                    khachHangId.Value;
            }

            var tinNhanKhach = new TinNhanAI
            {
                TinNhanAIId = Guid.NewGuid(),
                CuocTroChuyenAIId =
                    cuocTroChuyen.CuocTroChuyenAIId,
                VaiTro = "user",
                NoiDung = noiDung,
                ThoiGian = DateTime.UtcNow
            };

            var sanPhams =
                await TimSanPhamAsync(
                    noiDung,
                    cancellationToken);

            var lichSu =
                await _dbContext.TinNhanAIs
                    .AsNoTracking()
                    .Where(x =>
                        x.CuocTroChuyenAIId ==
                        cuocTroChuyen.CuocTroChuyenAIId)
                    .OrderByDescending(x => x.ThoiGian)
                    .Take(SoTinNhanNguCanh)
                    .OrderBy(x => x.ThoiGian)
                    .ToListAsync(cancellationToken);

            // Chỉ lưu khi OpenAI trả lời thành công.
            // Tránh tạo tin nhắn mồ côi khi dịch vụ AI lỗi.
            lichSu.Add(tinNhanKhach);

            var noiDungTraLoi =
                await GoiGeminiAsync(
                lichSu,
                sanPhams,
                cancellationToken);

            var tinNhanAI = new TinNhanAI
            {
                TinNhanAIId = Guid.NewGuid(),
                CuocTroChuyenAIId =
                    cuocTroChuyen.CuocTroChuyenAIId,
                VaiTro = "assistant",
                NoiDung = noiDungTraLoi,
                ModelAI = LayModelDangDung(),
                ThoiGian = DateTime.UtcNow
            };

            _dbContext.TinNhanAIs.Add(tinNhanKhach);
            _dbContext.TinNhanAIs.Add(tinNhanAI);

            cuocTroChuyen.NgayCapNhat =
                DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return new ChatResponseDTO
            {
                CuocTroChuyenId =
                    cuocTroChuyen.CuocTroChuyenAIId,
                TinNhan =
                    ChuyenTinNhanSangDTO(tinNhanAI),
                SanPhams = sanPhams
            };
        }

        public async Task<List<ChatConversationDTO>>
            LayLichSuAsync(
                Guid phienKhachId,
                Guid? khachHangId,
                CancellationToken cancellationToken)
        {
            // Khi khách đăng nhập, chuyển lịch sử chat
            // của đúng phiên trình duyệt sang tài khoản.
            if (khachHangId.HasValue)
            {
                await _dbContext.CuocTroChuyenAIs
                    .Where(x =>
                        !x.DaXoa &&
                        x.KhachHangId == null &&
                        x.PhienKhachId == phienKhachId)
                    .ExecuteUpdateAsync(
                        setters =>
                            setters.SetProperty(
                                x => x.KhachHangId,
                                (Guid?)khachHangId.Value),
                        cancellationToken);
            }

            return await TruyVanTheoChuSoHuu(
                    phienKhachId,
                    khachHangId)
                .AsNoTracking()
                .OrderByDescending(x => x.NgayCapNhat)
                .Take(20)
                .Select(x => new ChatConversationDTO
                {
                    CuocTroChuyenId =
                        x.CuocTroChuyenAIId,
                    TieuDe = x.TieuDe,
                    NgayCapNhat = x.NgayCapNhat,
                    TinNhans = x.TinNhans
                        .OrderByDescending(m => m.ThoiGian)
                        .Take(60)
                        .OrderBy(m => m.ThoiGian)
                        .Select(m => new ChatMessageDTO
                        {
                            TinNhanId = m.TinNhanAIId,
                            VaiTro = m.VaiTro,
                            NoiDung = m.NoiDung,
                            ThoiGian = m.ThoiGian
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> XoaAsync(
            Guid cuocTroChuyenId,
            Guid phienKhachId,
            Guid? khachHangId,
            CancellationToken cancellationToken)
        {
            var cuocTroChuyen =
                await TruyVanTheoChuSoHuu(
                        phienKhachId,
                        khachHangId)
                    .FirstOrDefaultAsync(
                        x =>
                            x.CuocTroChuyenAIId ==
                            cuocTroChuyenId,
                        cancellationToken);

            if (cuocTroChuyen == null)
            {
                return false;
            }

            cuocTroChuyen.DaXoa = true;
            cuocTroChuyen.NgayCapNhat =
                DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        private IQueryable<CuocTroChuyenAI>
            TruyVanTheoChuSoHuu(
                Guid phienKhachId,
                Guid? khachHangId)
        {
            var query =
                _dbContext.CuocTroChuyenAIs
                    .Where(x => !x.DaXoa);

            if (khachHangId.HasValue)
            {
                return query.Where(x =>
                    x.KhachHangId ==
                        khachHangId.Value ||
                    (
                        x.KhachHangId == null &&
                        x.PhienKhachId ==
                            phienKhachId
                    ));
            }

            return query.Where(x =>
                x.KhachHangId == null &&
                x.PhienKhachId == phienKhachId);
        }

        private Task<CuocTroChuyenAI?>
            TimCuocTroChuyenAsync(
                Guid cuocTroChuyenId,
                Guid phienKhachId,
                Guid? khachHangId,
                CancellationToken cancellationToken)
        {
            return TruyVanTheoChuSoHuu(
                    phienKhachId,
                    khachHangId)
                .FirstOrDefaultAsync(
                    x =>
                        x.CuocTroChuyenAIId ==
                        cuocTroChuyenId,
                    cancellationToken);
        }

        private async Task<List<ChatProductDTO>>
            TimSanPhamAsync(
                string cauHoi,
                CancellationToken cancellationToken)
        {
            var cauHoiChuan = ChuanHoa(cauHoi);
            var tuKhoa = TachTuKhoa(cauHoiChuan);
            var giaToiDa = TimGiaToiDa(cauHoi);
            var kichCo = TimKichCo(cauHoiChuan);

            var laCauHoiGoiY =
                CumTuGoiY.Any(cauHoiChuan.Contains);

            var danhSachGiay =
                await _dbContext.Giays
                    .AsNoTracking()
                    .Where(x =>
                        x.TrangThai &&
                        x.GiayChiTiets.Any(ct =>
                            ct.TrangThai &&
                            ct.SoLuongCon > 0))
                    .Include(x => x.ThuongHieu)
                    .Include(x => x.TheLoaiGiay)
                    .Include(x => x.KieuDang)
                    .Include(x => x.ChatLieu)
                    .Include(x => x.DeGiay)
                    .Include(x => x.GiayChiTiets)
                        .ThenInclude(x => x.KichCo)
                    .Include(x => x.GiayChiTiets)
                        .ThenInclude(x => x.MauSac)
                    .Include(x => x.GiayChiTiets)
                        .ThenInclude(x => x.Anhs)
                    .AsSplitQuery()
                    .OrderByDescending(x => x.NgayTao)
                    .Take(250)
                    .ToListAsync(cancellationToken);

            var ketQua =
                new List<(
                    ChatProductDTO SanPham,
                    int Diem,
                    DateTime NgayTao)>();

            foreach (var giay in danhSachGiay)
            {
                var bienThes = giay.GiayChiTiets
                    .Where(x =>
                        x.TrangThai &&
                        x.SoLuongCon > 0)
                    .OrderBy(x => x.Gia)
                    .ToList();

                if (bienThes.Count == 0)
                {
                    continue;
                }

                var sanPham =
                    TaoSanPhamDTO(giay, bienThes);

                if (giaToiDa.HasValue &&
                    sanPham.Gia > giaToiDa.Value)
                {
                    continue;
                }

                if (kichCo.HasValue &&
                    !sanPham.KichCos.Contains(
                        kichCo.Value))
                {
                    continue;
                }

                var diem = TinhDiemPhuHop(
                    sanPham,
                    cauHoiChuan,
                    tuKhoa);

                if (diem == 0 &&
                    tuKhoa.Count > 0 &&
                    !laCauHoiGoiY &&
                    !giaToiDa.HasValue &&
                    !kichCo.HasValue)
                {
                    continue;
                }

                ketQua.Add((
                    sanPham,
                    diem,
                    giay.NgayTao));
            }

            return ketQua
                .OrderByDescending(x => x.Diem)
                .ThenByDescending(x => x.NgayTao)
                .Take(SoSanPhamGoiY)
                .Select(x => x.SanPham)
                .ToList();
        }

        private static ChatProductDTO TaoSanPhamDTO(
            Giay giay,
            List<GiayChiTiet> bienThes)
        {
            var giaThapNhat =
                bienThes.Min(x => x.Gia);

            var giaCaoNhat =
                bienThes.Max(x => x.Gia);

            var anh = bienThes
                .SelectMany(x => x.Anhs)
                .FirstOrDefault(x => x.TrangThai)?
                .DuongDan;

            var moTa = bienThes
                .Select(x => x.MoTa?.Trim())
                .FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x));

            return new ChatProductDTO
            {
                GiayId = giay.GiayId,
                TenGiay = giay.TenGiay,

                ThuongHieu =
                    giay.ThuongHieu?.TenThuongHieu,

                Gia = giaThapNhat,
                GiaCaoNhat = giaCaoNhat,

                SoLuongCon = bienThes.Sum(x =>
                    Math.Max(0, x.SoLuongCon)),

                AnhGiay = anh,

                TheLoai =
                    giay.TheLoaiGiay?.TenTheLoai,

                KieuDang =
                    giay.KieuDang?.TenKieuDang,

                ChatLieu =
                    giay.ChatLieu?.TenChatLieu,

                DeGiay =
                    giay.DeGiay?.TenDeGiay,

                MoTa = moTa,

                KichCos = bienThes
                    .Where(x => x.KichCo != null)
                    .Select(x => x.KichCo!.size)
                    .Where(x => x > 0)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList(),

                MauSacs = bienThes
                    .Where(x => x.MauSac != null)
                    .Select(x => x.MauSac!.TenMau)
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList(),

                DuongDan =
                    $"/detailShoeCategory/{giay.GiayId}"
            };
        }

        private static int TinhDiemPhuHop(
            ChatProductDTO sanPham,
            string cauHoiChuan,
            List<string> tuKhoa)
        {
            var ten =
                ChuanHoa(sanPham.TenGiay);

            var thuongHieu =
                ChuanHoa(sanPham.ThuongHieu);

            var nhom = ChuanHoa(
                string.Join(
                    " ",
                    sanPham.TheLoai,
                    sanPham.KieuDang));

            var thongTin = ChuanHoa(
                string.Join(
                    " ",
                    sanPham.ChatLieu,
                    sanPham.DeGiay,
                    sanPham.MoTa,
                    string.Join(
                        " ",
                        sanPham.MauSacs),
                    "size",
                    string.Join(
                        " ",
                        sanPham.KichCos)));

            var diem = 0;

            if (ten.Length >= 3 &&
                cauHoiChuan.Contains(ten))
            {
                diem += 40;
            }

            if (thuongHieu.Length >= 2 &&
                cauHoiChuan.Contains(thuongHieu))
            {
                diem += 24;
            }

            foreach (var tu in tuKhoa)
            {
                if (ten.Contains(tu))
                {
                    diem += 12;
                }

                if (thuongHieu.Contains(tu))
                {
                    diem += 9;
                }

                if (nhom.Contains(tu))
                {
                    diem += 7;
                }

                if (thongTin.Contains(tu))
                {
                    diem += 3;
                }
            }

            return diem;
        }

        private async Task<string> GoiGeminiAsync(
            List<TinNhanAI> lichSu,
            List<ChatProductDTO> sanPhams,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(
                _geminiOptions.ApiKey))
            {
                throw new InvalidOperationException(
                    "Chưa cấu hình Gemini API key cho trợ lý AI.");
            }

            var duLieuSanPham =
                sanPhams.Count == 0
                    ? "Không tìm thấy sản phẩm phù hợp trong dữ liệu hiện tại."
                    : JsonSerializer.Serialize(
                        sanPhams.Select(x => new
                        {
                            x.TenGiay,
                            x.ThuongHieu,
                            GiaTu = x.Gia,
                            GiaDen = x.GiaCaoNhat,
                            x.SoLuongCon,
                            x.TheLoai,
                            x.KieuDang,
                            x.ChatLieu,
                            x.DeGiay,
                            x.MoTa,
                            x.KichCos,
                            x.MauSacs
                        }),
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy =
                                JsonNamingPolicy.CamelCase
                        });

            var instructions =
                "Bạn là trợ lý tư vấn bán giày của FSHOES. " +
                "Luôn trả lời bằng tiếng Việt, thân thiện, rõ ràng và ngắn gọn. " +
                "Chỉ khẳng định tên sản phẩm, thương hiệu, giá, màu, size và " +
                "tồn kho khi thông tin đó có trong dữ liệu cửa hàng được cung cấp. " +
                "Không tự tạo giá, tồn kho, chương trình giảm giá hoặc chính sách. " +
                "Có thể hướng dẫn kiến thức chọn giày và chọn size ở mức tổng quát, " +
                "nhưng phải nói rõ khách nên đo chân hoặc xem chi tiết sản phẩm. " +
                "Nếu không đủ dữ liệu, hãy nói chưa tìm thấy và đề nghị khách " +
                "cung cấp thêm mục đích sử dụng, size, màu hoặc ngân sách. " +
                "Không hiển thị ID, JSON hay đường dẫn kỹ thuật. " +
                "Không dùng bảng Markdown; ưu tiên đoạn ngắn hoặc danh sách gạch đầu dòng. " +
                "Dữ liệu sản phẩm liên quan của cửa hàng: " +
                duLieuSanPham;

            var messages = new List<object>
    {
        new
        {
            role = "system",
            content = instructions
        }
    };

            messages.AddRange(
                lichSu
                    .Where(x =>
                        x.VaiTro == "user" ||
                        x.VaiTro == "assistant")
                    .Select(x => (object)new
                    {
                        role = x.VaiTro,
                        content = x.NoiDung
                    }));

            var httpClient =
                _httpClientFactory.CreateClient(
                    "Gemini");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _geminiOptions.ApiKey.Trim());

            var requestBody = new
            {
                model = LayModelDangDung(),
                messages
            };

            try
            {
                using var response =
                    await httpClient.PostAsJsonAsync(
                        "chat/completions",
                        requestBody,
                        cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody =
                        await response.Content.ReadAsStringAsync(
                            cancellationToken);

                    _logger.LogWarning(
                        "Gemini API trả về HTTP {StatusCode}. Nội dung: {ErrorBody}",
                        (int)response.StatusCode,
                        errorBody);

                    throw new InvalidOperationException(
                        TaoThongBaoLoiGemini(
                            response.StatusCode));
                }

                var responseJson =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                using var document =
                    JsonDocument.Parse(responseJson);

                var noiDung =
                    DocNoiDungTraLoiGemini(
                        document.RootElement);

                if (string.IsNullOrWhiteSpace(noiDung))
                {
                    throw new InvalidOperationException(
                        "Gemini chưa trả về nội dung. Vui lòng thử lại.");
                }

                return noiDung;
            }
            catch (TaskCanceledException ex)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    ex,
                    "Gemini API phản hồi quá thời gian.");

                throw new InvalidOperationException(
                    "Trợ lý Gemini phản hồi quá lâu. Vui lòng thử lại.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Không thể kết nối Gemini API.");

                throw new InvalidOperationException(
                    "Không thể kết nối Gemini. " +
                    "Hãy kiểm tra kết nối Internet.");
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Phản hồi Gemini không đúng định dạng JSON.");

                throw new InvalidOperationException(
                    "Gemini trả về dữ liệu không hợp lệ. Vui lòng thử lại.");
            }
        }

        private string LayModelDangDung()
        {
            return string.IsNullOrWhiteSpace(
                _geminiOptions.Model)
                ? "gemini-3.5-flash-lite"
                : _geminiOptions.Model.Trim();
        }

        private static string TaoThongBaoLoiGemini(
            HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.Unauthorized or
                HttpStatusCode.Forbidden =>
                    "Gemini API key không hợp lệ hoặc không có quyền sử dụng.",

                HttpStatusCode.TooManyRequests =>
                    "Gemini đã vượt giới hạn miễn phí hoặc đang nhận quá nhiều yêu cầu. " +
                    "Vui lòng đợi một lúc rồi thử lại.",

                HttpStatusCode.NotFound =>
                    "Không tìm thấy model Gemini đã cấu hình.",

                HttpStatusCode.BadRequest =>
                    "Yêu cầu gửi đến Gemini chưa hợp lệ. " +
                    "Vui lòng kiểm tra model trong appsettings.json.",

                _ =>
                    "Dịch vụ Gemini đang tạm thời không sẵn sàng. " +
                    "Vui lòng thử lại sau."
            };
        }

        private static string? DocNoiDungTraLoiGemini(
            JsonElement root)
        {
            if (!root.TryGetProperty(
                    "choices",
                    out var choices) ||
                choices.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var choice in choices.EnumerateArray())
            {
                if (!choice.TryGetProperty(
                        "message",
                        out var message) ||
                    message.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                if (!message.TryGetProperty(
                        "content",
                        out var content) ||
                    content.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var text = content.GetString();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text.Trim();
                }
            }

            return null;
        }

        private static List<string> TachTuKhoa(
            string cauHoiChuan)
        {
            return cauHoiChuan
                .Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries)
                .Where(x =>
                    x.Length >= 2 &&
                    !TuDung.Contains(x) &&
                    !x.All(char.IsDigit))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .Take(12)
                .ToList();
        }

        private static float? TimGiaToiDa(
            string cauHoi)
        {
            var khongDau =
                BoDauTiengViet(cauHoi)
                    .ToLowerInvariant();

            var coYeuCauGia =
                khongDau.Contains("duoi") ||
                khongDau.Contains("toi da") ||
                khongDau.Contains("khong qua") ||
                khongDau.Contains("tam gia") ||
                khongDau.Contains("ngan sach") ||
                khongDau.Contains("gia");

            if (!coYeuCauGia)
            {
                return null;
            }

            var matches = Regex.Matches(
                khongDau,
                @"(?<so>\d+(?:[\.,]\d+)?)\s*(?<donvi>trieu|tr|k|nghin|ngan|dong|vnd)?",
                RegexOptions.IgnoreCase);

            var mucGias =
                new List<float>();

            foreach (Match match in matches)
            {
                var soText =
                    match.Groups["so"]
                        .Value
                        .Replace(',', '.');

                if (!float.TryParse(
                        soText,
                        NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture,
                        out var so))
                {
                    continue;
                }

                var donVi =
                    match.Groups["donvi"]
                        .Value
                        .ToLowerInvariant();

                var gia = donVi switch
                {
                    "trieu" or "tr" =>
                        so * 1_000_000,

                    "k" or "nghin" or "ngan" =>
                        so * 1_000,

                    "dong" or "vnd" =>
                        so,

                    _ when so >= 50_000 =>
                        so,

                    _ => 0
                };

                if (gia > 0)
                {
                    mucGias.Add(gia);
                }
            }

            return mucGias.Count == 0
                ? null
                : mucGias.Max();
        }

        private static int? TimKichCo(
            string cauHoiChuan)
        {
            var match = Regex.Match(
                cauHoiChuan,
                @"(?:size|kich co|co)\s*(?<size>\d{2})");

            if (match.Success &&
                int.TryParse(
                    match.Groups["size"].Value,
                    out var size) &&
                size is >= 20 and <= 55)
            {
                return size;
            }

            return null;
        }

        private static string ChuanHoa(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var khongDau =
                BoDauTiengViet(value)
                    .ToLowerInvariant();

            var builder =
                new StringBuilder(
                    khongDau.Length);

            foreach (var character in khongDau)
            {
                builder.Append(
                    char.IsLetterOrDigit(character)
                        ? character
                        : ' ');
            }

            return Regex.Replace(
                    builder.ToString(),
                    @"\s+",
                    " ")
                .Trim();
        }

        private static string BoDauTiengViet(
            string value)
        {
            var normalized =
                value.Normalize(
                    NormalizationForm.FormD);

            var builder =
                new StringBuilder(
                    normalized.Length);

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo
                    .GetUnicodeCategory(character) !=
                    UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(
                        character is 'đ' or 'Đ'
                            ? 'd'
                            : character);
                }
            }

            return builder
                .ToString()
                .Normalize(
                    NormalizationForm.FormC);
        }

        private static ChatMessageDTO
            ChuyenTinNhanSangDTO(
                TinNhanAI tinNhan)
        {
            return new ChatMessageDTO
            {
                TinNhanId =
                    tinNhan.TinNhanAIId,

                VaiTro =
                    tinNhan.VaiTro,

                NoiDung =
                    tinNhan.NoiDung,

                ThoiGian =
                    tinNhan.ThoiGian
            };
        }

        private static string TaoTieuDe(
            string noiDung)
        {
            var tieuDe = Regex.Replace(
                    noiDung
                        .Replace("\r", " ")
                        .Replace("\n", " "),
                    @"\s+",
                    " ")
                .Trim();

            return tieuDe.Length <= 80
                ? tieuDe
                : tieuDe[..77] + "...";
        }
    }
}