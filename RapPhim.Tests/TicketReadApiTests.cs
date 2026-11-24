using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    public class TicketReadApiPureTests
    {
        private static readonly DateTime Day = new(2030, 1, 15);
        private static readonly DateTime Now = Day.AddHours(11);

        private static LichChieu L(int id, int phim, DateTime bd, DateTime kt) => new()
        {
            MaLichChieu = id,
            MaPhim = phim,
            MaPhong = 1,
            GioBatDau = bd,
            GioKetThuc = kt,
            TenPhim = "Movie " + phim,
            TenPhong = "Room 1"
        };

        [Fact]
        public void LaySuatSapChieu_FiltersByMovie_ExcludesStarted_AndSortsAscending()
        {
            var ds = new List<LichChieu>
            {
                L(1, 1, Day.AddHours(9), Day.AddHours(11)),           // started (before Now=11:00)
                L(2, 1, Day.AddHours(11), Day.AddHours(13)),          // starts exactly at Now -> excluded
                L(3, 1, Day.AddHours(16), Day.AddHours(18)),          // upcoming
                L(4, 2, Day.AddHours(13), Day.AddHours(15)),          // different movie
                L(5, 1, Day.AddHours(12), Day.AddHours(14)),          // upcoming, earlier than #3
                L(6, 1, Day.AddDays(1).AddHours(10), Day.AddDays(1).AddHours(12)) // next day
            };

            var result = LichChieuService.LaySuatSapChieu(ds, Day, Now, maPhim: 1);
            Assert.Equal(new[] { 5, 3 }, result.Select(x => x.MaLichChieu).ToArray());
        }

        [Fact]
        public void LaySuatSapChieu_OvernightFromPreviousDay_AlreadyStarted_IsExcluded()
        {
            // Started yesterday 23:00, ends today 01:30; even at 00:30 today it has already started.
            var earlyNow = Day.AddMinutes(30);
            var ds = new List<LichChieu>
            {
                L(1, 1, Day.AddHours(-1), Day.AddHours(1).AddMinutes(30)),
                L(2, 1, Day.AddHours(9), Day.AddHours(11))
            };

            var result = LichChieuService.LaySuatSapChieu(ds, Day, earlyNow);
            var only = Assert.Single(result);
            Assert.Equal(2, only.MaLichChieu);
        }
    }

    [Collection("Sequential")]
    public class TicketReadApiIntegrationTests : IDisposable
    {
        private static readonly DateTime FakeNow = new(2030, 1, 15, 8, 0, 0);
        private readonly ILichChieuRepository _prevRepo;
        private readonly Func<DateTime> _prevNow;

        public TicketReadApiIntegrationTests()
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
        public void ReturnedObjects_AreDefensiveCopies_MutatingDoesNotChangeDataManager()
        {
            Assert.True(LichChieuService.Them(1, 1, FakeNow.AddHours(2)).ThanhCong);
            int id = DataManager.DanhSachLichChieu[0].MaLichChieu;

            var fromList = LichChieuService.LaySuatSapChieu(FakeNow.Date)[0];
            fromList.SoVeDaBan = 999;
            fromList.TenPhim = "Hacked";

            var byId = LichChieuService.LayTheoMa(id)!;
            byId.SoVeDaBan = 888;

            Assert.Equal(0, DataManager.DanhSachLichChieu[0].SoVeDaBan);
            Assert.NotEqual("Hacked", DataManager.DanhSachLichChieu[0].TenPhim);
        }

        [Fact]
        public void LayPhimCoSuatTrongNgay_ReturnsDistinctMoviesWithUpcomingShowtimes()
        {
            Assert.True(LichChieuService.Them(1, 1, FakeNow.AddHours(2)).ThanhCong);
            Assert.True(LichChieuService.Them(1, 2, FakeNow.AddHours(3)).ThanhCong);
            Assert.True(LichChieuService.Them(3, 3, FakeNow.AddHours(4)).ThanhCong);

            var movies = LichChieuService.LayPhimCoSuatTrongNgay(FakeNow.Date);
            Assert.Equal(new[] { 1, 3 }, movies.Select(m => m.MaPhim).OrderBy(x => x).ToArray());
        }
    }
}
