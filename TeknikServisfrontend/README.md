# TeknikServis Frontend

Next.js 14 ile geliştirilmiş, LinkedIn tarzı kurumsal tasarıma sahip telefon teknik servis yönetim paneli.

## Teknolojiler

- **Next.js 14** - React framework
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Styling
- **Zustand** - State management
- **React Hook Form** - Form handling
- **Axios** - HTTP client
- **Recharts** - Charts & graphs
- **Lucide React** - Icons

## Kurulum

### 1. Bağımlılıkları Yükle

```bash
cd frontend
npm install
```

### 2. Ortam Değişkenlerini Ayarla

`.env.local` dosyası oluştur:

```env
NEXT_PUBLIC_API_URL=http://localhost:5000
```

### 3. Geliştirme Sunucusunu Başlat

```bash
npm run dev
```

Tarayıcıda aç: http://localhost:3000

### 4. Production Build

```bash
npm run build
npm start
```

## Proje Yapısı

```
frontend/
├── src/
│   ├── app/                    # Next.js App Router
│   │   ├── dashboard/          # Dashboard sayfaları
│   │   │   ├── bayiler/
│   │   │   ├── depolar/
│   │   │   ├── musteriler/
│   │   │   ├── kategoriler/
│   │   │   ├── urunler/
│   │   │   ├── stok/
│   │   │   ├── satislar/
│   │   │   ├── teknik-servis/
│   │   │   ├── kasa/
│   │   │   ├── personel/
│   │   │   └── raporlar/
│   │   ├── layout.tsx
│   │   ├── page.tsx            # Login sayfası
│   │   └── globals.css
│   ├── components/
│   │   ├── layout/             # Sidebar, Header
│   │   └── ui/                 # Reusable UI components
│   ├── lib/
│   │   └── api.ts              # API client
│   ├── store/
│   │   └── authStore.ts        # Zustand auth store
│   └── types/
│       └── index.ts            # TypeScript types
├── package.json
├── tailwind.config.js
├── tsconfig.json
└── next.config.js
```

## Özellikler

### Kimlik Doğrulama
- JWT tabanlı authentication
- Otomatik token yenileme
- Persist login state

### Dashboard
- Özet istatistikler
- Grafikler (Line, Bar, Pie)
- Son satışlar ve servisler
- Hızlı işlem butonları

### Modüller

| Modül | Özellikler |
|-------|------------|
| Bayiler | CRUD işlemleri, merkez/şube yönetimi |
| Depolar | Depo yönetimi, stok lokasyonları |
| Müşteriler | Müşteri kaydı, satış/servis geçmişi |
| Kategoriler | Ürün kategorileri, hiyerarşik yapı |
| Ürünler | Ürün kataloğu, barkod, fiyatlandırma |
| Stok | Stok takibi, kritik stok uyarıları |
| Satışlar | POS sistemi, barkod okutma |
| Teknik Servis | Servis kaydı, durum takibi |
| Kasa | Giriş/çıkış, hareket geçmişi |
| Personel | Kullanıcı yönetimi, rol bazlı erişim |
| Raporlar | Satış analizi, performans grafikleri |

## Demo Bilgileri

```
E-posta: admin@demo.com
Şifre: Admin123!
```

## API Bağlantısı

Frontend, backend API'ye `http://localhost:5000` üzerinden bağlanır.

API endpoint'leri `src/lib/api.ts` dosyasında tanımlıdır.

## Lisans

MIT
