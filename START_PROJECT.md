# Projeyi Ayağa Kaldırma Rehberi

## Ön Gereksinimler

- ✅ .NET 8.0 SDK (Kurulu: 8.0.417)
- ✅ Node.js v24.13.0 (Kurulu)
- ✅ npm 11.6.2 (Kurulu)
- ✅ PostgreSQL (Uzak DB: 45.67.203.207)

## Database Bağlantısı

Uzak PostgreSQL veritabanı kullanılıyor:
- **Host**: 45.67.203.207
- **Port**: 5432
- **Database**: teknikservis_db
- **Username**: postgres
- **Password**: 12121212aA!!

**Not**: Local'de herhangi bir database kurulumu gerekmez.

## Adım 1: Frontend Environment Dosyası

Frontend klasöründe `.env.local` dosyası oluşturun:

```bash
cd TeknikServisfrontend
echo "NEXT_PUBLIC_API_URL=http://localhost:5000" > .env.local
```

## Adım 2: Backend Bağımlılıklarını Yükle

```bash
cd TeknikServisApp/src/API
dotnet restore
```

## Adım 3: Frontend Bağımlılıklarını Yükle

```bash
cd TeknikServisfrontend
npm install
```

## Adım 4: Backend'i Başlat

```bash
cd TeknikServisApp/src/API
dotnet run
```

Backend şu adreste çalışacak:
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

**Not**: İlk çalıştırmada Development ortamında otomatik migration yapılacak (Program.cs:48-53).

## Adım 5: Frontend'i Başlat (Yeni Terminal)

```bash
cd TeknikServisfrontend
npm run dev
```

Frontend şu adreste çalışacak:
- **Frontend**: http://localhost:3000

## Demo Giriş Bilgileri

- **Email**: admin@demo.com
- **Şifre**: Admin123!

## Sorun Giderme

### Backend başlamıyorsa:
1. Database bağlantısını kontrol edin (appsettings.json)
2. Port 5000'in kullanılabilir olduğundan emin olun
3. Migration hataları varsa logları kontrol edin

### Frontend başlamıyorsa:
1. `.env.local` dosyasının oluşturulduğundan emin olun
2. Port 3000'in kullanılabilir olduğundan emin olun
3. `npm install` komutunun başarıyla tamamlandığından emin olun

### Database bağlantı hatası:
1. Uzak database'in erişilebilir olduğundan emin olun
2. Firewall ayarlarını kontrol edin
3. Database kullanıcı adı ve şifresini doğrulayın

## Hızlı Başlatma (Tek Komut)

Backend ve Frontend'i ayrı terminallerde başlatın:

**Terminal 1 (Backend):**
```bash
cd TeknikServisApp/src/API && dotnet run
```

**Terminal 2 (Frontend):**
```bash
cd TeknikServisfrontend && npm run dev
```

## Proje Yapısı

```
TeknikServis/
├── TeknikServisApp/          # Backend (.NET 8.0)
│   └── src/
│       ├── API/              # API Katmanı
│       ├── Application/      # Business Logic
│       ├── Domain/           # Entity'ler
│       └── Infrastructure/   # Data Access
└── TeknikServisfrontend/     # Frontend (Next.js 14)
    └── src/
        ├── app/              # Pages
        ├── components/       # React Components
        └── lib/              # Utilities
```

## API Endpoints

- **Auth**: `/api/auth/login`, `/api/auth/me`
- **Swagger**: `/swagger`
- **Tüm API'ler**: `/api/*`

## Notlar

- Database migration'lar otomatik çalışır (Development ortamında)
- CORS tüm origin'lere açık (Development için)
- JWT authentication aktif
- Swagger UI her zaman erişilebilir
