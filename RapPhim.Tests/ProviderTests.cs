using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    internal class FakeDanhMucProvider : IDanhMucProvider
    {
        private readonly List<Phim> _phim;
        private readonly List<PhongChieu> _phong;

        public FakeDanhMucProvider(List<Phim> phim, List<PhongChieu> phong)
        {
            _phim = phim;
            _phong = phong;
        }

        public IReadOnlyList<Phim> LayDanhSachPhim() => _phim;
        public IReadOnlyList<PhongChieu> LayDanhSachPhong() => _phong;
    }

    internal class FakeVeDaBanProvider : IVeDaBanProvider
    {
        private readonly Func<int, int> _count;
        public FakeVeDaBanProvider(Func<int, int> count) => _count = count;
        public int DemVeDaBan(int maLichChieu) => _count(maLichChieu);
    }

    [Collection("Sequential")]
    public class ProviderTests : IDisposable
    {
        private readonly IDanhMucProvider _prevCatalog;
        private readonly IVeDaBanProvider _prevTickets;
        private readonly ILichChieuRepository _prevRepo;
        private readonly Func<DateTime> _prevNow;

        public ProviderTests()
        {
            _prevCatalog = DataManager.DanhMucProvider;
            _prevTickets = DataManager.VeDaBanProvider;
            _prevRepo = DataManager.Repository;
            _prevNow = LichChieuService.Now;
            DataManager.DungRepository(new InMemoryLichChieuRepository());
        }

        public void Dispose()
        {
            DataManager.DungDanhMucProvider(_prevCatalog);
            DataManager.DungVeDaBanProvider(_prevTickets);
            DataManager.DungRepository(_prevRepo);
            LichChieuService.Now = _prevNow;
        }

        [Fact]
        public void FakeCatalogProvider_PopulatesScheduleFormCombos()
        {
            var fakeCatalog = new FakeDanhMucProvider(
                new List<Phim>
                {
                    new() { MaPhim = 10, TenPhim = "Movie Alpha", ThoiLuong = 100 },
                    new() { MaPhim = 20, TenPhim = "Movie Beta", ThoiLuong = 120 }
                },
                new List<PhongChieu>
                {
                    new() { MaPhong = 99, TenPhong = "Room X", SoGhe = 50 }
                });
            DataManager.DungDanhMucProvider(fakeCatalog);

            Exception? err = null;
            int movieCount = -1, roomCount = -1;
            var t = new Thread(() =>
            {
                try
                {
                    using var f = new FormXepLich();
                    f.Show();
                    var cboPhim = (ComboBox)f.GetType().GetField("cboPhim",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;
                    var cboPhong = (ComboBox)f.GetType().GetField("cboPhong",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;
                    movieCount = cboPhim.Items.Count;
                    roomCount = cboPhong.Items.Count;
                    f.Close();
                }
                catch (Exception ex) { err = ex; }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            Assert.Null(err);
            Assert.Equal(2, movieCount);
            Assert.Equal(1, roomCount);
        }

        [Fact]
        public void FakeSoldTicketProvider_LocksShowtime_EvenWhenSoVeDaBanIsZero()
        {
            var now = new DateTime(2030, 1, 15, 8, 0, 0);
            LichChieuService.Now = () => now;
            DataManager.DungVeDaBanProvider(new FakeVeDaBanProvider(_ => 3));

            var showtime = new LichChieu
            {
                MaLichChieu = 42,
                MaPhim = 1,
                MaPhong = 1,
                GioBatDau = now.AddHours(3),
                GioKetThuc = now.AddHours(5),
                SoVeDaBan = 0
            };

            Assert.False(LichChieuService.CoTheSuaXoa(showtime, now, out string lyDo));
            Assert.Contains("sold", lyDo, StringComparison.OrdinalIgnoreCase);
        }
    }
}
