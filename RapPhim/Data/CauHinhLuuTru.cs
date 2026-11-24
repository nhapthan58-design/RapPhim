namespace RapPhim.Data
{
    /// <summary>
    /// Single composition root for storage &amp; provider configuration of the Showtime module.
    /// Change ONLY this file when switching from JSON/sample data to the team's SQL Server database.
    /// </summary>
    public static class CauHinhLuuTru
    {
        /// <summary>
        /// Creates the showtime repository.
        /// Khi có DB của nhóm: đổi dòng này sang `new SqlLichChieuRepository(connStr)`.
        /// </summary>
        public static ILichChieuRepository TaoRepository() => new JsonLichChieuRepository();

        /// <summary>
        /// Creates the read-only movie &amp; room catalog provider.
        /// Khi Mạnh có bảng Phim/Phòng thì đổi sang provider đọc DB.
        /// </summary>
        public static IDanhMucProvider TaoDanhMucProvider() => new MauDanhMucProvider();

        /// <summary>
        /// Creates the sold-ticket counter provider.
        /// Khi Khôi lưu ghế 'Đã bán' thì đổi sang provider đếm ghế đã bán theo suất.
        /// </summary>
        public static IVeDaBanProvider TaoVeDaBanProvider() => new BoDemVeDaBanProvider();
    }
}
