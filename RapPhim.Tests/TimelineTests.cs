using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;
using RapPhim.UI;

namespace RapPhim.Tests
{
    public class TimelineMathTests
    {
        private static readonly DateTime Day = new(2030, 1, 15);

        [Fact]
        public void OpeningHour_0800_MapsToZero() =>
            Assert.Equal(0f, TimelineLichChieu.ThoiGianSangX(Day.AddHours(8), Day, 800f), precision: 3);

        [Fact]
        public void ClosingHour_2400_MapsToFullWidth() =>
            Assert.Equal(800f, TimelineLichChieu.ThoiGianSangX(Day.AddDays(1), Day, 800f), precision: 3);

        [Fact]
        public void Midpoint_1600_MapsToHalfWidth() =>
            Assert.Equal(400f, TimelineLichChieu.ThoiGianSangX(Day.AddHours(16), Day, 800f), precision: 3);

        [Fact]
        public void OutOfRange_ClampsToBounds()
        {
            Assert.Equal(0f, TimelineLichChieu.ThoiGianSangX(Day.AddHours(6), Day, 800f));
            Assert.Equal(800f, TimelineLichChieu.ThoiGianSangX(Day.AddDays(1).AddHours(2), Day, 800f));
        }
    }

    [Collection("Sequential")]
    public class TimelineUiTests : IDisposable
    {
        private readonly ILichChieuRepository _prevRepo;
        private readonly Func<DateTime> _prevNow;

        public TimelineUiTests()
        {
            _prevRepo = DataManager.Repository;
            _prevNow = LichChieuService.Now;
            DataManager.DungRepository(new InMemoryLichChieuRepository());
            LichChieuService.Now = () => new DateTime(2030, 1, 15, 8, 0, 0);
        }

        public void Dispose()
        {
            LichChieuService.Now = _prevNow;
            DataManager.DungRepository(_prevRepo);
        }

        private static void RunSta(Action action)
        {
            Exception? err = null;
            var t = new Thread(() => { try { action(); } catch (Exception ex) { err = ex; } });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            if (err != null) throw new Exception("Timeline UI test failed", err);
        }

        [Fact]
        public void TimelineControl_DrawToBitmap_DoesNotThrow() => RunSta(() =>
        {
            var day = new DateTime(2030, 1, 15);
            using var ctrl = new TimelineLichChieu { Size = new Size(900, 380) };
            ctrl.NapDuLieu(day, DataManager.DanhSachPhongChieu, new List<LichChieu>
            {
                new()
                {
                    MaLichChieu = 1, MaPhim = 1, MaPhong = 1,
                    GioBatDau = day.AddHours(9), GioKetThuc = day.AddHours(12),
                    TenPhim = "Avengers: Endgame", TenPhong = "Room 1 - 2D", ThoiLuongPhim = 180
                },
                new()
                {
                    MaLichChieu = 2, MaPhim = 2, MaPhong = 2,
                    GioBatDau = day.AddHours(14), GioKetThuc = day.AddHours(16),
                    SoVeDaBan = 10, TenPhim = "Lat Mat 7", TenPhong = "Room 2 - 3D", ThoiLuongPhim = 120
                }
            }, maDangChon: 1);

            using var bmp = new Bitmap(ctrl.Width, ctrl.Height);
            ctrl.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            Assert.Equal(900, bmp.Width);
        });

        [Fact]
        public void ListForm_ToggleTimelineButton_SwitchesViewCleanly() => RunSta(() =>
        {
            using var f = new FormDanhSachLich();
            f.Show();

            var btnToggle = (Button)f.GetType().GetField("btnCheDoXem",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;
            var grid = (DataGridView)f.GetType().GetField("dgvDanhSach",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;
            var timeline = (TimelineLichChieu)f.GetType().GetField("timelineLichChieu",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(f)!;

            Assert.True(grid.Visible);
            Assert.False(timeline.Visible);

            btnToggle.PerformClick();
            Assert.False(grid.Visible);
            Assert.True(timeline.Visible);
            Assert.Contains("Table", btnToggle.Text);

            btnToggle.PerformClick();
            Assert.True(grid.Visible);
            Assert.False(timeline.Visible);

            f.Close();
        });
    }
}
