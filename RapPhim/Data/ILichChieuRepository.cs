using RapPhim.Models;

namespace RapPhim.Data
{
    /// <summary>
    /// Storage abstraction for showtimes.
    /// Swap the implementation (JSON file -> SQL Server) without touching any form or service code.
    /// </summary>
    public interface ILichChieuRepository
    {
        /// <summary>True if the store already contains saved data (false on first run).</summary>
        bool HasData();

        /// <summary>Load all showtimes. Must never throw for corrupted data; return an empty list instead.</summary>
        List<LichChieu> LoadAll();

        /// <summary>Replace the stored showtimes with the given list.</summary>
        void SaveAll(IEnumerable<LichChieu> items);
    }
}
