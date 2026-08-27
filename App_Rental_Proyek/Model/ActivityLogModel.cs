using System;

namespace App_Rental_Proyek.Model
{
    public class ActivityLogModel
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string Aktivitas { get; set; } = string.Empty;
        public string? Modul { get; set; }
        public ulong? ReferensiId { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
