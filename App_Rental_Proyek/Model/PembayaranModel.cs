using System;

namespace App_Rental_Proyek.Model
{
    public class PembayaranModel
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public string KodePembayaran { get; set; } = string.Empty;
        public DateTime TanggalPembayaran { get; set; }
        public decimal Jumlah { get; set; }
        public string MetodePembayaran { get; set; } = string.Empty; // cash, transfer, qris
        public string? BuktiPembayaran { get; set; }
        public string Status { get; set; } = "pending"; // pending, diverifikasi, ditolak
        public ulong? DiverifikasiOleh { get; set; }
        public DateTime? TanggalVerifikasi { get; set; }
        public string? Catatan { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
