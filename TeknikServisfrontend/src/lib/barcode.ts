import { SeriNumarasiDetay } from '@/types';

export const printBarcodeLabel = (kalem: SeriNumarasiDetay) => {
  const printWindow = window.open('', '_blank');
  if (!printWindow) return;

  const barcodeData = JSON.stringify({
    barkodNo: kalem.barkod || kalem.seriNo,
    fiyat: kalem.satisFiyati || kalem.alisFiyati || 0,
    stokId: kalem.id,
    durum: kalem.satildi ? 'Satıldı' : 'Stokta',
  });

  printWindow.document.write(`
    <!DOCTYPE html>
    <html>
      <head>
        <title>Karekod - ${kalem.urunAd}</title>
        <style>
          @media print {
            @page { margin: 0; size: 300px 70px; }
            body { margin: 0; padding: 0; }
          }
          body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            width: 300px;
            height: 70px;
            display: flex;
            border: 1px solid #000;
          }
          .barcode-container {
            width: 64px;
            height: 64px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-right: 1px solid #000;
            padding: 3px;
          }
          .barcode {
            width: 100%;
            height: 100%;
            object-fit: contain;
          }
          .info-container {
            flex: 1;
            padding: 4px 6px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
          }
          .urun-ad {
            font-weight: bold;
            font-size: 11px;
            line-height: 1.2;
          }
          .kategori-info {
            font-size: 9px;
            color: #666;
            margin-top: 2px;
          }
          .fiyat {
            font-size: 12px;
            font-weight: bold;
            margin-top: auto;
          }
        </style>
        <script src="https://cdn.jsdelivr.net/npm/qrcode@1.5.3/build/qrcode.min.js"></script>
      </head>
      <body>
        <div class="barcode-container">
          <canvas id="qrcode" class="barcode"></canvas>
        </div>
        <div class="info-container">
          <div>
            <div class="urun-ad">${kalem.urunAd || 'Ürün Adı'}</div>
            <div class="kategori-info">
              ${kalem.kategoriAd || ''} ${kalem.uretimTarihi ? '| ' + new Date(kalem.uretimTarihi).toLocaleDateString('tr-TR') : ''} ${kalem.mensei ? '| ' + kalem.mensei : ''}
            </div>
          </div>
          <div class="fiyat">${(kalem.satisFiyati || kalem.alisFiyati || 0).toFixed(2)} ₺</div>
        </div>
        <script>
          QRCode.toCanvas(document.getElementById('qrcode'), '${barcodeData}', {
            width: 60,
            margin: 1
          }, function (error) {
            if (error) console.error(error);
          });
        </script>
      </body>
    </html>
  `);
  printWindow.document.close();
  setTimeout(() => {
    printWindow.print();
  }, 500);
};

export const printMultipleBarcodeLabels = (kalemler: SeriNumarasiDetay[]) => {
  const printWindow = window.open('', '_blank');
  if (!printWindow) return;

  let htmlContent = `
    <!DOCTYPE html>
    <html>
      <head>
        <title>Karekodlar - ${kalemler.length} Adet</title>
        <style>
          @media print {
            @page { margin: 0; size: 300px 70px; }
            body { margin: 0; padding: 0; }
            .label { page-break-after: always; }
            .label:last-child { page-break-after: auto; }
          }
          body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
          }
          .label {
            width: 300px;
            height: 70px;
            display: flex;
            border: 1px solid #000;
            margin-bottom: 0;
          }
          .barcode-container {
            width: 64px;
            height: 64px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-right: 1px solid #000;
            padding: 3px;
          }
          .barcode {
            width: 100%;
            height: 100%;
            object-fit: contain;
          }
          .info-container {
            flex: 1;
            padding: 4px 6px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
          }
          .urun-ad {
            font-weight: bold;
            font-size: 11px;
            line-height: 1.2;
          }
          .kategori-info {
            font-size: 9px;
            color: #666;
            margin-top: 2px;
          }
          .fiyat {
            font-size: 12px;
            font-weight: bold;
            margin-top: auto;
          }
        </style>
        <script src="https://cdn.jsdelivr.net/npm/qrcode@1.5.3/build/qrcode.min.js"></script>
      </head>
      <body>
  `;

  kalemler.forEach((kalem, index) => {
    const barcodeData = JSON.stringify({
      barkodNo: kalem.barkod || kalem.seriNo,
      fiyat: kalem.satisFiyati || kalem.alisFiyati || 0,
      stokId: kalem.id,
      durum: kalem.satildi ? 'Satıldı' : 'Stokta',
    });

    htmlContent += `
      <div class="label">
        <div class="barcode-container">
          <canvas id="qrcode-${index}" class="barcode"></canvas>
        </div>
        <div class="info-container">
          <div>
            <div class="urun-ad">${kalem.urunAd || 'Ürün Adı'}</div>
            <div class="kategori-info">
              ${kalem.kategoriAd || ''} ${kalem.uretimTarihi ? '| ' + new Date(kalem.uretimTarihi).toLocaleDateString('tr-TR') : ''} ${kalem.mensei ? '| ' + kalem.mensei : ''}
            </div>
          </div>
          <div class="fiyat">${(kalem.satisFiyati || kalem.alisFiyati || 0).toFixed(2)} ₺</div>
        </div>
      </div>
      <script>
        QRCode.toCanvas(document.getElementById('qrcode-${index}'), '${barcodeData}', {
          width: 60,
          margin: 1
        }, function (error) {
          if (error) console.error(error);
        });
      </script>
    `;
  });

  htmlContent += `
      </body>
    </html>
  `;

  printWindow.document.write(htmlContent);
  printWindow.document.close();
  setTimeout(() => {
    printWindow.print();
  }, 1000);
};
