-- phpMyAdmin SQL Dump (format mengikuti helper yang sudah ada)
-- Database: `db_rental_alat_proyek`
--
-- Tabel denda untuk modul Manajemen Denda Admin.
-- Denda dihasilkan dari proses pengembalian (keterlambatan, kerusakan,
-- kehilangan, kekurangan komponen), bukan dibuat terpisah.
-- Jalankan skrip ini di database `db_rental_alat_proyek` sebelum
-- menggunakan halaman Denda di AdminDashboard.

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

-- --------------------------------------------------------

--
-- Table structure for table `denda`
--

CREATE TABLE `denda` (
  `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `penyewaan_id` bigint UNSIGNED NOT NULL,
  `pengembalian_id` bigint UNSIGNED DEFAULT NULL,
  `jenis_denda` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `jumlah` decimal(14,2) NOT NULL DEFAULT 0.00,
  `alasan` text COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pending',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL,

  PRIMARY KEY (`id`),
  KEY `denda_penyewaan_id_foreign` (`penyewaan_id`),
  KEY `denda_pengembalian_id_foreign` (`pengembalian_id`),

  CONSTRAINT `denda_penyewaan_id_foreign`
    FOREIGN KEY (`penyewaan_id`) REFERENCES `penyewaans` (`id`)
    ON DELETE CASCADE,

  CONSTRAINT `denda_pengembalian_id_foreign`
    FOREIGN KEY (`pengembalian_id`) REFERENCES `pengembalians` (`id`)
    ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;