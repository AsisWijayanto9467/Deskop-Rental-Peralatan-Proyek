using System;

namespace App_Rental_Proyek.Model
{
    public class PengembalianModel
    {
        public ulong Id { get; set; }
        public ulong PenyewaanId { get; set; }
        public DateTime TanggalPengembalian { get; set; }
        public ulong? DiterimaOleh { get; set; }
        public string? KondisiAlat { get; set; }
        public string? Foto { get; set; }  // Tambahan: path/lokasi foto bukti pengembalian (nullable)
        public int TerlambatHari { get; set; } = 0;
        public string? Catatan { get; set; }
        public string Status { get; set; } = "diterima"; // diterima, perlu_perbaikan, ditolak
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}