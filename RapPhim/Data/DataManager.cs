using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Data
{
    /// <summary>
    /// Central data holder — movies, rooms, and showtimes.
    /// Showtimes are persisted per-record through <see cref="Repository"/> configured in <see cref="CauHinhLuuTru"/>.
    /// Scheduling rules live in <see cref="LichChieuService"/>; the public methods below are thin
    /// wrappers kept for backward compatibility.
    /// </summary>
    public static class DataManager
    {
        private static ILichChieuRepository? _repo;
        private static readonly List<LichChieu> _danhSachLichChieu = new();
        private static bool _daTaiLichChieu = false;
        private static bool _choPhepTaoMau = true;

        /// <summary>Active storage backend (lazy-initialized via <see cref="CauHinhLuuTru"/>).</summary>
        public static ILichChieuRepository Repository => _repo ??= CauHinhLuuTru.TaoRepository();

        /// <summary>
        /// Swap the active repository (for tests or custom configuration) and reset the in-memory cache.
        /// </summary>
        public static void DungRepository(ILichChieuRepository repo, bool choPhepTaoMau = false)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _danhSachLichChieu.Clear();
            _daTaiLichChieu = false;
            _choPhepTaoMau = choPhepTaoMau;
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

        // ===== SCHEDULE LIST (lazy-loaded cache) =====
        public static List<LichChieu> DanhSachLichChieu
        {
            get
            {
                if (!_daTaiLichChieu) TaiLai();
                return _danhSachLichChieu;
            }
        }

        // =====================================================
        // RELOAD & SYNC
        // =====================================================

        /// <summary>Reload showtimes fresh from the repository and synchronize movie/room names.</summary>
        public static void TaiLai()
        {
            var repo = Repository;
            if (!_daTaiLichChieu && _choPhepTaoMau && !repo.HasData())
            {
                _daTaiLichChieu = true;
                TaoDuLieuMau();
                return;
            }

            List<LichChieu> loaded;
            try
            {
                loaded = repo.LoadAll();
            }
            catch (Exception)
            {
                loaded = new List<LichChieu>();
            }

            _danhSachLichChieu.Clear();
            _danhSachLichChieu.AddRange(loaded);
            DongBoTenTuPhimPhong();
            _daTaiLichChieu = true;
        }

        /// <summary>Refresh the cached movie/room names in loaded showtimes (they may have been renamed).</summary>
        private static void DongBoTenTuPhimPhong()
        {
            foreach (var l in _danhSachLichChieu)
            {
                var phim = DanhSachPhim.FirstOrDefault(p => p.MaPhim == l.MaPhim);
                var phong = DanhSachPhongChieu.FirstOrDefault(p => p.MaPhong == l.MaPhong);
                if (phim != null) l.TenPhim = phim.TenPhim;
                if (phong != null) l.TenPhong = phong.TenPhong;
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
        /// Seed 5 sample showtimes for tomorrow through <see cref="Repository"/>.
        /// Bypasses the "not in the past" rule on purpose.
        /// </summary>
        public static void TaoDuLieuMau()
        {
            if (Repository.HasData())
            {
                TaiLai();
                return;
            }

            var ngay = DateTime.Today.AddDays(1);
            (int phim, int phong, int gio, int ve)[] mau =
            {
                (1, 1, 9, 25),   // One showtime already has tickets sold (locked: cannot edit/delete)
                (2, 1, 13, 0),
                (3, 2, 10, 0),
                (4, 3, 14, 0),
                (5, 2, 15, 0)
            };

            try
            {
                foreach (var (maPhim, maPhong, gio, ve) in mau)
                {
                    var phim = DanhSachPhim.First(p => p.MaPhim == maPhim);
                    var phong = DanhSachPhongChieu.First(p => p.MaPhong == maPhong);
                    var bd = ngay.AddHours(gio);
                    Repository.Insert(new LichChieu
                    {
                        MaPhim = maPhim,
                        MaPhong = maPhong,
                        GioBatDau = bd,
                        GioKetThuc = bd.AddMinutes(phim.ThoiLuong),
                        SoVeDaBan = ve,
                        TenPhim = phim.TenPhim,
                        TenPhong = phong.TenPhong,
                        ThoiLuongPhim = phim.ThoiLuong
                    });
                }
            }
            catch (Exception)
            {
                // Ignore seed error on read-only storage
            }

            TaiLai();
        }
    }
}
