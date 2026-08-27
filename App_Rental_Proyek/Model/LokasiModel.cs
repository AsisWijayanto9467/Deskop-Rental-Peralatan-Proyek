using System;

namespace App_Rental_Proyek.Model
{
    public class LokasiModel
    {
        public ulong Id { get; set; }
        public string NamaLokasi { get; set; } = string.Empty;
        public string Alamat { get; set; } = string.Empty;
        public string? Keterangan { get; set; }
        public string Status { get; set; } = "aktif"; // aktif, nonaktif
        public int JumlahAlat { get; set; } = 0;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
