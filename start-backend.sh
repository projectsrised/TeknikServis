#!/bin/bash

# Backend Başlatma Scripti
echo "🚀 Backend başlatılıyor..."
echo "📍 Dizin: TeknikServisApp/src/API"
echo "🌐 Port: http://localhost:5000"
echo "📚 Swagger: http://localhost:5000/swagger"
echo ""

cd "$(dirname "$0")/TeknikServisApp/src/API"

# .NET restore
echo "📦 NuGet paketleri yükleniyor..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "❌ NuGet paketleri yüklenirken hata oluştu!"
    exit 1
fi

# Backend'i başlat
echo "▶️  Backend başlatılıyor..."
echo ""
dotnet run
