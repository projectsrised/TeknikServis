'use client';

import { useState, useRef, useEffect } from 'react';
import { Scan, Package, CheckCircle, XCircle } from 'lucide-react';
import { Card, Badge } from '@/components/ui';
import { stokApi } from '@/lib/api';
import { SeriNumarasiDetay } from '@/types';
import toast from 'react-hot-toast';

export default function KarekodOkutPage() {
  const [seriNo, setSeriNo] = useState('');
  const [stokBilgi, setStokBilgi] = useState<SeriNumarasiDetay | null>(null);
  const [loading, setLoading] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  const handleScan = async (value: string) => {
    if (!value.trim()) return;

    setLoading(true);
    try {
      const response = await stokApi.getBySeriNo(value.trim());
      if (response.data) {
        setStokBilgi(response.data);
        toast.success('Stok bilgisi bulundu');
      } else {
        setStokBilgi(null);
        toast.error('Stok bulunamadı');
      }
    } catch (error: any) {
      setStokBilgi(null);
      toast.error(error.response?.data?.mesaj || 'Stok bilgisi alınamadı');
    } finally {
      setLoading(false);
      setSeriNo('');
      setTimeout(() => inputRef.current?.focus(), 100);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter') {
      handleScan(seriNo);
    }
  };

  const parseBarcodeData = (barcodeData: string) => {
    try {
      const data = JSON.parse(barcodeData);
      return {
        barkodNo: data.barkodNo,
        fiyat: data.fiyat,
        stokId: data.stokId,
        durum: data.durum,
      };
    } catch {
      return null;
    }
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-secondary-900">Karekod Okut</h1>
        <p className="text-secondary-500">Karekod veya seri numarası okutarak stok bilgisi görüntüleyin</p>
      </div>

      <Card>
        <div className="p-6 space-y-6">
          {/* Scanner Input */}
          <div className="space-y-2">
            <label className="label flex items-center gap-2">
              <Scan className="w-4 h-4" />
              Seri No / Karekod
            </label>
            <input
              ref={inputRef}
              type="text"
              value={seriNo}
              onChange={(e) => setSeriNo(e.target.value)}
              onKeyPress={handleKeyPress}
              placeholder="Seri numarası girin veya karekod okutun..."
              className="input text-lg font-mono"
              autoFocus
            />
            <button
              onClick={() => handleScan(seriNo)}
              disabled={loading || !seriNo.trim()}
              className="btn-primary w-full"
            >
              {loading ? 'Aranıyor...' : 'Ara'}
            </button>
          </div>

          {/* Stok Bilgisi */}
          {stokBilgi && (
            <div className="border-2 border-primary-200 rounded-lg p-6 bg-primary-50">
              <div className="flex items-start gap-4">
                <div className="w-16 h-16 bg-white rounded-lg flex items-center justify-center border-2 border-primary-200">
                  <Package className="w-8 h-8 text-primary-600" />
                </div>
                <div className="flex-1 space-y-3">
                  <div>
                    <h3 className="text-lg font-bold text-secondary-900">{stokBilgi.urunAd}</h3>
                    <p className="text-sm text-secondary-500 font-mono">{stokBilgi.barkod || stokBilgi.seriNo}</p>
                  </div>

                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <span className="text-secondary-500">Kategori:</span>
                      <span className="ml-2 font-medium">{stokBilgi.kategoriAd || '-'}</span>
                    </div>
                    <div>
                      <span className="text-secondary-500">Depo:</span>
                      <span className="ml-2 font-medium">{stokBilgi.mevcutDepoAd || '-'}</span>
                    </div>
                    <div>
                      <span className="text-secondary-500">Üretim Tarihi:</span>
                      <span className="ml-2 font-medium">
                        {stokBilgi.uretimTarihi ? new Date(stokBilgi.uretimTarihi).toLocaleDateString('tr-TR') : '-'}
                      </span>
                    </div>
                    <div>
                      <span className="text-secondary-500">Menşei:</span>
                      <span className="ml-2 font-medium">{stokBilgi.mensei || '-'}</span>
                    </div>
                    <div>
                      <span className="text-secondary-500">Alış Fiyatı:</span>
                      <span className="ml-2 font-medium">
                        {stokBilgi.alisFiyati ? `${stokBilgi.alisFiyati.toFixed(2)} ₺` : '-'}
                      </span>
                    </div>
                    <div>
                      <span className="text-secondary-500">Satış Fiyatı:</span>
                      <span className="ml-2 font-medium text-primary-600 font-bold">
                        {stokBilgi.satisFiyati ? `${stokBilgi.satisFiyati.toFixed(2)} ₺` : '-'}
                      </span>
                    </div>
                  </div>

                  <div className="flex items-center gap-4 pt-2 border-t border-primary-200">
                    <div>
                      <span className="text-secondary-500 text-sm">Stok ID:</span>
                      <span className="ml-2 font-mono text-sm">{stokBilgi.id}</span>
                    </div>
                    <div>
                      {stokBilgi.satildi ? (
                        <Badge variant="danger" className="flex items-center gap-1">
                          <XCircle className="w-3 h-3" />
                          Satıldı
                        </Badge>
                      ) : (
                        <Badge variant="success" className="flex items-center gap-1">
                          <CheckCircle className="w-3 h-3" />
                          Stokta
                        </Badge>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </Card>
    </div>
  );
}
