using System;

namespace App_Rental_Proyek.Model
{
    public class DendaModel
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public ulong? PengembalianId { get; set; }
        public string JenisDenda { get; set; } = string.Empty; // terlambat, kerusakan, kehilangan
        public decimal Jumlah { get; set; }
        public string Alasan { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // pending, dibayar, ditangguhkan
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
