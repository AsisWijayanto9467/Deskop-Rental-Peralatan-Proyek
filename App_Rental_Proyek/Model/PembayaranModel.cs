using System;

namespace App_Rental_Proyek.Model
{
    public class PembayaranModel
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public string KodePembayaran { get; set; } = string.Empty;
        public DateTime TanggalPembayaran { get; set; }
        public decimal Jumlah { get; set; } = 0;
        public string MetodePembayaran { get; set; } = "cash";
        // cash, transfer, qris
        public string? BuktiPembayaran { get; set; }
        public string Status { get; set; } = "pending";
        // pending, diverifikasi, ditolak
        public ulong? DiverifikasiOleh { get; set; }
        public DateTime? TanggalVerifikasi { get; set; }
        public string? Catatan { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // =====================
        // Properti untuk tampilan (hasil JOIN)
        // =====================
        public string KodePenyewaan { get; set; } = string.Empty;
        public decimal TotalSewa { get; set; } = 0;
        public string StatusPenyewaan { get; set; } = string.Empty;
        public string NamaCustomer { get; set; } = string.Empty;
        public string EmailCustomer { get; set; } = string.Empty;
        public string NoTeleponCustomer { get; set; } = string.Empty;
        public string AlamatCustomer { get; set; } = string.Empty;
        public string NamaVerifikator { get; set; } = string.Empty;
    }
}