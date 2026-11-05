using System;
using System.Text.RegularExpressions;

namespace WEB_TinhNguyen.Tests.E2E.Framework
{
    /// <summary>
    /// Explicit authoritative reference oracles derived from PROJECT.md and ORIGINAL_REQUEST.md.
    /// Used as the ground truth for validating system behavior, calculations, and boundaries.
    /// </summary>
    public static class ReferenceOracles
    {
        public const string BankBinVietcombank = "970436"; // VCB standard or BIDV / VietinBank BIN
        public const string DefaultAccountNumber = "1023456789";
        public const string AreaName = "CuaHang";
        public const string SessionKeyCart = "GioHangGayQuy";

        /// <summary>
        /// Derives expected line total: DonGia * SoLuong
        /// </summary>
        public static decimal LineTotal(decimal unitPrice, int quantity)
        {
            if (quantity <= 0) return 0m;
            return unitPrice * quantity;
        }

        /// <summary>
        /// Derives expected charity contribution portion: (ThanhTien * PhanTramDongGop) / 100
        /// </summary>
        public static decimal CharityContribution(decimal lineTotal, int contributionPercent)
        {
            if (contributionPercent < 0) return 0m;
            if (contributionPercent > 100) contributionPercent = 100;
            return Math.Round(lineTotal * contributionPercent / 100m, 2);
        }

        /// <summary>
        /// Derives total order payment: TongTienHang + TienUngHoThem + PhiVanChuyen
        /// </summary>
        public static decimal OrderTotal(decimal subtotal, decimal extraDonation, decimal shippingFee = 0m)
        {
            var validExtra = extraDonation > 0 ? extraDonation : 0m;
            var validShipping = shippingFee > 0 ? shippingFee : 0m;
            return subtotal + validExtra + validShipping;
        }

        /// <summary>
        /// Derives VietQR image URL per VietQR specification
        /// </summary>
        public static string VietQrImageUrl(string bank, string accountNo, decimal amount, string orderCode)
        {
            var cleanBank = string.IsNullOrWhiteSpace(bank) ? "VCB" : bank.Trim();
            var cleanAcc = string.IsNullOrWhiteSpace(accountNo) ? DefaultAccountNumber : accountNo.Trim();
            var cleanMemo = Uri.EscapeDataString(orderCode.Trim());
            var cleanAmount = (long)Math.Max(0, amount);
            return $"https://img.vietqr.io/image/{cleanBank}-{cleanAcc}-compact2.png?amount={cleanAmount}&addInfo={cleanMemo}";
        }

        /// <summary>
        /// Authoritative validation for Vietnamese 10-digit mobile phone numbers:
        /// Starting with 03, 05, 07, 08, 09 followed by 8 digits.
        /// </summary>
        public static bool IsValidVietnamesePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var trimmed = phone.Trim().Replace(" ", "").Replace(".", "").Replace("-", "");
            return Regex.IsMatch(trimmed, @"^(03|05|07|08|09)\d{8}$");
        }

        /// <summary>
        /// Standard email validation oracle
        /// </summary>
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>
        /// Expected route pattern for Area CuaHang
        /// </summary>
        public static string ExpectedAreaRoutePattern => "{area:exists}/{controller=CuaHang}/{action=Index}/{id?}";
    }
}
