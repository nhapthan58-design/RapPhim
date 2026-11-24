using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    [Collection("Sequential")]
    public class CopyDayTests : IDisposable
    {
        private static readonly DateTime FakeNow = new(2030, 1, 15, 8, 0, 0);
        private static readonly DateTime DayA = new(2030, 1, 16);
        private static readonly DateTime DayB = new(2030, 1, 17);

        private readonly ILichChieuRepository _prevRepo;
        private readonly Func<DateTime> _prevNow;

        public CopyDayTests()
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

        private void SeedThreeOnDayA()
        {
            Assert.True(LichChieuService.Them(1, 1, DayA.AddHours(9)).ThanhCong);
            Assert.True(LichChieuService.Them(2, 1, DayA.AddHours(13)).ThanhCong);
            Assert.True(LichChieuService.Them(3, 2, DayA.AddHours(10)).ThanhCong);
        }

        [Fact]
        public void Copy3ToEmptyDay_CreatesAll3()
        {
            SeedThreeOnDayA();

            var kq = LichChieuService.SaoChepLichNgay(DayA, DayB);
            Assert.Equal(3, kq.SoTaoMoi);
            Assert.Empty(kq.BoQua);
            Assert.Equal(3, DataManager.DanhSachLichChieu.Count(l => l.GioBatDau.Date == DayB));
        }

        [Fact]
        public void CopyWithOneConflict_Creates2Skips1()
        {
            SeedThreeOnDayA();
            // Occupy Room 1 around 09:30 on DayB so the 09:00 showtime conflicts
            Assert.True(LichChieuService.Them(4, 1, DayB.AddHours(9).AddMinutes(30)).ThanhCong);

            var kq = LichChieuService.SaoChepLichNgay(DayA, DayB);
            Assert.Equal(2, kq.SoTaoMoi);
            Assert.Single(kq.BoQua);
            Assert.Contains("Conflicts", kq.BoQua[0]);
        }

        [Fact]
        public void CopyToPastDate_Creates0()
        {
            SeedThreeOnDayA();

            var kq = LichChieuService.SaoChepLichNgay(DayA, FakeNow.Date.AddDays(-1));
            Assert.Equal(0, kq.SoTaoMoi);
            Assert.Equal(3, kq.BoQua.Count);
        }

        [Fact]
        public void CopySameDate_ReturnsError()
        {
            SeedThreeOnDayA();

            var kq = LichChieuService.SaoChepLichNgay(DayA, DayA);
            Assert.Equal(0, kq.SoTaoMoi);
            Assert.Single(kq.BoQua);
        }

        [Fact]
        public void CopyWithRoomFilter_CopiesOnlySelectedRoom()
        {
            SeedThreeOnDayA();

            var kq = LichChieuService.SaoChepLichNgay(DayA, DayB, maPhong: 2);
            Assert.Equal(1, kq.SoTaoMoi);
            Assert.Empty(kq.BoQua);
            var copied = Assert.Single(DataManager.DanhSachLichChieu, l => l.GioBatDau.Date == DayB);
            Assert.Equal(2, copied.MaPhong);
        }

        [Fact]
        public void CopySourceWithSoldTickets_NewShowtimeHasZeroTickets()
        {
            SeedThreeOnDayA();
            var first = DataManager.DanhSachLichChieu[0];
            Assert.True(LichChieuService.GhiNhanBanVe(first.MaLichChieu, 20).ThanhCong);

            var kq = LichChieuService.SaoChepLichNgay(DayA, DayB, maPhong: 1);
            Assert.Equal(2, kq.SoTaoMoi);
            Assert.All(DataManager.DanhSachLichChieu.Where(l => l.GioBatDau.Date == DayB),
                l => Assert.Equal(0, l.SoVeDaBan));
        }

        [Fact]
        public void FormSaoChepLich_SmokeOpensAndCopies()
        {
            SeedThreeOnDayA();
            Exception? err = null;
            string resultText = "";
            var t = new Thread(() =>
            {
                try
                {
                    using var dlg = new FormSaoChepLich(DayA);
                    dlg.Show();
                    var btnCopy = (Button)dlg.GetType().GetField("btnSaoChep",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(dlg)!;
                    var txtRes = (TextBox)dlg.GetType().GetField("txtKetQua",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(dlg)!;
                    btnCopy.PerformClick();
                    resultText = txtRes.Text;
                    dlg.Close();
                }
                catch (Exception ex) { err = ex; }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            Assert.Null(err);
            Assert.Contains("Created 3", resultText);
        }
    }
}
