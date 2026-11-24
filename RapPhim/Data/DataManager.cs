using RapPhim.Models;
using RapPhim.Services;

namespace RapPhim.Data
{
    /// <summary>
    /// Central data holder — movies, rooms, and showtimes.
    /// Storage &amp; read-only providers are configured in <see cref="CauHinhLuuTru"/>.
    /// Scheduling rules live in <see cref="LichChieuService"/>; the public methods below are thin
    /// wrappers kept for backward compatibility.
    /// </summary>
    public static class DataManager
    {
        private static ILichChieuRepository? _repo;
        private static IDanhMucProvider? _danhMucProvider;
        private static IVeDaBanProvider? _veDaBanProvider;

        private static readonly List<Phim> _danhSachPhim = new();
        private static readonly List<PhongChieu> _danhSachPhongChieu = new();
        private static readonly List<LichChieu> _danhSachLichChieu = new();

        private static bool _daTaiDanhMuc = false;
        private static bool _daTaiLichChieu = false;
        private static bool _choPhepTaoMau = true;

        /// <summary>Active showtime repository (lazy-initialized via <see cref="CauHinhLuuTru"/>).</summary>
        public static ILichChieuRepository Repository => _repo ??= CauHinhLuuTru.TaoRepository();

        /// <summary>Active read-only movie/room catalog provider.</summary>
        public static IDanhMucProvider DanhMucProvider => _danhMucProvider ??= CauHinhLuuTru.TaoDanhMucProvider();

        /// <summary>Active sold-ticket counter provider.</summary>
        public static IVeDaBanProvider VeDaBanProvider => _veDaBanProvider ??= CauHinhLuuTru.TaoVeDaBanProvider();

        /// <summary>Swap the showtime repository (for tests or custom configuration) and reset the cache.</summary>
        public static void DungRepository(ILichChieuRepository repo, bool choPhepTaoMau = false)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _danhSachLichChieu.Clear();
            _daTaiLichChieu = false;
            _choPhepTaoMau = choPhepTaoMau;
        }

        /// <summary>Swap the read-only catalog provider (for tests or DB integration) and reset the catalog cache.</summary>
        public static void DungDanhMucProvider(IDanhMucProvider provider)
        {
            _danhMucProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _danhSachPhim.Clear();
            _danhSachPhongChieu.Clear();
            _daTaiDanhMuc = false;
        }

        /// <summary>Swap the sold-ticket counter provider (for tests or DB integration).</summary>
        public static void DungVeDaBanProvider(IVeDaBanProvider provider)
        {
            _veDaBanProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        // ===== CATALOGS (lazy-loaded from IDanhMucProvider) =====
        public static List<Phim> DanhSachPhim
        {
            get
            {
                if (!_daTaiDanhMuc) TaiDanhMuc();
                return _danhSachPhim;
            }
        }

        public static List<PhongChieu> DanhSachPhongChieu
        {
            get
            {
                if (!_daTaiDanhMuc) TaiDanhMuc();
                return _danhSachPhongChieu;
            }
        }

        // ===== SCHEDULE LIST (lazy-loaded from ILichChieuRepository) =====
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

        private static void TaiDanhMuc()
        {
            var provider = DanhMucProvider;
            _danhSachPhim.Clear();
            _danhSachPhim.AddRange(provider.LayDanhSachPhim());
            _danhSachPhongChieu.Clear();
            _danhSachPhongChieu.AddRange(provider.LayDanhSachPhong());
            _daTaiDanhMuc = true;
        }

        /// <summary>Reload movies, rooms, and showtimes fresh from their providers/repository.</summary>
        public static void TaiLai()
        {
            TaiDanhMuc();

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
                var phim = _danhSachPhim.FirstOrDefault(p => p.MaPhim == l.MaPhim);
                var phong = _danhSachPhongChieu.FirstOrDefault(p => p.MaPhong == l.MaPhong);
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

            if (!_daTaiDanhMuc) TaiDanhMuc();

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
                    var phim = _danhSachPhim.FirstOrDefault(p => p.MaPhim == maPhim);
                    var phong = _danhSachPhongChieu.FirstOrDefault(p => p.MaPhong == maPhong);
                    if (phim == null || phong == null) continue;

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
