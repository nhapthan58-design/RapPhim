using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    /// <summary>Pure-logic tests (no storage, no clock).</summary>
    public class ConflictDetectionTests
    {
        private static readonly DateTime Day = new(2030, 1, 15);

        private static DateTime T(int h, int m = 0) => Day.AddHours(h).AddMinutes(m);

        private static LichChieu L(int id, int phong, DateTime bd, DateTime kt, int ve = 0) => new()
        {
            MaLichChieu = id, MaPhong = phong, GioBatDau = bd, GioKetThuc = kt, SoVeDaBan = ve, TenPhim = "X"
        };

        private static List<LichChieu> Existing() => new() { L(5, 1, T(10), T(12)) };

        [Fact] // T1
        public void DifferentRoom_NoConflict() =>
            Assert.Null(LichChieuService.TimXungDot(Existing(), 2, T(10), T(12)));

        [Fact] // T2
        public void PartialOverlap_Conflict() =>
            Assert.NotNull(LichChieuService.TimXungDot(Existing(), 1, T(11), T(13)));

        [Fact] // T3
        public void OverlapAtStart_Conflict() =>
            Assert.NotNull(LichChieuService.TimXungDot(Existing(), 1, T(8), T(10, 30)));

        [Fact] // T4
        public void InsideExisting_Conflict() =>
            Assert.NotNull(LichChieuService.TimXungDot(Existing(), 1, T(10, 30), T(11, 30)));

        [Fact] // T5
        public void ContainsExisting_Conflict() =>
            Assert.NotNull(LichChieuService.TimXungDot(Existing(), 1, T(9), T(13)));

        [Fact] // T6
        public void StartsExactlyAfterBuffer_NoConflict() =>
            Assert.Null(LichChieuService.TimXungDot(Existing(), 1, T(12, 15), T(14)));

        [Fact] // T7
        public void StartsInsideBuffer_Conflict() =>
            Assert.NotNull(LichChieuService.TimXungDot(Existing(), 1, T(12, 14), T(14)));

        [Fact] // T8
        public void EndsExactlyBeforeBuffer_NoConflict() =>
            Assert.Null(LichChieuService.TimXungDot(Existing(), 1, T(7), T(9, 45)));

        [Fact] // T9
        public void OvernightShowtime_ConflictNextDay()
        {
            var ds = new List<LichChieu> { L(1, 1, T(23), T(25, 30)) };   // 23:00 -> 01:30 next day
            Assert.NotNull(LichChieuService.TimXungDot(ds, 1, T(25), T(26, 30)));
        }

        [Fact] // T10
        public void EditingSameShowtime_IgnoresItself() =>
            Assert.Null(LichChieuService.TimXungDot(Existing(), 1, T(10, 30), T(12, 30), boQuaMa: 5));

        [Fact]
        public void ReturnsTheConflictingShowtime()
        {
            var found = LichChieuService.TimXungDot(Existing(), 1, T(11), T(13));
            Assert.Equal(5, found!.MaLichChieu);
        }
    }

    public class EditLockTests
    {
        private static readonly DateTime Now = new(2030, 1, 15, 12, 0, 0);

        private static LichChieu L(DateTime bd, int ve) => new()
        {
            MaLichChieu = 1, GioBatDau = bd, GioKetThuc = bd.AddHours(2), SoVeDaBan = ve
        };

        [Fact]
        public void FutureAndNoTickets_CanEdit()
        {
            var l = L(Now.AddHours(3), 0);
            Assert.True(LichChieuService.CoTheSuaXoa(l, Now, l.SoVeDaBan, out var lyDo));
            Assert.Equal(string.Empty, lyDo);
        }

        [Fact]
        public void HasTickets_Locked()
        {
            var l = L(Now.AddHours(3), 1);
            Assert.False(LichChieuService.CoTheSuaXoa(l, Now, l.SoVeDaBan, out var lyDo));
            Assert.Contains("sold", lyDo, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void AlreadyStarted_Locked()
        {
            var l = L(Now.AddMinutes(-1), 0);
            Assert.False(LichChieuService.CoTheSuaXoa(l, Now, l.SoVeDaBan, out _));
        }

        [Fact]
        public void StartsExactlyNow_Locked()
        {
            var l = L(Now, 0);
            Assert.False(LichChieuService.CoTheSuaXoa(l, Now, l.SoVeDaBan, out _));
        }
    }

    public class TicketRuleTests
    {
        private static LichChieu L(int ve) => new() { MaLichChieu = 1, SoVeDaBan = ve };

        [Fact]
        public void Sell_WithinCapacity_Succeeds()
        {
            var l = L(10);
            var kq = LichChieuService.ApDungBanVe(l, soGhe: 40, soVe: 5);
            Assert.True(kq.ThanhCong);
            Assert.Equal(15, l.SoVeDaBan);
        }

        [Fact]
        public void Sell_BeyondCapacity_Fails_AndKeepsCount()
        {
            var l = L(38);
            var kq = LichChieuService.ApDungBanVe(l, soGhe: 40, soVe: 3);
            Assert.False(kq.ThanhCong);
            Assert.Equal(38, l.SoVeDaBan);
        }

        [Fact]
        public void Sell_ExactlyFull_Succeeds()
        {
            var l = L(38);
            Assert.True(LichChieuService.ApDungBanVe(l, 40, 2).ThanhCong);
            Assert.Equal(40, l.SoVeDaBan);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Sell_NonPositive_Fails(int soVe) =>
            Assert.False(LichChieuService.ApDungBanVe(L(0), 40, soVe).ThanhCong);

        [Fact]
        public void Cancel_MoreThanSold_Fails_NeverNegative()
        {
            var l = L(2);
            Assert.False(LichChieuService.ApDungHuyVe(l, 3).ThanhCong);
            Assert.Equal(2, l.SoVeDaBan);
        }

        [Fact]
        public void Cancel_Valid_Succeeds()
        {
            var l = L(5);
            Assert.True(LichChieuService.ApDungHuyVe(l, 5).ThanhCong);
            Assert.Equal(0, l.SoVeDaBan);
        }
    }
}
