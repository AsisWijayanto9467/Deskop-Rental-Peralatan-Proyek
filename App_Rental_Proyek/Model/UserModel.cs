using System;

namespace App_Rental_Proyek.Model
{
    public class UserModel
    {
        public ulong Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? NoTelepon { get; set; }
        public string? Alamat { get; set; }
        public string Role { get; set; } = "user"; // admin, petugas, user
        public string Status { get; set; } = "aktif"; // aktif, nonaktif
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
