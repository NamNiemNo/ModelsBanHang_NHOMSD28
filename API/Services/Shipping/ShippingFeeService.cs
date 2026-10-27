using API.Models.DTO.BanHang;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace API.Services.Shipping
{
    public sealed class ShippingFeeOptions
    {
        public const string SectionName = "ShippingFees";

        public decimal FreeShippingThreshold { get; set; } = 2_000_000m;

        public decimal HaNoiFee { get; set; } = 20_000m;

        public decimal NorthernFee { get; set; } = 28_000m;

        public decimal CentralFee { get; set; } = 35_000m;

        public decimal SouthernFee { get; set; } = 45_000m;

        public decimal DefaultFee { get; set; } = 40_000m;

        public int FreeShippingDeliveryDays { get; set; } = 2;

        public int HaNoiDeliveryDays { get; set; } = 1;

        public int NorthernDeliveryDays { get; set; } = 2;

        public int CentralDeliveryDays { get; set; } = 3;

        public int SouthernDeliveryDays { get; set; } = 4;

        public int DefaultDeliveryDays { get; set; } = 4;
    }

    public interface IShippingFeeService
    {
        DatHang.TinhPhiShipResponse Calculate(
            string tinhThanhPho,
            decimal tamTinh);
    }

    public sealed class ShippingFeeService : IShippingFeeService
    {
        private static readonly string[] NorthernProvinces =
        {
            "hai phong",
            "bac ninh",
            "ha nam",
            "hai duong",
            "hung yen",
            "nam dinh",
            "ninh binh",
            "thai binh",
            "vinh phuc",
            "lao cai",
            "yen bai",
            "dien bien",
            "hoa binh",
            "lai chau",
            "son la",
            "ha giang",
            "cao bang",
            "bac kan",
            "lang son",
            "tuyen quang",
            "thai nguyen",
            "phu tho",
            "bac giang",
            "quang ninh"
        };

        private static readonly string[] CentralProvinces =
        {
            "thanh hoa",
            "nghe an",
            "ha tinh",
            "quang binh",
            "quang tri",
            "thua thien hue",
            "hue",
            "da nang",
            "quang nam",
            "quang ngai",
            "binh dinh",
            "phu yen",
            "khanh hoa",
            "ninh thuan",
            "binh thuan",
            "kon tum",
            "gia lai",
            "dak lak",
            "dac lak",
            "dak nong",
            "dac nong",
            "lam dong"
        };

        private static readonly string[] SouthernProvinces =
        {
            "ho chi minh",
            "ba ria vung tau",
            "binh duong",
            "binh phuoc",
            "dong nai",
            "tay ninh",
            "an giang",
            "bac lieu",
            "ben tre",
            "ca mau",
            "can tho",
            "dong thap",
            "hau giang",
            "kien giang",
            "long an",
            "soc trang",
            "tien giang",
            "tra vinh",
            "vinh long"
        };

        private readonly ShippingFeeOptions _options;

        public ShippingFeeService(
            IOptions<ShippingFeeOptions> options)
        {
            _options = options.Value;
        }

        public DatHang.TinhPhiShipResponse Calculate(
            string tinhThanhPho,
            decimal tamTinh)
        {
            if (tamTinh < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tamTinh),
                    "Tạm tính không được nhỏ hơn 0.");
            }

            if (string.IsNullOrWhiteSpace(tinhThanhPho))
            {
                throw new ArgumentException(
                    "Tỉnh/thành phố không được để trống.",
                    nameof(tinhThanhPho));
            }

            string normalizedProvince =
                NormalizeLocation(tinhThanhPho);

            decimal fee = _options.DefaultFee;

            int deliveryDays =
                _options.DefaultDeliveryDays;

            // Miễn phí vận chuyển khi đơn đạt mức quy định.
            if (tamTinh >= _options.FreeShippingThreshold)
            {
                fee = 0m;

                deliveryDays =
                    _options.FreeShippingDeliveryDays;
            }
            else if (normalizedProvince.Contains("ha noi"))
            {
                fee = _options.HaNoiFee;

                deliveryDays =
                    _options.HaNoiDeliveryDays;
            }
            else if (MatchesRegion(
                normalizedProvince,
                NorthernProvinces))
            {
                fee = _options.NorthernFee;

                deliveryDays =
                    _options.NorthernDeliveryDays;
            }
            else if (MatchesRegion(
                normalizedProvince,
                CentralProvinces))
            {
                fee = _options.CentralFee;

                deliveryDays =
                    _options.CentralDeliveryDays;
            }
            else if (MatchesRegion(
                normalizedProvince,
                SouthernProvinces))
            {
                fee = _options.SouthernFee;

                deliveryDays =
                    _options.SouthernDeliveryDays;
            }

            return new DatHang.TinhPhiShipResponse
            {
                PhiShip = fee,

                MienPhiTu =
                    _options.FreeShippingThreshold,

                ConThieuDeMienPhi = Math.Max(
                    0m,
                    _options.FreeShippingThreshold - tamTinh),

                SoNgayGiaoDuKien = deliveryDays,

                NgayGiaoDuKien =
                    DateTime.Today.AddDays(deliveryDays)
            };
        }

        private static bool MatchesRegion(
            string normalizedProvince,
            IEnumerable<string> provinces)
        {
            return provinces.Any(
                province => normalizedProvince.Contains(
                    province,
                    StringComparison.Ordinal));
        }

        private static string NormalizeLocation(string value)
        {
            string normalized = value
                .Trim()
                .ToLowerInvariant()
                .Replace('đ', 'd')
                .Normalize(NormalizationForm.FormD);

            IEnumerable<char> characters =
                normalized.Where(character =>
                    CharUnicodeInfo.GetUnicodeCategory(character)
                    != UnicodeCategory.NonSpacingMark);

            string withoutDiacritics =
                new string(characters.ToArray())
                    .Normalize(NormalizationForm.FormC);

            IEnumerable<char> cleanedCharacters =
                withoutDiacritics.Select(character =>
                    char.IsLetterOrDigit(character)
                        ? character
                        : ' ');

            return string.Join(
                ' ',
                new string(cleanedCharacters.ToArray())
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries));
        }
    }
}