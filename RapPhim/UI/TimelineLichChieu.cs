using System.Drawing.Drawing2D;
using RapPhim.Data;
using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.UI
{
    /// <summary>
    /// Custom-painted day timeline (08:00 - 24:00) showing showtime blocks per screening room.
    /// Zero external dependencies; uses DoubleBuffered OnPaint.
    /// </summary>
    public class TimelineLichChieu : UserControl
    {
        public const int GIO_BAT_DAU_TRUC = 8;
        public const int GIO_KET_THUC_TRUC = 24;

        private const int RONG_COT_PHONG = 145;
        private const int CAO_HEADER = 36;
        private const int CAO_HANG = 58;
        private const int LE_PHAI = 18;

        private DateTime _ngay = DateTime.Today;
        private List<PhongChieu> _phong = new();
        private List<LichChieu> _suat = new();
        private readonly List<(RectangleF Rect, LichChieu Suat)> _khoiDaVe = new();

        private readonly ToolTip _toolTip = new();
        private int _maDangChon = -1;
        private int _maHover = -1;

        public event Action<int>? SuatDuocChon;

        public TimelineLichChieu()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.FromArgb(18, 18, 20);
            ForeColor = Color.FromArgb(244, 244, 245);
            Font = new Font("Segoe UI", 9F);

            MouseMove += OnTimelineMouseMove;
            MouseClick += OnTimelineMouseClick;
            MouseLeave += (_, _) =>
            {
                _maHover = -1;
                _toolTip.Hide(this);
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _toolTip.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Pure coordinate helper: maps a timestamp on <paramref name="ngay"/> from [08:00, 24:00] to [0, chieuRong].
        /// </summary>
        public static float ThoiGianSangX(DateTime thoiDiem, DateTime ngay, float chieuRong)
        {
            if (chieuRong <= 0) return 0f;
            double tongPhut = (GIO_KET_THUC_TRUC - GIO_BAT_DAU_TRUC) * 60.0;
            double phutTuMoCua = (thoiDiem - ngay.Date.AddHours(GIO_BAT_DAU_TRUC)).TotalMinutes;
            double tiLe = Math.Clamp(phutTuMoCua / tongPhut, 0.0, 1.0);
            return (float)(tiLe * chieuRong);
        }

        public void NapDuLieu(DateTime ngay, IEnumerable<PhongChieu> danhSachPhong, IEnumerable<LichChieu> danhSachSuat, int maDangChon = -1)
        {
            _ngay = ngay.Date;
            _phong = danhSachPhong.ToList();
            DateTime cuoiNgay = _ngay.AddDays(1);
            _suat = danhSachSuat
                .Where(l => l.GioBatDau < cuoiNgay && l.GioKetThuc > _ngay)
                .OrderBy(l => l.GioBatDau)
                .ToList();
            _maDangChon = maDangChon;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            _khoiDaVe.Clear();

            float trucRong = Math.Max(100, Width - RONG_COT_PHONG - LE_PHAI);
            int soHang = Math.Max(1, _phong.Count);
            float tongCao = CAO_HEADER + soHang * CAO_HANG;

            using var headerBg = new SolidBrush(Color.FromArgb(27, 27, 31));
            using var rowAltBg = new SolidBrush(Color.FromArgb(24, 24, 28));
            using var gridPen = new Pen(Color.FromArgb(45, 45, 50), 1f);
            using var axisTextBrush = new SolidBrush(Color.FromArgb(161, 161, 170));
            using var roomTextBrush = new SolidBrush(Color.FromArgb(244, 244, 245));
            using var boldFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            using var smallFont = new Font("Segoe UI", 8F);

            // Header bar
            g.FillRectangle(headerBg, 0, 0, Width, CAO_HEADER);
            g.DrawString($"📅 {_ngay:dd/MM/yyyy}", boldFont, roomTextBrush, new PointF(12, 9));

            // Alternating room rows + horizontal dividers
            for (int i = 0; i < _phong.Count; i++)
            {
                float y = CAO_HEADER + i * CAO_HANG;
                if (i % 2 == 1)
                {
                    g.FillRectangle(rowAltBg, 0, y, Width, CAO_HANG);
                }
                g.DrawLine(gridPen, 0, y, Width, y);
                var roomRect = new RectangleF(10, y + 6, RONG_COT_PHONG - 16, CAO_HANG - 12);
                var sfRoom = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(_phong[i].TenPhong, boldFont, roomTextBrush, roomRect, sfRoom);
            }
            g.DrawLine(gridPen, 0, tongCao, Width, tongCao);

            // Vertical hour gridlines (08:00 .. 24:00)
            for (int h = GIO_BAT_DAU_TRUC; h <= GIO_KET_THUC_TRUC; h++)
            {
                float x = RONG_COT_PHONG + ThoiGianSangX(_ngay.AddHours(h), _ngay, trucRong);
                g.DrawLine(gridPen, x, CAO_HEADER, x, tongCao);
                string label = h == 24 ? "24:00" : $"{h:00}:00";
                g.DrawString(label, smallFont, axisTextBrush, new PointF(x - 16, 10));
            }

            // Showtime blocks
            DateTime now = LichChieuService.Now();
            var sfBlock = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };

            for (int i = 0; i < _phong.Count; i++)
            {
                int maPhong = _phong[i].MaPhong;
                float y = CAO_HEADER + i * CAO_HANG + 8;
                float hBlock = CAO_HANG - 16;

                foreach (var suat in _suat.Where(s => s.MaPhong == maPhong))
                {
                    float x1 = RONG_COT_PHONG + ThoiGianSangX(suat.GioBatDau, _ngay, trucRong);
                    float x2 = RONG_COT_PHONG + ThoiGianSangX(suat.GioKetThuc, _ngay, trucRong);
                    float w = Math.Max(18f, x2 - x1);
                    var rect = new RectangleF(x1, y, w, hBlock);
                    _khoiDaVe.Add((rect, suat));

                    var (fill, border, textCol) = MauTheoTrangThai(suat, now);
                    bool selected = suat.MaLichChieu == _maDangChon;

                    using var path = TaoBoGoc(rect, 6f);
                    using var fillBrush = new SolidBrush(fill);
                    using var borderPen = new Pen(selected ? Color.FromArgb(252, 211, 77) : border, selected ? 2.2f : 1.2f);
                    using var textBrush = new SolidBrush(textCol);

                    g.FillPath(fillBrush, path);
                    g.DrawPath(borderPen, path);

                    if (w >= 44)
                    {
                        var titleRect = new RectangleF(rect.X + 6, rect.Y + 4, rect.Width - 10, 18);
                        var timeRect = new RectangleF(rect.X + 6, rect.Y + 22, rect.Width - 10, 16);
                        g.DrawString(suat.TenPhim, boldFont, textBrush, titleRect, sfBlock);
                        g.DrawString($"{suat.GioBatDau:HH:mm}-{suat.GioKetThuc:HH:mm}", smallFont, textBrush, timeRect, sfBlock);
                    }
                }
            }
        }

        private static (Color Fill, Color Border, Color Text) MauTheoTrangThai(LichChieu l, DateTime now)
        {
            if (l.GioKetThuc <= now)
                return (Color.FromArgb(39, 39, 42), Color.FromArgb(82, 82, 91), Color.FromArgb(161, 161, 170));
            if (!LichChieuService.CoTheSuaXoa(l, now, out _))
                return (Color.FromArgb(48, 16, 24), Color.FromArgb(244, 63, 94), Color.FromArgb(254, 205, 211));
            return (Color.FromArgb(14, 40, 30), Color.FromArgb(52, 211, 153), Color.FromArgb(167, 243, 208));
        }

        private static GraphicsPath TaoBoGoc(RectangleF r, float radius)
        {
            float d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private LichChieu? TimSuatTaiDiem(Point p)
        {
            foreach (var (rect, suat) in _khoiDaVe)
            {
                if (rect.Contains(p)) return suat;
            }
            return null;
        }

        private void OnTimelineMouseMove(object? sender, MouseEventArgs e)
        {
            var suat = TimSuatTaiDiem(e.Location);
            Cursor = suat != null ? Cursors.Hand : Cursors.Default;

            int ma = suat?.MaLichChieu ?? -1;
            if (ma == _maHover) return;
            _maHover = ma;

            if (suat == null)
            {
                _toolTip.Hide(this);
            }
            else
            {
                int ve = DataManager.VeDaBanProvider.DemVeDaBan(suat.MaLichChieu);
                string tip = $"{suat.TenPhim}\n{suat.TenPhong}\n{suat.GioBatDau:HH:mm} - {suat.GioKetThuc:HH:mm dd/MM/yyyy}\nTickets sold: {ve}";
                _toolTip.Show(tip, this, e.X + 14, e.Y + 14, 5000);
            }
        }

        private void OnTimelineMouseClick(object? sender, MouseEventArgs e)
        {
            var suat = TimSuatTaiDiem(e.Location);
            if (suat == null) return;

            _maDangChon = suat.MaLichChieu;
            Invalidate();
            SuatDuocChon?.Invoke(suat.MaLichChieu);
        }
    }
}
