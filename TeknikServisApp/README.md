# TeknikServisApp

Multi-tenant telefon teknik servis ve satış yönetim sistemi - ASP.NET Core 8.0 Web API

## Özellikler

### Temel Özellikler
- **Multi-Tenant Mimari**: Birden fazla firma/tenant desteği
- **Multi-Bayi**: Her tenant altında birden fazla bayi/şube yönetimi
- **JWT Kimlik Doğrulama**: Güvenli API erişimi
- **Code-First EF Core**: PostgreSQL veritabanı
- **Swagger UI**: API dokümantasyonu

### Modüller
- 📱 **Ürün Yönetimi**: Kategori, ürün ve seri numarası takibi
- 🛒 **Satış Yönetimi**: POS tarzı satış, fatura, iade işlemleri
- 🔧 **Teknik Servis**: Cihaz kabul, onarım takibi, parça yönetimi
- 📦 **Stok Yönetimi**: Depo, transfer, sayım işlemleri
- 💰 **Kasa Yönetimi**: Gelir/gider, günlük ciro takibi
- 👥 **Personel Yönetimi**: Maaş, avans, izin, mesai takibi
- 📊 **Raporlama**: Satış, performans, stok raporları

## Kurulum

### Gereksinimler
- .NET 8.0 SDK
- PostgreSQL 14+
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Projeyi klonlayın**
```bash
git clone <repo-url>
cd TeknikServisApp
```

2. **Veritabanı bağlantısını ayarlayın**
`src/API/appsettings.json` dosyasında:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=teknikservis_db;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

3. **Migration oluşturun ve uygulayın**
```bash
cd src/API
dotnet ef migrations add InitialCreate --project ../Infrastructure
dotnet ef database update --project ../Infrastructure
```

4. **Seed data yükleyin**
```bash
psql -U postgres -d teknikservis_db -f scripts/seed_data.sql
```

5. **Uygulamayı çalıştırın**
```bash
dotnet run --project src/API
```

6. **Swagger UI'a erişin**
```
http://localhost:5000
```

## Demo Giriş Bilgileri
- **Email**: admin@demo.com
- **Şifre**: Admin123!

## API Endpoints

### Auth
- `POST /api/auth/login` - Giriş
- `POST /api/auth/refresh` - Token yenileme
- `POST /api/auth/change-password` - Şifre değiştirme
- `GET /api/auth/me` - Kullanıcı bilgisi

### Satış
- `GET /api/satis` - Satış listesi
- `POST /api/satis` - Yeni satış
- `POST /api/satis/validate-seri` - Seri numarası doğrulama
- `POST /api/satis/{id}/iptal` - Satış iptali

### Teknik Servis
- `GET /api/teknikservis` - Servis listesi
- `POST /api/teknikservis` - Yeni servis kaydı
- `PUT /api/teknikservis/{id}/durum` - Durum güncelleme
- `POST /api/teknikservis/{id}/parca` - Parça ekleme
- `POST /api/teknikservis/{id}/odeme` - Ödeme alma

### Stok
- `GET /api/stok/kritik` - Kritik stok ürünleri
- `POST /api/stok/giris` - Stok girişi
- `GET /api/stok/seri/{seriNo}` - Seri numarası sorgulama

### Raporlar
- `GET /api/rapor/ozet` - Özet rapor
- `GET /api/rapor/satis` - Satış raporu
- `GET /api/rapor/bayi-performans` - Bayi performans
- `GET /api/rapor/stok` - Stok raporu

## Proje Yapısı

```
TeknikServisApp/
├── src/
│   ├── Domain/           # Entity'ler, Enum'lar
│   ├── Application/      # DTO'lar, Interface'ler, Servisler
│   ├── Infrastructure/   # DbContext, Repository, UnitOfWork
│   └── API/              # Controller'lar, Middleware
├── scripts/
│   └── seed_data.sql     # Demo veriler
└── TeknikServisApp.sln
```

## Roller
1. **SuperAdmin** - Tüm yetkiler
2. **TenantAdmin** - Tenant yönetimi
3. **BayiAdmin** - Bayi yönetimi
4. **SatisSorumlusu** - Satış işlemleri
5. **TeknikServisPersoneli** - Servis işlemleri

## Lisans
MIT License
