using RapPhim.Models;

namespace RapPhim.Services
{
    /// <summary>
    /// Result of a scheduling operation.
    /// </summary>
    public record KetQua(bool ThanhCong, string ThongBao, LichChieu? LichXungDot = null);
}
