#!/bin/bash

# Frontend Başlatma Scripti
echo "🚀 Frontend başlatılıyor..."
echo "📍 Dizin: TeknikServisfrontend"
echo "🌐 URL: http://localhost:3000"
echo ""

cd "$(dirname "$0")/TeknikServisfrontend"

# .env.local kontrolü
if [ ! -f .env.local ]; then
    echo "📝 .env.local dosyası oluşturuluyor..."
    echo "NEXT_PUBLIC_API_URL=http://localhost:5000" > .env.local
fi

# npm install kontrolü
if [ ! -d "node_modules" ]; then
    echo "📦 npm paketleri yükleniyor..."
    npm install
    
    if [ $? -ne 0 ]; then
        echo "❌ npm paketleri yüklenirken hata oluştu!"
        exit 1
    fi
fi

# Frontend'i başlat
echo "▶️  Frontend başlatılıyor..."
echo ""
npm run dev
