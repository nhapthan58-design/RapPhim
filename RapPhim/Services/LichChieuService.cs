using RapPhim.Data;
using RapPhim.Models;

namespace RapPhim.Services
{
    /// <summary>
    /// All business rules for showtime scheduling.
    /// Pure helpers (TimXungDot, CoTheSuaXoa, ApDungBanVe) take their inputs as parameters so they can be unit tested.
    /// </summary>
    public static class LichChieuService
    {
        /// <summary>Gap (minutes) required between two showtimes in the same room for cleaning.</summary>
        public const int BUFFER_PHUT = 15;

        /// <summary>Clock used by the service (replaceable in tests).</summary>
        public static Func<DateTime> Now { get; set; } = () => DateTime.Now;

        // =====================================================
        // PURE HELPERS
        // =====================================================

        /// <summary>
        /// Find a showtime in the same room that overlaps [batDau, ketThuc] (including the cleaning buffer).
        /// Overlap rule: new.start &lt; old.end + buffer  AND  new.end + buffer &gt; old.start
        /// </summary>
        public static LichChieu? TimXungDot(IEnumerable<LichChieu> ds, int maPhong,
            DateTime batDau, DateTime ketThuc, int boQuaMa = -1)
        {
            foreach (var cu in ds)
            {
                if (cu.MaLichChieu == boQuaMa) continue;   // skip the showtime being edited
                if (cu.MaPhong != maPhong) continue;       // only the same room

                bool trung = batDau < cu.GioKetThuc.AddMinutes(BUFFER_PHUT)
                          && ketThuc.AddMinutes(BUFFER_PHUT) > cu.GioBatDau;
                if (trung) return cu;
            }
            return null;
        }

        /// <summary>Pure lock check: a showtime is locked once tickets are sold or it has already started.</summary>
        public static bool CoTheSuaXoa(LichChieu l, DateTime now, int soVeDaBan, out string lyDo)
        {
            if (soVeDaBan > 0)
            {
                lyDo = "Tickets already sold for this showtime.";
                return false;
            }
            if (l.GioBatDau <= now)
            {
                lyDo = "Showtime has already started or ended.";
                return false;
            }
            lyDo = string.Empty;
            return true;
        }

        /// <summary>Lock check using <see cref="DataManager.VeDaBanProvider"/> for sold ticket count.</summary>
        public static bool CoTheSuaXoa(LichChieu l, DateTime now, out string lyDo)
        {
            int soVe = DataManager.VeDaBanProvider.DemVeDaBan(l.MaLichChieu);
            return CoTheSuaXoa(l, now, soVe, out lyDo);
        }

        public static bool CoTheSuaXoa(LichChieu l, out string lyDo) => CoTheSuaXoa(l, Now(), out lyDo);

        /// <summary>Validate and apply a ticket sale to a showtime (pure: no storage access).</summary>
        public static KetQua ApDungBanVe(LichChieu l, int soGhe, int soVe)
        {
            if (soVe <= 0) return new KetQua(false, "Ticket count must be greater than 0.");
            if (l.SoVeDaBan + soVe > soGhe)
                return new KetQua(false, $"Not enough seats. Remaining: {soGhe - l.SoVeDaBan}.");
            l.SoVeDaBan += soVe;
            return new KetQua(true, "Tickets recorded.");
        }

        /// <summary>Validate and apply a ticket cancellation (pure).</summary>
        public static KetQua ApDungHuyVe(LichChieu l, int soVe)
        {
            if (soVe <= 0) return new KetQua(false, "Ticket count must be greater than 0.");
            if (soVe > l.SoVeDaBan) return new KetQua(false, "Cannot cancel more tickets than were sold.");
            l.SoVeDaBan -= soVe;
            return new KetQua(true, "Tickets cancelled.");
        }

        /// <summary>Remove seconds and milliseconds.</summary>
        public static DateTime CatGiay(DateTime d) =>
            new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0, d.Kind);

        // =====================================================
        // VALIDATION (no side effects)
        // =====================================================

        /// <summary>
        /// Check whether a movie can be placed in a room at the given time.
        /// Use for live preview in the UI. <paramref name="boQuaMa"/> = id of the showtime being edited.
        /// </summary>
        public static KetQua KiemTraKhaDung(int maPhim, int maPhong, DateTime batDau, int boQuaMa = -1)
        {
            return Validate(maPhim, maPhong, batDau, boQuaMa, out _, out _, out _, out _);
        }

        private static KetQua Validate(int maPhim, int maPhong, DateTime batDau, int boQuaMa,
            out Phim? phim, out PhongChieu? phong, out DateTime bd, out DateTime kt)
        {
            bd = CatGiay(batDau);
            kt = bd;
            phim = DataManager.DanhSachPhim.FirstOrDefault(p => p.MaPhim == maPhim);
            phong = DataManager.DanhSachPhongChieu.FirstOrDefault(p => p.MaPhong == maPhong);

            if (phim == null) return new KetQua(false, "Movie not found!");
            if (phong == null) return new KetQua(false, "Room not found!");
            if (phim.ThoiLuong <= 0) return new KetQua(false, "Invalid movie duration.");

            kt = bd.AddMinutes(phim.ThoiLuong);

            if (bd <= Now()) return new KetQua(false, "Cannot schedule a showtime in the past.");

            var xungDot = TimXungDot(DataManager.DanhSachLichChieu, maPhong, bd, kt, boQuaMa);
            if (xungDot != null)
                return new KetQua(false,
                    $"Conflicts with {xungDot.TenPhim} ({xungDot.GioBatDau:HH:mm} - {xungDot.GioKetThuc:HH:mm})",
                    xungDot);

            return new KetQua(true, "Slot available.");
        }

        // =====================================================
        // COMMANDS
        // =====================================================

        public static KetQua Them(int maPhim, int maPhong, DateTime batDau)
        {
            DataManager.TaiLai();
            var v = Validate(maPhim, maPhong, batDau, -1, out var phim, out var phong, out var bd, out var kt);
            if (!v.ThanhCong)
            {
                if (v.LichXungDot != null)
                {
                    var c = v.LichXungDot;
                    string msg = "TIME CONFLICT!\n\n" +
                        $"\"{phong!.TenPhong}\" already has a showtime:\n" +
                        $"  Movie: {c.TenPhim}\n" +
                        $"  From: {c.GioBatDau:HH:mm dd/MM/yyyy}\n" +
                        $"  To: {c.GioKetThuc:HH:mm dd/MM/yyyy}\n\n" +
                        $"(Minimum {BUFFER_PHUT}-minute gap required between showtimes)";
                    return new KetQua(false, msg, c);
                }
                return v;
            }

            var moi = new LichChieu
            {
                MaPhim = maPhim,
                MaPhong = maPhong,
                GioBatDau = bd,
                GioKetThuc = kt,
                SoVeDaBan = 0,
                TenPhim = phim!.TenPhim,
                TenPhong = phong!.TenPhong,
                ThoiLuongPhim = phim.ThoiLuong
            };

            try
            {
                DataManager.Repository.Insert(moi);
            }
            catch (Exception ex)
            {
                return new KetQua(false, "Could not save: " + ex.Message);
            }

            DataManager.TaiLai();
            return new KetQua(true,
                $"Showtime scheduled successfully!\n\nMovie: {phim.TenPhim}\nRoom: {phong.TenPhong}\n" +
                $"Time: {bd:HH:mm} - {kt:HH:mm dd/MM/yyyy}");
        }

        public static KetQua CapNhat(int maLichChieu, int maPhim, int maPhong, DateTime batDau)
        {
            DataManager.TaiLai();
            var l = DataManager.DanhSachLichChieu.FirstOrDefault(x => x.MaLichChieu == maLichChieu);
            if (l == null) return new KetQua(false, "Showtime not found!");
            if (!CoTheSuaXoa(l, out var lyDo)) return new KetQua(false, "Cannot edit! " + lyDo);

            var v = Validate(maPhim, maPhong, batDau, maLichChieu, out var phim, out var phong, out var bd, out var kt);
            if (!v.ThanhCong)
                return v.LichXungDot != null ? new KetQua(false, "TIME CONFLICT: " + v.ThongBao, v.LichXungDot) : v;

            var capNhat = new LichChieu
            {
                MaLichChieu = l.MaLichChieu,
                MaPhim = maPhim,
                MaPhong = maPhong,
                GioBatDau = bd,
                GioKetThuc = kt,
                SoVeDaBan = l.SoVeDaBan,
                TenPhim = phim!.TenPhim,
                TenPhong = phong!.TenPhong,
                ThoiLuongPhim = phim.ThoiLuong
            };

            try
            {
                DataManager.Repository.Update(capNhat);
            }
            catch (Exception ex)
            {
                return new KetQua(false, "Could not save: " + ex.Message);
            }

            DataManager.TaiLai();
            return new KetQua(true, "Showtime updated successfully!");
        }

        public static KetQua Xoa(int maLichChieu)
        {
            DataManager.TaiLai();
            var l = DataManager.DanhSachLichChieu.FirstOrDefault(x => x.MaLichChieu == maLichChieu);
            if (l == null) return new KetQua(false, "Showtime not found!");
            if (!CoTheSuaXoa(l, out var lyDo)) return new KetQua(false, "Cannot delete! " + lyDo);

            try
            {
                DataManager.Repository.Delete(maLichChieu);
            }
            catch (Exception ex)
            {
                return new KetQua(false, "Could not save: " + ex.Message);
            }

            DataManager.TaiLai();
            return new KetQua(true, "Showtime deleted successfully!");
        }

        // =====================================================
        // API FOR THE TICKET-SALES MODULE
        // The ticket module MUST use these methods and must NEVER modify SoVeDaBan directly,
        // otherwise the "locked once tickets are sold" rule can be bypassed.
        // =====================================================

        /// <summary>
        /// Record ticket sales for a showtime.
        /// </summary>
        /// <remarks>
        /// Chỉ dùng khi chưa có DB vé của Khôi. Khi đã có, trạng thái khóa lấy từ <see cref="IVeDaBanProvider"/>.
        /// </remarks>
        public static KetQua GhiNhanBanVe(int maLichChieu, int soVe)
        {
            DataManager.TaiLai();
            var l = DataManager.DanhSachLichChieu.FirstOrDefault(x => x.MaLichChieu == maLichChieu);
            if (l == null) return new KetQua(false, "Showtime not found!");
            var phong = DataManager.DanhSachPhongChieu.FirstOrDefault(p => p.MaPhong == l.MaPhong);
            if (phong == null) return new KetQua(false, "Room not found!");

            var kq = ApDungBanVe(l, phong.SoGhe, soVe);
            if (!kq.ThanhCong) return kq;

            try
            {
                DataManager.Repository.Update(l);
            }
            catch (Exception ex)
            {
                DataManager.TaiLai();
                return new KetQua(false, "Could not save: " + ex.Message);
            }

            DataManager.TaiLai();
            return kq;
        }

        /// <summary>
        /// Cancel previously recorded tickets for a showtime.
        /// </summary>
        /// <remarks>
        /// Chỉ dùng khi chưa có DB vé của Khôi. Khi đã có, trạng thái khóa lấy từ <see cref="IVeDaBanProvider"/>.
        /// </remarks>
        public static KetQua HuyVe(int maLichChieu, int soVe)
        {
            DataManager.TaiLai();
            var l = DataManager.DanhSachLichChieu.FirstOrDefault(x => x.MaLichChieu == maLichChieu);
            if (l == null) return new KetQua(false, "Showtime not found!");

            var kq = ApDungHuyVe(l, soVe);
            if (!kq.ThanhCong) return kq;

            try
            {
                DataManager.Repository.Update(l);
            }
            catch (Exception ex)
            {
                DataManager.TaiLai();
                return new KetQua(false, "Could not save: " + ex.Message);
            }

            DataManager.TaiLai();
            return kq;
        }

        // =====================================================
        // SUGGESTION
        // =====================================================

        /// <summary>
        /// Earliest start time (5-minute steps) at or after <paramref name="tuLuc"/>, on the same day,
        /// at which the movie fits in the room. Returns null if none is left that day.
        /// </summary>
        public static DateTime? GoiYGioTrong(int maPhim, int maPhong, DateTime tuLuc, int boQuaMa = -1)
        {
            var phim = DataManager.DanhSachPhim.FirstOrDefault(p => p.MaPhim == maPhim);
            if (phim == null || phim.ThoiLuong <= 0) return null;

            DateTime batDau = CatGiay(tuLuc);
            DateTime min = CatGiay(Now()).AddMinutes(1);
            if (batDau < min) batDau = min;

            // Round up to the next multiple of 5 minutes
            int du = batDau.Minute % 5;
            if (du != 0) batDau = batDau.AddMinutes(5 - du);

            DateTime ngayKeTiep = tuLuc.Date.AddDays(1);
            for (; batDau < ngayKeTiep; batDau = batDau.AddMinutes(5))
            {
                var ketThuc = batDau.AddMinutes(phim.ThoiLuong);
                if (TimXungDot(DataManager.DanhSachLichChieu, maPhong, batDau, ketThuc, boQuaMa) == null)
                    return batDau;
            }
            return null;
        }
    }
}
