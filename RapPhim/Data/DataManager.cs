using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Central data manager — stores movies, rooms, and schedules.
    /// Uses in-memory sample data (can be replaced with a database later).
    /// </summary>
    public static class DataManager
    {
        private static int _nextLichChieuId = 1;

        static DataManager()
        {
            TaoDuLieuMau();
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

        /// <summary>
        /// Check if a new showtime conflicts with existing ones in the same room.
        /// 
        /// CONFLICT DETECTION LOGIC:
        /// Two showtimes overlap when: new.start < old.end AND new.end > old.start
        /// (same room only)
        /// 
        /// Adds 15-minute buffer between showtimes for room cleanup.
        /// </summary>
        public static LichChieu? KiemTraTrungGio(int maPhong, DateTime gioBatDau, DateTime gioKetThuc, int maLichChieuHienTai = -1)
        {
            const int BUFFER_MINUTES = 15; // 15-min gap between showtimes for cleanup

            foreach (var existing in DanhSachLichChieu)
            {
                // Skip the showtime being edited
                if (existing.MaLichChieu == maLichChieuHienTai)
                    continue;

                // Only check same room
                if (existing.MaPhong != maPhong)
                    continue;

                // Add 15-min buffer after each showtime
                DateTime existingEndWithBuffer = existing.GioKetThuc.AddMinutes(BUFFER_MINUTES);
                DateTime newEndWithBuffer = gioKetThuc.AddMinutes(BUFFER_MINUTES);

                // Check time overlap:
                // New starts before existing ends (+ buffer)
                // AND new ends (+ buffer) after existing starts
                bool hasConflict = gioBatDau < existingEndWithBuffer && newEndWithBuffer > existing.GioBatDau;

                if (hasConflict)
                {
                    return existing; // Return conflicting showtime
                }
            }

            return null; // No conflict
        }

        /// <summary>
        /// Add a new showtime. Returns success status and message.
        /// </summary>
        public static (bool Success, string Message, LichChieu? Conflict) ThemLichChieu(int maPhim, int maPhong, DateTime gioBatDau)
        {
            var phim = DanhSachPhim.FirstOrDefault(p => p.MaPhim == maPhim);
            var phong = DanhSachPhongChieu.FirstOrDefault(p => p.MaPhong == maPhong);

            if (phim == null) return (false, "Movie not found!", null);
            if (phong == null) return (false, "Room not found!", null);

            DateTime gioKetThuc = gioBatDau.AddMinutes(phim.ThoiLuong);

            // *** CHECK FOR TIME CONFLICT ***
            var conflict = KiemTraTrungGio(maPhong, gioBatDau, gioKetThuc);
            if (conflict != null)
            {
                string msg = $"TIME CONFLICT!\n\n" +
                    $"\"{phong.TenPhong}\" already has a showtime:\n" +
                    $"  Movie: {conflict.TenPhim}\n" +
                    $"  From: {conflict.GioBatDau:HH:mm dd/MM/yyyy}\n" +
                    $"  To: {conflict.GioKetThuc:HH:mm dd/MM/yyyy}\n\n" +
                    $"(Minimum 15-minute gap required between showtimes)";
                return (false, msg, conflict);
            }

            var newSchedule = new LichChieu
            {
                MaLichChieu = _nextLichChieuId++,
                MaPhim = maPhim,
                MaPhong = maPhong,
                GioBatDau = gioBatDau,
                GioKetThuc = gioKetThuc,
                SoVeDaBan = 0,
                TenPhim = phim.TenPhim,
                TenPhong = phong.TenPhong,
                ThoiLuongPhim = phim.ThoiLuong
            };

            DanhSachLichChieu.Add(newSchedule);
            return (true, $"Showtime scheduled successfully!\n\nMovie: {phim.TenPhim}\nRoom: {phong.TenPhong}\nTime: {gioBatDau:HH:mm} - {gioKetThuc:HH:mm dd/MM/yyyy}", null);
        }

        /// <summary>
        /// Update a showtime (only allowed if no tickets sold).
        /// </summary>
        public static (bool Success, string Message) CapNhatLichChieu(int maLichChieu, int maPhim, int maPhong, DateTime gioBatDau)
        {
            var lichChieu = DanhSachLichChieu.FirstOrDefault(l => l.MaLichChieu == maLichChieu);
            if (lichChieu == null) return (false, "Showtime not found!");
            if (lichChieu.SoVeDaBan > 0) return (false, "Cannot edit! Tickets have already been sold for this showtime.");

            var phim = DanhSachPhim.FirstOrDefault(p => p.MaPhim == maPhim);
            var phong = DanhSachPhongChieu.FirstOrDefault(p => p.MaPhong == maPhong);

            if (phim == null) return (false, "Movie not found!");
            if (phong == null) return (false, "Room not found!");

            DateTime gioKetThuc = gioBatDau.AddMinutes(phim.ThoiLuong);

            // Check conflict (skip current showtime being edited)
            var conflict = KiemTraTrungGio(maPhong, gioBatDau, gioKetThuc, maLichChieu);
            if (conflict != null)
            {
                return (false, $"TIME CONFLICT with: {conflict.TenPhim} ({conflict.GioBatDau:HH:mm} - {conflict.GioKetThuc:HH:mm})");
            }

            lichChieu.MaPhim = maPhim;
            lichChieu.MaPhong = maPhong;
            lichChieu.GioBatDau = gioBatDau;
            lichChieu.GioKetThuc = gioKetThuc;
            lichChieu.TenPhim = phim.TenPhim;
            lichChieu.TenPhong = phong.TenPhong;
            lichChieu.ThoiLuongPhim = phim.ThoiLuong;

            return (true, "Showtime updated successfully!");
        }

        /// <summary>
        /// Delete a showtime (only allowed if no tickets sold).
        /// </summary>
        public static (bool Success, string Message) XoaLichChieu(int maLichChieu)
        {
            var lichChieu = DanhSachLichChieu.FirstOrDefault(l => l.MaLichChieu == maLichChieu);
            if (lichChieu == null) return (false, "Showtime not found!");
            if (lichChieu.SoVeDaBan > 0) return (false, "Cannot delete! Tickets have already been sold for this showtime.");

            DanhSachLichChieu.Remove(lichChieu);
            return (true, "Showtime deleted successfully!");
        }

        /// <summary>
        /// Create sample data for demo purposes.
        /// </summary>
        public static void TaoDuLieuMau()
        {
            if (DanhSachLichChieu.Count > 0) return;

            var today = DateTime.Today;

            ThemLichChieu(1, 1, today.AddHours(9));
            ThemLichChieu(2, 1, today.AddHours(13));
            ThemLichChieu(3, 2, today.AddHours(10));
            ThemLichChieu(4, 3, today.AddHours(14));
            ThemLichChieu(5, 2, today.AddHours(15));

            // Simulate one showtime with tickets sold (cannot edit/delete)
            var soldShowtime = DanhSachLichChieu.FirstOrDefault(l => l.MaPhim == 1);
            if (soldShowtime != null) soldShowtime.SoVeDaBan = 25;
        }
    }
}
