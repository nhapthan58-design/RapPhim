using RapPhim.Data;
using RapPhim.Services;

namespace RapPhim.Tests
{
    [Collection("Sequential")]
    public class OperatingHoursTests : IDisposable
    {
        private static readonly DateTime FakeNow = new(2030, 1, 15, 5, 0, 0);
        private static readonly DateTime Day = new(2030, 1, 15);

        private readonly ILichChieuRepository _prevRepo;
        private readonly Func<DateTime> _prevNow;

        public OperatingHoursTests()
        {
            _prevRepo = DataManager.Repository;
            _prevNow = LichChieuService.Now;
            DataManager.DungRepository(new InMemoryLichChieuRepository());
            LichChieuService.Now = () => FakeNow;
        }

        public void Dispose()
        {
            LichChieuService.Now = _prevNow;
            DataManager.DungRepository(_prevRepo);
        }

        [Fact]
        public void BeforeOpening_0759_Rejected()
        {
            var kq = LichChieuService.Them(1, 1, Day.AddHours(7).AddMinutes(59));
            Assert.False(kq.ThanhCong);
            Assert.Contains("08:00", kq.ThongBao);
        }

        [Fact]
        public void ExactlyOpening_0800_Allowed()
        {
            var kq = LichChieuService.Them(1, 1, Day.AddHours(8));
            Assert.True(kq.ThanhCong, kq.ThongBao);
        }

        [Fact]
        public void LatestStart_2330_Allowed()
        {
            var kq = LichChieuService.Them(4, 1, Day.AddHours(23).AddMinutes(30));
            Assert.True(kq.ThanhCong, kq.ThongBao);
        }

        [Fact]
        public void AfterLatestStart_2331_Rejected()
        {
            var kq = LichChieuService.Them(4, 1, Day.AddHours(23).AddMinutes(31));
            Assert.False(kq.ThanhCong);
            Assert.Contains("23:30", kq.ThongBao);
        }

        [Fact]
        public void LongMovieStarting2300_EndingPastMidnight_Allowed()
        {
            // Movie 1 = Avengers: Endgame (181 min) -> 23:00 to 02:01 next day
            var kq = LichChieuService.Them(1, 1, Day.AddHours(23));
            Assert.True(kq.ThanhCong, kq.ThongBao);
            var stored = Assert.Single(DataManager.DanhSachLichChieu);
            Assert.Equal(Day.AddDays(1), stored.GioKetThuc.Date);
        }

        [Fact]
        public void SuggestNextFreeSlot_From0600_ReturnsAtLeast0800()
        {
            var suggested = LichChieuService.GoiYGioTrong(1, 1, Day.AddHours(6));
            Assert.NotNull(suggested);
            Assert.True(suggested!.Value.TimeOfDay >= TimeSpan.FromHours(8),
                $"Expected >= 08:00, got {suggested:HH:mm}");
        }
    }
}
