using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Data
{
    /// <summary>
    /// Central data holder — movies, rooms, and showtimes.
    /// Showtimes are persisted through <see cref="Repository"/> (JSON file by default).
    /// Scheduling rules live in <see cref="LichChieuService"/>; the public methods below are thin
    /// wrappers kept for backward compatibility.
    /// </summary>
    public static class DataManager
    {
        private static int _nextLichChieuId = 1;

        /// <summary>Storage backend. Replace with a SQL implementation of ILichChieuRepository later.</summary>
        public static ILichChieuRepository Repository { get; set; } = new JsonLichChieuRepository();

        static DataManager()
        {
            KhoiTaoLichChieu();
        }

        // ===== SAMPLE MOVIES =====
        public static List<Phim> DanhSachPhim { get; } = new List<Phim>
        {
            new Phim { MaPhim = 1, TenPhim = "Avengers: Endgame", ThoiLuong = 181, TheLoai = "Action" },
            new Phim { MaPhim = 2, TenPhim = "Lat Mat 7", ThoiLuong = 132, TheLoai = "Action" },
            new Phim { MaPhim = 3, TenPhim = "Mai", ThoiLuong = 131, TheLoai = "Drama" },
            new Phim { MaPhim = 4, TenPhim = "Doraemon: Nobita's Sky Utopia", ThoiLuong = 108, TheLoai = "Animation" },
            new Phim { MaPhim = 5, TenPhim = "Nha Ba Nu", ThoiLuong = 112, TheLoai = "Comedy" },
            new Phim { MaPhim = 6, TenPhim = "Oppenheimer", ThoiLuong = 180, TheLoai = "Drama" },
            new Phim { MaPhim = 7, TenPhim = "Spider-Man: Across the Spider-Verse", ThoiLuong = 140, TheLoai = "Animation" },
            new Phim { MaPhim = 8, TenPhim = "Dat Rung Phuong Nam", ThoiLuong = 110, TheLoai = "Adventure" }
        };

        // ===== SAMPLE ROOMS =====
        public static List<PhongChieu> DanhSachPhongChieu { get; } = new List<PhongChieu>
        {
            new PhongChieu { MaPhong = 1, TenPhong = "Room 1 - 2D", SoGhe = 120 },
            new PhongChieu { MaPhong = 2, TenPhong = "Room 2 - 3D", SoGhe = 80 },
            new PhongChieu { MaPhong = 3, TenPhong = "Room 3 - IMAX", SoGhe = 200 },
            new PhongChieu { MaPhong = 4, TenPhong = "Room 4 - VIP", SoGhe = 40 },
            new PhongChieu { MaPhong = 5, TenPhong = "Room 5 - 2D", SoGhe = 100 }
        };

        // ===== SCHEDULE LIST =====
        public static List<LichChieu> DanhSachLichChieu { get; } = new List<LichChieu>();

        // =====================================================
        // PERSISTENCE
        // =====================================================

        private static void KhoiTaoLichChieu()
        {
            try
            {
                if (Repository.HasData())
                {
                    DanhSachLichChieu.Clear();
                    DanhSachLichChieu.AddRange(Repository.LoadAll());
                    DongBoTenTuPhimPhong();
                    _nextLichChieuId = DanhSachLichChieu.Count == 0 ? 1 : DanhSachLichChieu.Max(l => l.MaLichChieu) + 1;
                    return;
                }
            }
            catch (Exception)
            {
                // Unreadable store: fall back to sample data below.
            }

            TaoDuLieuMau();
            Luu();
        }

        /// <summary>Refresh the cached movie/room names in loaded showtimes (they may have been renamed).</summary>
        private static void DongBoTenTuPhimPhong()
        {
            foreach (var l in DanhSachLichChieu)
            {
                var phim = DanhSachPhim.FirstOrDefault(p => p.MaPhim == l.MaPhim);
                var phong = DanhSachPhongChieu.FirstOrDefault(p => p.MaPhong == l.MaPhong);
                if (phim != null) l.TenPhim = phim.TenPhim;
                if (phong != null) l.TenPhong = phong.TenPhong;
            }
        }

        /// <summary>Next free showtime id.</summary>
        public static int CapPhatMa() => _nextLichChieuId++;

        /// <summary>Save showtimes to the repository. Returns false if saving failed (app keeps working).</summary>
        public static bool Luu()
        {
            try
            {
                Repository.SaveAll(DanhSachLichChieu);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // BACKWARD-COMPATIBLE WRAPPERS (logic is in LichChieuService)
        // =====================================================

        public static LichChieu? KiemTraTrungGio(int maPhong, DateTime gioBatDau, DateTime gioKetThuc, int maLichChieuHienTai = -1)
            => LichChieuService.TimXungDot(DanhSachLichChieu, maPhong, gioBatDau, gioKetThuc, maLichChieuHienTai);

        public static (bool Success, string Message, LichChieu? Conflict) ThemLichChieu(int maPhim, int maPhong, DateTime gioBatDau)
        {
            var kq = LichChieuService.Them(maPhim, maPhong, gioBatDau);
            return (kq.ThanhCong, kq.ThongBao, kq.LichXungDot);
        }

        public static (bool Success, string Message) CapNhatLichChieu(int maLichChieu, int maPhim, int maPhong, DateTime gioBatDau)
        {
            var kq = LichChieuService.CapNhat(maLichChieu, maPhim, maPhong, gioBatDau);
            return (kq.ThanhCong, kq.ThongBao);
        }

        public static (bool Success, string Message) XoaLichChieu(int maLichChieu)
        {
            var kq = LichChieuService.Xoa(maLichChieu);
            return (kq.ThanhCong, kq.ThongBao);
        }

        // =====================================================
        // SAMPLE DATA
        // =====================================================

        /// <summary>
        /// Create sample showtimes for tomorrow (so they are editable in a demo).
        /// Bypasses the "not in the past" rule on purpose.
        /// </summary>
        public static void TaoDuLieuMau()
        {
            if (DanhSachLichChieu.Count > 0) return;

            var ngay = DateTime.Today.AddDays(1);
            (int phim, int phong, int gio)[] mau =
            {
                (1, 1, 9), (2, 1, 13), (3, 2, 10), (4, 3, 14), (5, 2, 15)
            };

            foreach (var (maPhim, maPhong, gio) in mau)
            {
                var phim = DanhSachPhim.First(p => p.MaPhim == maPhim);
                var phong = DanhSachPhongChieu.First(p => p.MaPhong == maPhong);
                var bd = ngay.AddHours(gio);
                DanhSachLichChieu.Add(new LichChieu
                {
                    MaLichChieu = _nextLichChieuId++,
                    MaPhim = maPhim,
                    MaPhong = maPhong,
                    GioBatDau = bd,
                    GioKetThuc = bd.AddMinutes(phim.ThoiLuong),
                    SoVeDaBan = 0,
                    TenPhim = phim.TenPhim,
                    TenPhong = phong.TenPhong,
                    ThoiLuongPhim = phim.ThoiLuong
                });
            }

            // One showtime already has tickets sold (locked: cannot edit/delete)
            DanhSachLichChieu[0].SoVeDaBan = 25;
        }
    }
}
