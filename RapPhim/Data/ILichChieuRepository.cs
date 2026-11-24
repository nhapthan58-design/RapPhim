using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Per-record storage abstraction for showtimes.
    /// Swap the implementation (JSON file -> SQL Server) in <see cref="CauHinhLuuTru"/> without touching forms or services.
    /// </summary>
    public interface ILichChieuRepository
    {
        /// <summary>True if the store has already been initialized (false on first run).</summary>
        bool HasData();

        /// <summary>Load all showtimes. Must never throw for corrupted data; return an empty list instead.</summary>
        List<LichChieu> LoadAll();

        /// <summary>Insert a single showtime. Repository allocates <see cref="LichChieu.MaLichChieu"/> (JSON: max+1, SQL: IDENTITY) and returns the stored record.</summary>
        LichChieu Insert(LichChieu item);

        /// <summary>Update an existing showtime. Throws <see cref="KeyNotFoundException"/> if not found.</summary>
        void Update(LichChieu item);

        /// <summary>Delete a showtime by id. Missing id is a safe no-op.</summary>
        void Delete(int maLichChieu);
    }
}
