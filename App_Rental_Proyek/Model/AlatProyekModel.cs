using System;

namespace App_Rental_Proyek.Model
{
    public class AlatProyekModel
    {
        public ulong Id { get; set; }
        public ulong KategoriId { get; set; }
        public ulong LokasiId { get; set; }
        public string KodeAlat { get; set; } = string.Empty;
        public string NamaAlat { get; set; } = string.Empty;
        public string? Deskripsi { get; set; }
        public decimal HargaSewaHarian { get; set; }
        public int Stok { get; set; } = 0;
        public int StokTersedia { get; set; } = 0;
        public string Kondisi { get; set; } = "baik"; // baik, rusak_ringan, rusak_berat
        public string Status { get; set; } = "tersedia"; // tersedia, disewa, maintenance, tidak_aktif
        public string? Gambar { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
