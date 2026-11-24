namespace RapPhim.Data
{
    /// <summary>
    /// Single composition root for storage configuration of the Showtime module.
    /// Change ONLY this file when switching from JSON files to the team's SQL Server database.
    /// </summary>
    public static class CauHinhLuuTru
    {
        /// <summary>
        /// Creates the showtime repository.
        /// Khi có DB của nhóm: đổi dòng này sang `new SqlLichChieuRepository(connStr)`.
        /// </summary>
        public static ILichChieuRepository TaoRepository() => new JsonLichChieuRepository();
    }
}
