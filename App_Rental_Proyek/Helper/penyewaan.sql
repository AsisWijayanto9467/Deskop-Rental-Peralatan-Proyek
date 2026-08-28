-- phpMyAdmin SQL Dump (format mengikuti helper yang sudah ada)
-- Database: `db_rental_alat_proyek`
--
-- Tabel penyewaan untuk modul Manajemen Penyewaan Admin.
-- Jalankan skrip ini di database `db_rental_alat_proyek` sebelum
-- menggunakan halaman Penyewaan di AdminDashboard.

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

-- --------------------------------------------------------

--
-- Table structure for table `penyewaans`
--

CREATE TABLE `penyewaans` (
  `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `kode_penyewaan` varchar(255) NOT NULL,
  `user_id` bigint UNSIGNED NOT NULL,
  `tanggal_pengajuan` date NOT NULL,
  `tanggal_mulai` date NOT NULL,
  `tanggal_selesai` date NOT NULL,
  `total_hari` int NOT NULL,
  `subtotal` decimal(14,2) NOT NULL DEFAULT 0.00,
  `denda` decimal(14,2) NOT NULL DEFAULT 0.00,
  `total` decimal(14,2) NOT NULL DEFAULT 0.00,
  `status` enum(
    'pending',
    'disetujui',
    'ditolak',
    'menunggu_pembayaran',
    'dibayar',
    'sedang_disewa',
    'selesai',
    'dibatalkan'
  ) NOT NULL DEFAULT 'pending',
  `catatan` text DEFAULT NULL,
  `processed_by` bigint UNSIGNED DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `penyewaans_kode_penyewaan_unique` (`kode_penyewaan`),
  KEY `penyewaans_user_id_foreign` (`user_id`),
  KEY `penyewaans_processed_by_foreign` (`processed_by`),

  CONSTRAINT `penyewaans_user_id_foreign`
    FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
    ON DELETE CASCADE,

  CONSTRAINT `penyewaans_processed_by_foreign`
    FOREIGN KEY (`processed_by`) REFERENCES `users` (`id`)
    ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `detail_penyewaans`
--

CREATE TABLE `detail_penyewaans` (
  `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `penyewaan_id` bigint UNSIGNED NOT NULL,
  `alat_id` bigint UNSIGNED NOT NULL,
  `jumlah` int NOT NULL DEFAULT 1,
  `harga_sewa` decimal(14,2) NOT NULL DEFAULT 0.00,
  `subtotal` decimal(14,2) NOT NULL DEFAULT 0.00,
  `kondisi_sebelum` varchar(100) DEFAULT NULL,
  `kondisi_sesudah` varchar(100) DEFAULT NULL,
  `catatan` text DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,

  PRIMARY KEY (`id`),
  KEY `detail_penyewaans_penyewaan_id_foreign` (`penyewaan_id`),
  KEY `detail_penyewaans_alat_id_foreign` (`alat_id`),

  CONSTRAINT `detail_penyewaans_penyewaan_id_foreign`
    FOREIGN KEY (`penyewaan_id`) REFERENCES `penyewaans` (`id`)
    ON DELETE CASCADE,

  CONSTRAINT `detail_penyewaans_alat_id_foreign`
    FOREIGN KEY (`alat_id`) REFERENCES `alat_proyeks` (`id`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;