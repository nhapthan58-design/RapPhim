using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    [Collection("Sequential")]
    public class ListFormFeatureTests : IDisposable
    {
        private static readonly DateTime FakeNow = new(2030, 1, 15, 14, 0, 0);
        private readonly ILichChieuRepository _prevRepo;
        private readonly Func<DateTime> _prevNow;
        private readonly InMemoryLichChieuRepository _repo = new();

        public ListFormFeatureTests()
        {
            _prevRepo = DataManager.Repository;
            _prevNow = LichChieuService.Now;
            DataManager.DungRepository(_repo);
            LichChieuService.Now = () => FakeNow;

            // 1 Ended (09:00-11:00), 1 Locked (16:00-18:00, 5 tickets), 1 Available (20:00-22:00)
            _repo.Insert(new LichChieu
            {
                MaPhim = 1, MaPhong = 1,
                GioBatDau = FakeNow.Date.AddHours(9), GioKetThuc = FakeNow.Date.AddHours(11),
                SoVeDaBan = 0, TenPhim = "Ended Movie", TenPhong = "Room 1", ThoiLuongPhim = 120
            });
            _repo.Insert(new LichChieu
            {
                MaPhim = 2, MaPhong = 1,
                GioBatDau = FakeNow.Date.AddHours(16), GioKetThuc = FakeNow.Date.AddHours(18),
                SoVeDaBan = 5, TenPhim = "Locked Movie", TenPhong = "Room 1", ThoiLuongPhim = 120
            });
            _repo.Insert(new LichChieu
            {
                MaPhim = 3, MaPhong = 2,
                GioBatDau = FakeNow.Date.AddHours(20), GioKetThuc = FakeNow.Date.AddHours(22),
                SoVeDaBan = 0, TenPhim = "Available Movie", TenPhong = "Room 2", ThoiLuongPhim = 120
            });
        }

        public void Dispose()
        {
            LichChieuService.Now = _prevNow;
            DataManager.DungRepository(_prevRepo);
        }

        private static T Ctl<T>(Form f, string field) where T : class =>
            (T)f.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;

        private static void RunSta(Action action)
        {
            Exception? err = null;
            var t = new Thread(() => { try { action(); } catch (Exception ex) { err = ex; } });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            if (err != null) throw new Exception("STA test failed", err);
        }

        [Fact]
        public void HideEndedCheckbox_HidesEndedRows_WhileFooterShowsFullCounts() => RunSta(() =>
        {
            using var f = new FormDanhSachLich();
            f.Show();

            var chkHide = Ctl<CheckBox>(f, "chkAnDaChieu");
            var grid = Ctl<DataGridView>(f, "dgvDanhSach");
            var lblSummary = Ctl<Label>(f, "lblTongSuat");

            // Default: Hide ended is checked -> 2 rows visible, footer still reports Total: 3 ... Ended: 1
            Assert.True(chkHide.Checked);
            Assert.Equal(2, grid.Rows.Count);
            Assert.Contains("Total: 3", lblSummary.Text);
            Assert.Contains("Available: 1", lblSummary.Text);
            Assert.Contains("Locked: 1", lblSummary.Text);
            Assert.Contains("Ended: 1", lblSummary.Text);

            // Uncheck -> all 3 rows visible
            chkHide.Checked = false;
            Assert.Equal(3, grid.Rows.Count);

            f.Close();
        });

        [Fact]
        public void SortByStartDescending_PutsLatestShowtimeFirst() => RunSta(() =>
        {
            using var f = new FormDanhSachLich();
            f.Show();

            var chkHide = Ctl<CheckBox>(f, "chkAnDaChieu");
            var grid = Ctl<DataGridView>(f, "dgvDanhSach");
            chkHide.Checked = false;

            // Default is Start ascending (09:00 first). Calling SapXepTheoCot("Start") toggles to descending (20:00 first).
            var firstAsc = (LichChieuViewModel)grid.Rows[0].DataBoundItem!;
            Assert.Equal("09:00", firstAsc.Start);

            f.SapXepTheoCot("Start");
            var firstDesc = (LichChieuViewModel)grid.Rows[0].DataBoundItem!;
            Assert.Equal("20:00", firstDesc.Start);

            f.Close();
        });

        [Fact]
        public void SapXepDanhSach_SortsByRealDateTime_NotFormattedString()
        {
            var items = new List<LichChieuViewModel>
            {
                new() { ID = 1, Date = "02/02/2030", Start = "09:00", GioBatDauGoc = new DateTime(2030, 2, 2, 9, 0, 0) },
                new() { ID = 2, Date = "25/01/2030", Start = "20:00", GioBatDauGoc = new DateTime(2030, 1, 25, 20, 0, 0) }
            };

            // String sort on "dd/MM/yyyy" would put "02/02/2030" before "25/01/2030" (wrong).
            var sorted = FormDanhSachLich.SapXepDanhSach(items, "Date", ascending: true);
            Assert.Equal(2, sorted[0].ID);
            Assert.Equal(1, sorted[1].ID);
        }
    }
}
