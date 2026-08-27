using System;

namespace App_Rental_Proyek.Model
{
    public class PenyewaanModel
    {
        public ulong Id { get; set; }
        public string KodePenyewaan { get; set; } = string.Empty;
        public ulong UserId { get; set; }
        public DateTime TanggalPengajuan { get; set; }
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalSelesai { get; set; }
        public int TotalHari { get; set; }
        public decimal Subtotal { get; set; } = 0;
        public decimal Denda { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string Status { get; set; } = "pending";
        // pending, disetujui, ditolak, menunggu_pembayaran, dibayar, sedang_disewa, selesai, dibatalkan
        public string? Catatan { get; set; }
        public ulong? ProcessedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
