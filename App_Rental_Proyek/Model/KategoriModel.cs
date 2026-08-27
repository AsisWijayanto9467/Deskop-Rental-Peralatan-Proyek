using System;

namespace App_Rental_Proyek.Model
{
    public class KategoriModel
    {
        public ulong Id { get; set; }
        public string NamaKategori { get; set; } = string.Empty;
        public string? Deskripsi { get; set; }
        public string Status { get; set; } = "aktif"; // aktif, nonaktif
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
