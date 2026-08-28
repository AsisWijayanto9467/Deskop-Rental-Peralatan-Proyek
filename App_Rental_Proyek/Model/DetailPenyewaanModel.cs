using System;

namespace App_Rental_Proyek.Model
{
    public class DetailPenyewaanModel
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public ulong AlatId { get; set; }
        public int Jumlah { get; set; } = 1;
        public decimal HargaSewa { get; set; }
        public decimal Subtotal { get; set; }
        public string? KondisiSebelum { get; set; }
        public string? KondisiSesudah { get; set; }
        public string? Catatan { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // =====================
        // Properti untuk tampilan (hasil JOIN)
        // =====================
        public string KodeAlat { get; set; } = string.Empty;
        public string NamaAlat { get; set; } = string.Empty;
    }
}
