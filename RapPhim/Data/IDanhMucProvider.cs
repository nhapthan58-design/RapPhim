using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Read-only provider for movies and screening rooms.
    /// Showtime scheduling ONLY reads catalog data; Mạnh's module owns movie/room management.
    /// </summary>
    public interface IDanhMucProvider
    {
        IReadOnlyList<Phim> LayDanhSachPhim();
        IReadOnlyList<PhongChieu> LayDanhSachPhong();
    }
}
