-- TeknikServisApp Seed Data
-- Run after migrations

-- Demo Tenant
INSERT INTO "Tenants" ("Id", "Ad", "Kod", "VergiNo", "Adres", "Telefon", "Email", "Aktif", "OlusturmaTarihi", "Silindi")
VALUES 
('11111111-1111-1111-1111-111111111111', 'Demo Firma', 'DEMO001', '1234567890', 'İstanbul, Türkiye', '02121234567', 'info@demofirma.com', true, NOW(), false);

-- Demo Bayi (Merkez)
INSERT INTO "Bayiler" ("Id", "TenantId", "Ad", "Kod", "Adres", "Il", "Ilce", "Telefon", "YetkiliAd", "Aktif", "MerkezMi", "OlusturmaTarihi", "Silindi")
VALUES 
('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', 'Merkez Şube', 'MRK001', 'Kadıköy, İstanbul', 'İstanbul', 'Kadıköy', '02161234567', 'Ahmet Yılmaz', true, true, NOW(), false);

-- Demo Kullanıcı (Admin) - Şifre: Admin123!
INSERT INTO "Kullanicilar" ("Id", "TenantId", "BayiId", "Ad", "Soyad", "Email", "SifreHash", "Telefon", "Rol", "Aktif", "YillikIzinHakki", "OlusturmaTarihi", "Silindi")
VALUES 
('33333333-3333-3333-3333-333333333333', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', 'Admin', 'Kullanıcı', 'admin@demo.com', '$2a$11$rKN/VVqXxXfZh6H3KeL0l.XQXK0pCL3gVH7bvK0EkGNJ6pIGLjYJu', '05301234567', 2, true, 14, NOW(), false);

-- Demo Depo
INSERT INTO "Depolar" ("Id", "TenantId", "BayiId", "Ad", "Kod", "MerkezDepoMu", "Adres", "Aktif", "OlusturmaTarihi", "Silindi")
VALUES 
('44444444-4444-4444-4444-444444444444', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', 'Merkez Depo', 'DEP001', true, 'Kadıköy, İstanbul', true, NOW(), false);

-- Demo Kasa
INSERT INTO "Kasalar" ("Id", "TenantId", "BayiId", "Ad", "Bakiye", "Aktif", "OlusturmaTarihi", "Silindi")
VALUES 
('55555555-5555-5555-5555-555555555555', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', 'Ana Kasa', 10000.00, true, NOW(), false);

-- Kategoriler
INSERT INTO "Kategoriler" ("Id", "TenantId", "Ad", "Kod", "Tip", "Sira", "Aktif", "OlusturmaTarihi", "Silindi")
VALUES 
('66666666-6666-6666-6666-666666666661', '11111111-1111-1111-1111-111111111111', 'Telefonlar', 'TEL', 1, 1, true, NOW(), false),
('66666666-6666-6666-6666-666666666662', '11111111-1111-1111-1111-111111111111', 'Tabletler', 'TAB', 2, 2, true, NOW(), false),
('66666666-6666-6666-6666-666666666663', '11111111-1111-1111-1111-111111111111', 'Aksesuarlar', 'AKS', 3, 3, true, NOW(), false),
('66666666-6666-6666-6666-666666666664', '11111111-1111-1111-1111-111111111111', 'Yedek Parçalar', 'YPC', 4, 4, true, NOW(), false);

-- Demo Ürünler
INSERT INTO "Urunler" ("Id", "TenantId", "KategoriId", "Ad", "Kod", "Barkod", "Marka", "Model", "AlisFiyat", "SatisFiyat", "KdvOran", "KritikStok", "SeriTakipli", "Aktif", "OlusturmaTarihi", "Silindi")
VALUES 
('77777777-7777-7777-7777-777777777771', '11111111-1111-1111-1111-111111111111', '66666666-6666-6666-6666-666666666661', 'iPhone 15 Pro 256GB', 'IPH15P256', '8901234567890', 'Apple', 'iPhone 15 Pro', 45000.00, 55000.00, 20, 5, true, true, NOW(), false),
('77777777-7777-7777-7777-777777777772', '11111111-1111-1111-1111-111111111111', '66666666-6666-6666-6666-666666666661', 'Samsung S24 Ultra 512GB', 'SMS24U512', '8901234567891', 'Samsung', 'Galaxy S24 Ultra', 50000.00, 62000.00, 20, 5, true, true, NOW(), false),
('77777777-7777-7777-7777-777777777773', '11111111-1111-1111-1111-111111111111', '66666666-6666-6666-6666-666666666663', 'USB-C Şarj Kablosu', 'USBC001', '8901234567892', 'Generic', 'USB-C 1m', 50.00, 100.00, 20, 20, false, true, NOW(), false),
('77777777-7777-7777-7777-777777777774', '11111111-1111-1111-1111-111111111111', '66666666-6666-6666-6666-666666666664', 'iPhone Ekran (Original)', 'IPHEKR001', '8901234567893', 'Apple', 'iPhone 15 Screen', 5000.00, 7500.00, 20, 3, true, true, NOW(), false);

-- Demo Seri Numaraları
INSERT INTO "SeriNumaralari" ("Id", "TenantId", "BayiId", "UrunId", "DepoId", "SeriNo", "AlisFiyati", "SatisFiyati", "Satildi", "GirisTarihi", "OlusturmaTarihi", "Silindi")
VALUES 
('88888888-8888-8888-8888-888888888881', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', '77777777-7777-7777-7777-777777777771', '44444444-4444-4444-4444-444444444444', 'IMEI001234567890', 45000.00, 55000.00, false, NOW(), NOW(), false),
('88888888-8888-8888-8888-888888888882', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', '77777777-7777-7777-7777-777777777771', '44444444-4444-4444-4444-444444444444', 'IMEI001234567891', 45000.00, 55000.00, false, NOW(), NOW(), false),
('88888888-8888-8888-8888-888888888883', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', '77777777-7777-7777-7777-777777777772', '44444444-4444-4444-4444-444444444444', 'IMEI002234567890', 50000.00, 62000.00, false, NOW(), NOW(), false),
('88888888-8888-8888-8888-888888888884', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', '77777777-7777-7777-7777-777777777774', '44444444-4444-4444-4444-444444444444', 'EKR001234567890', 5000.00, 7500.00, false, NOW(), NOW(), false);

-- Demo Müşteri
INSERT INTO "Musteriler" ("Id", "TenantId", "BayiId", "Ad", "Soyad", "Telefon", "Email", "Adres", "Kurumsal", "OlusturmaTarihi", "Silindi")
VALUES 
('99999999-9999-9999-9999-999999999991', '11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', 'Mehmet', 'Demir', '05321234567', 'mehmet@email.com', 'Ataşehir, İstanbul', false, NOW(), false);

-- ====================================
-- KULLANIM TALİMATLARI
-- ====================================
-- 1. PostgreSQL veritabanı oluşturun: CREATE DATABASE teknikservis_db;
-- 2. Migration uygulayın: dotnet ef database update --project src/Infrastructure --startup-project src/API
-- 3. Bu SQL dosyasını çalıştırın
-- 4. Giriş bilgileri: admin@demo.com / Admin123!
