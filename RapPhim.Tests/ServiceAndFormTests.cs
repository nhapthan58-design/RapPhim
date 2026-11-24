using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Tests
{
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SequentialCollection { }

    /// <summary>Tests that touch shared static state (DataManager, clock). Run one at a time.</summary>
    [Collection("Sequential")]
    public class ServiceIntegrationTests : IDisposable
    {
        private static readonly DateTime FakeNow = new(2030, 1, 15, 8, 0, 0);
        private readonly ILichChieuRepository _prevRepo;
        private readonly InMemoryLichChieuRepository _fakeRepo = new();

        public ServiceIntegrationTests()
        {
            _prevRepo = DataManager.Repository;
            DataManager.DungRepository(_fakeRepo);
            LichChieuService.Now = () => FakeNow;
        }

        public void Dispose()
        {
            LichChieuService.Now = () => DateTime.Now;
            DataManager.DungRepository(_prevRepo);
        }

        [Fact]
        public void Them_InThePast_IsRejected()
        {
            var kq = LichChieuService.Them(1, 1, FakeNow.AddMinutes(-30));
            Assert.False(kq.ThanhCong);
            Assert.Empty(DataManager.DanhSachLichChieu);
        }

        [Fact]
        public void Them_StripsSeconds()
        {
            var kq = LichChieuService.Them(1, 1, FakeNow.AddHours(2).AddSeconds(37).AddMilliseconds(123));
            Assert.True(kq.ThanhCong);
            var l = Assert.Single(DataManager.DanhSachLichChieu);
            Assert.Equal(0, l.GioBatDau.Second);
            Assert.Equal(0, l.GioBatDau.Millisecond);
        }

        [Fact]
        public void Them_Overlapping_IsRejected_WithConflictInfo()
        {
            Assert.True(LichChieuService.Them(1, 1, FakeNow.AddHours(2)).ThanhCong);
            var kq = LichChieuService.Them(2, 1, FakeNow.AddHours(3));
            Assert.False(kq.ThanhCong);
            Assert.NotNull(kq.LichXungDot);
        }

        [Fact]
        public void Them_SameTimeDifferentRoom_IsAllowed()
        {
            Assert.True(LichChieuService.Them(1, 1, FakeNow.AddHours(2)).ThanhCong);
            Assert.True(LichChieuService.Them(1, 2, FakeNow.AddHours(2)).ThanhCong);
        }

        [Fact]
        public void CapNhat_WithTickets_IsRejected()
        {
            LichChieuService.Them(1, 1, FakeNow.AddHours(2));
            var l = DataManager.DanhSachLichChieu[0];
            LichChieuService.GhiNhanBanVe(l.MaLichChieu, 3);

            var kq = LichChieuService.CapNhat(l.MaLichChieu, 1, 1, FakeNow.AddHours(5));
            Assert.False(kq.ThanhCong);
        }

        [Fact]
        public void Xoa_WithoutTickets_Succeeds_WithTickets_Fails()
        {
            LichChieuService.Them(1, 1, FakeNow.AddHours(2));
            LichChieuService.Them(1, 2, FakeNow.AddHours(2));
            var a = DataManager.DanhSachLichChieu[0];
            var b = DataManager.DanhSachLichChieu[1];
            LichChieuService.GhiNhanBanVe(b.MaLichChieu, 1);

            Assert.True(LichChieuService.Xoa(a.MaLichChieu).ThanhCong);
            Assert.False(LichChieuService.Xoa(b.MaLichChieu).ThanhCong);
            Assert.Single(DataManager.DanhSachLichChieu);
        }

        [Fact]
        public void CapNhat_MoveToFreeSlot_Succeeds_AndIgnoresItself()
        {
            LichChieuService.Them(1, 1, FakeNow.AddHours(2));
            var l = DataManager.DanhSachLichChieu[0];
            // Shift by 30 minutes: overlaps its own old slot, which must be ignored.
            var kq = LichChieuService.CapNhat(l.MaLichChieu, 1, 1, FakeNow.AddHours(2).AddMinutes(30));
            Assert.True(kq.ThanhCong, kq.ThongBao);
        }

        [Fact]
        public void GhiNhanBanVe_BeyondRoomSeats_Fails()
        {
            LichChieuService.Them(1, 4, FakeNow.AddHours(2));          // Room 4 has 40 seats
            var l = DataManager.DanhSachLichChieu[0];
            Assert.False(LichChieuService.GhiNhanBanVe(l.MaLichChieu, 41).ThanhCong);
            Assert.True(LichChieuService.GhiNhanBanVe(l.MaLichChieu, 40).ThanhCong);
            Assert.Equal(40, DataManager.DanhSachLichChieu[0].SoVeDaBan);
        }

        [Fact]
        public void GoiYGioTrong_SkipsOverBusySlotAndBuffer()
        {
            DataManager.DanhSachLichChieu.Add(new LichChieu
            {
                MaLichChieu = 99, MaPhong = 1, MaPhim = 1,
                GioBatDau = FakeNow.Date.AddHours(10), GioKetThuc = FakeNow.Date.AddHours(12), TenPhim = "X"
            });

            // Movie 5 = 112 min. Before 10:00 it can never fit; first free start is 12:15 (12:00 + 15 min buffer).
            var goiY = LichChieuService.GoiYGioTrong(5, 1, FakeNow.Date.AddHours(9));
            Assert.Equal(FakeNow.Date.AddHours(12).AddMinutes(15), goiY);
        }

        [Fact]
        public void GoiYGioTrong_ReturnsNull_WhenDayIsFull()
        {
            // Room 1 busy all day long.
            DataManager.DanhSachLichChieu.Add(new LichChieu
            {
                MaLichChieu = 98, MaPhong = 1, MaPhim = 1,
                GioBatDau = FakeNow.Date, GioKetThuc = FakeNow.Date.AddDays(1).AddHours(1), TenPhim = "X"
            });
            Assert.Null(LichChieuService.GoiYGioTrong(5, 1, FakeNow.Date.AddHours(9)));
        }

        [Fact]
        public void KiemTraKhaDung_ReportsPastAndConflict()
        {
            Assert.False(LichChieuService.KiemTraKhaDung(1, 1, FakeNow.AddHours(-1)).ThanhCong);
            LichChieuService.Them(1, 1, FakeNow.AddHours(2));
            var kq = LichChieuService.KiemTraKhaDung(2, 1, FakeNow.AddHours(3));
            Assert.False(kq.ThanhCong);
            Assert.NotNull(kq.LichXungDot);
            Assert.True(LichChieuService.KiemTraKhaDung(2, 3, FakeNow.AddHours(3)).ThanhCong);
        }
    }

    public class JsonRepositoryTests : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), "RapPhimRepo_" + Guid.NewGuid().ToString("N"));

        public void Dispose()
        {
            if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
        }

        private JsonLichChieuRepository NewRepo() => new(Path.Combine(_dir, "lichchieu.json"));

        [Fact]
        public void NoFile_HasDataFalse_LoadEmpty()
        {
            var repo = NewRepo();
            Assert.False(repo.HasData());
            Assert.Empty(repo.LoadAll());
        }

        [Fact]
        public void InsertThenLoad_RoundTrips()
        {
            var repo = NewRepo();
            var original = new LichChieu
            {
                MaPhim = 2, MaPhong = 3,
                GioBatDau = new DateTime(2030, 1, 15, 10, 0, 0), GioKetThuc = new DateTime(2030, 1, 15, 12, 12, 0),
                SoVeDaBan = 12, TenPhim = "Lat Mat 7", TenPhong = "Room 3 - IMAX", ThoiLuongPhim = 132
            };
            var inserted = repo.Insert(original);

            Assert.True(repo.HasData());
            var loaded = Assert.Single(repo.LoadAll());
            Assert.Equal(inserted.MaLichChieu, loaded.MaLichChieu);
            Assert.Equal(original.GioBatDau, loaded.GioBatDau);
            Assert.Equal(original.GioKetThuc, loaded.GioKetThuc);
            Assert.Equal(original.SoVeDaBan, loaded.SoVeDaBan);
            Assert.Equal(original.TenPhim, loaded.TenPhim);
        }

        [Fact]
        public void CorruptedFile_ReturnsEmpty_AndKeepsBackup()
        {
            var repo = NewRepo();
            Directory.CreateDirectory(_dir);
            File.WriteAllText(repo.FilePath, "{ this is not valid json");

            Assert.Empty(repo.LoadAll());
            Assert.True(File.Exists(repo.FilePath + ".bak"));
        }
    }

    /// <summary>Opens every screen of the module to catch designer/layout/binding crashes.</summary>
    [Collection("Sequential")]
    public class FormSmokeTests : IDisposable
    {
        public FormSmokeTests() => LichChieuService.Now = () => DateTime.Now;
        public void Dispose() => LichChieuService.Now = () => DateTime.Now;

        /// <summary>Designer controls are private fields without a Name, so read them by field name.</summary>
        private static T Ctl<T>(Form f, string field) where T : class =>
            (T)f.GetType().GetField(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;

        private static void RunSta(Action action)
        {
            Exception? error = null;
            var t = new Thread(() =>
            {
                try { action(); } catch (Exception ex) { error = ex; }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            if (error != null) throw new Exception("Form smoke test failed: " + error, error);
        }

        [Fact]
        public void MainForm_Opens() => RunSta(() =>
        {
            using var f = new Form1();
            f.Show();
            f.Close();
        });

        [Fact]
        public void ScheduleForm_Opens_AndAllControlsReact() => RunSta(() =>
        {
            using var f = new FormXepLich();
            f.Show();

            var cboPhim = Ctl<ComboBox>(f, "cboPhim");
            var cboPhong = Ctl<ComboBox>(f, "cboPhong");
            var dtpNgay = Ctl<DateTimePicker>(f, "dtpNgayChieu");
            var btnGoiY = Ctl<Button>(f, "btnGoiYGio");
            var btnReset = Ctl<Button>(f, "btnXoaForm");
            var lblSlot = Ctl<Label>(f, "lblTrangThaiSlot");

            for (int i = 0; i < cboPhim.Items.Count; i++) cboPhim.SelectedIndex = i;
            for (int i = 0; i < cboPhong.Items.Count; i++) cboPhong.SelectedIndex = i;
            dtpNgay.Value = DateTime.Today.AddDays(1);
            btnReset.PerformClick();

            Assert.False(string.IsNullOrEmpty(lblSlot.Text));
            f.Close();
        });

        [Fact]
        public void ScheduleForm_EditMode_WithPastShowtime_DoesNotCrash() => RunSta(() =>
        {
            // Regression: DateTimePicker.MinDate = Today used to throw when loading an old showtime.
            var old = new LichChieu
            {
                MaLichChieu = 1000, MaPhim = 1, MaPhong = 1,
                GioBatDau = DateTime.Today.AddDays(-30).AddHours(10),
                GioKetThuc = DateTime.Today.AddDays(-30).AddHours(13),
                TenPhim = "Avengers: Endgame", TenPhong = "Room 1 - 2D", ThoiLuongPhim = 181
            };
            using var f = new FormXepLich(old);
            f.Show();
            f.Close();
        });

        [Fact]
        public void ListForm_Opens_AndFiltersWork() => RunSta(() =>
        {
            using var f = new FormDanhSachLich();
            f.Show();

            var cboLoc = Ctl<ComboBox>(f, "cboLocPhong");
            var chk = Ctl<CheckBox>(f, "chkTatCaNgay");
            var txt = Ctl<TextBox>(f, "txtTimPhim");
            var btnLoc = Ctl<Button>(f, "btnLoc");
            var grid = Ctl<DataGridView>(f, "dgvDanhSach");

            for (int i = 0; i < cboLoc.Items.Count; i++) { cboLoc.SelectedIndex = i; btnLoc.PerformClick(); }
            chk.Checked = false; btnLoc.PerformClick();
            chk.Checked = true; btnLoc.PerformClick();
            txt.Text = "zzz-no-such-movie";
            Assert.Equal(0, grid.Rows.Count);
            txt.Text = "";
            f.Close();
        });
    }
}
