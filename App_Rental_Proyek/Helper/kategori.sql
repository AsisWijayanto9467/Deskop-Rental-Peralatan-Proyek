-- =====================================================================
-- Database: db_rental_alat_proyek
-- Tambahan tabel untuk fitur Manajemen Kategori
-- Jalankan skrip ini setelah membuat database db_rental_alat_proyek
-- =====================================================================

-- --------------------------------------------------------
-- Table structure for table `kategori`
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `kategori` (
  `id` bigint UNSIGNED NOT NULL,
  `nama_kategori` varchar(255) NOT NULL,
  `deskripsi` text DEFAULT NULL,
  `status` enum('aktif','nonaktif') NOT NULL DEFAULT 'aktif',
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Table structure for table `alat_proyeks`
-- (dibutuhkan untuk menghitung jumlah alat per kategori
--  dan untuk aturan "hapus kategori hanya jika tidak digunakan")
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `alat_proyeks` (
  `id` bigint UNSIGNED NOT NULL,
  `kategori_id` bigint UNSIGNED DEFAULT NULL,
  `lokasi_id` bigint UNSIGNED DEFAULT NULL,
  `kode_alat` varchar(100) NOT NULL,
  `nama_alat` varchar(255) NOT NULL,
  `deskripsi` text DEFAULT NULL,
  `harga_sewa_harian` decimal(15,2) NOT NULL DEFAULT 0.00,
  `stok` int NOT NULL DEFAULT 0,
  `stok_tersedia` int NOT NULL DEFAULT 0,
  `kondisi` enum('baik','rusak_ringan','rusak_berat') NOT NULL DEFAULT 'baik',
  `status` enum('tersedia','disewa','maintenance','tidak_aktif') NOT NULL DEFAULT 'tersedia',
  `gambar` varchar(255) DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Indexes for table `kategori`
-- --------------------------------------------------------
ALTER TABLE `kategori`
  ADD PRIMARY KEY (`id`);

-- --------------------------------------------------------
-- Indexes for table `alat_proyeks`
-- --------------------------------------------------------
ALTER TABLE `alat_proyeks`
  ADD PRIMARY KEY (`id`),
  ADD KEY `alat_proyeks_kategori_id_index` (`kategori_id`),
  ADD KEY `alat_proyeks_lokasi_id_index` (`lokasi_id`);

-- --------------------------------------------------------
-- AUTO_INCREMENT
-- --------------------------------------------------------
ALTER TABLE `kategori`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

ALTER TABLE `alat_proyeks`
  MODIFY `id` bigint UNSIGNED NOT NULL AUTO_INCREMENT;

-- --------------------------------------------------------
-- Relasi foreign key (opsional, sesuaikan dengan tabel lokasi)
-- --------------------------------------------------------
ALTER TABLE `alat_proyeks`
  ADD CONSTRAINT `alat_proyeks_kategori_id_foreign`
      FOREIGN KEY (`kategori_id`) REFERENCES `kategori` (`id`) ON DELETE SET NULL;

-- --------------------------------------------------------
-- Data contoh kategori
-- --------------------------------------------------------
INSERT INTO `kategori` (`id`, `nama_kategori`, `deskripsi`, `status`, `created_at`, `updated_at`) VALUES
(1, 'Mesin', 'Kategori untuk mesin-mesin konstruksi dan pendukungnya', 'aktif', NOW(), NOW()),
(2, 'Alat Beton', 'Peralatan untuk pekerjaan beton', 'aktif', NOW(), NOW()),
(3, 'Alat Pemotong', 'Peralatan pemotong seperti gergaji, grinda, dll', 'aktif', NOW(), NOW()),
(4, 'Alat Pengangkut', 'Peralatan untuk mengangkut material', 'aktif', NOW(), NOW()),
(5, 'Alat Kelistrikan', 'Peralatan yang berhubungan dengan kelistrikan', 'aktif', NOW(), NOW()),
(6, 'Alat Keselamatan', 'Peralatan keselamatan kerja', 'aktif', NOW(), NOW());
