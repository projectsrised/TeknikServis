'use client';

import { useEffect, useState } from 'react';
import { Plus, Search, ShoppingCart, Trash2, CreditCard, Banknote, QrCode, X } from 'lucide-react';
import { Card, CardHeader, Modal, Badge, PageLoading } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { satisApi, stokApi } from '@/lib/api';
import { Satis, SatisCreate, SeriNumarasi, OdemeTipi, PagedResult } from '@/types';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';
import toast from 'react-hot-toast';

interface SepetItem {
  seriNo: string;
  urunAd: string;
  fiyat: number;
  kdvOran: number;
}

export default function SatislarPage() {
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<PagedResult<Satis>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
  });
  const [showPOS, setShowPOS] = useState(false);
  const [sepet, setSepet] = useState<SepetItem[]>([]);
  const [barkodInput, setBarkodInput] = useState('');
  const [odemeTipi, setOdemeTipi] = useState<OdemeTipi>(OdemeTipi.Nakit);
  const [musteriAd, setMusteriAd] = useState('');
  const [musteriTelefon, setMusteriTelefon] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    loadData();
  }, [data.page]);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await satisApi.getList(data.page, data.pageSize);
      if (response.data.basarili) {
        setData(response.data.data);
      }
    } catch (error) {
      toast.error('Satışlar yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleBarkodTara = async () => {
    if (!barkodInput.trim()) return;

    try {
      const response = await satisApi.validateSeri(barkodInput.trim());
      if (response.data.basarili && response.data.data.gecerli) {
        const { seriNumarasi, satisFiyat, kdvOran } = response.data.data;
        
        // Zaten sepette mi kontrol et
        if (sepet.find(s => s.seriNo === barkodInput.trim())) {
          toast.error('Bu ürün zaten sepette');
          return;
        }

        setSepet([...sepet, {
          seriNo: barkodInput.trim(),
          urunAd: seriNumarasi?.urunAd || 'Ürün',
          fiyat: satisFiyat,
          kdvOran: kdvOran,
        }]);
        setBarkodInput('');
        toast.success('Ürün eklendi');
      } else {
        toast.error(response.data.data?.mesaj || 'Geçersiz seri numarası');
      }
    } catch (error: any) {
      toast.error('Ürün bulunamadı');
    }
  };

  const handleSepettenCikar = (seriNo: string) => {
    setSepet(sepet.filter(s => s.seriNo !== seriNo));
  };

  const toplamTutar = sepet.reduce((acc, item) => acc + item.fiyat, 0);
  const toplamKdv = sepet.reduce((acc, item) => acc + (item.fiyat * item.kdvOran / (100 + item.kdvOran)), 0);

  const handleSatisYap = async () => {
    if (sepet.length === 0) {
      toast.error('Sepet boş');
      return;
    }

    setSaving(true);
    try {
      const satisData: SatisCreate = {
        musteriAd: musteriAd || undefined,
        musteriTelefon: musteriTelefon || undefined,
        odemeTipi,
        nakitTutar: odemeTipi === OdemeTipi.Nakit ? toplamTutar : 0,
        kartTutar: odemeTipi === OdemeTipi.KrediKarti ? toplamTutar : 0,
        indirimTutar: 0,
        kalemler: sepet.map(s => ({ seriNo: s.seriNo })),
      };

      const response = await satisApi.create(satisData);
      if (response.data.basarili) {
        toast.success(`Satış tamamlandı! Satış No: ${response.data.data.satisNo}`);
        setShowPOS(false);
        setSepet([]);
        setMusteriAd('');
        setMusteriTelefon('');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Satış yapılamadı');
    } finally {
      setSaving(false);
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  const columns: Column<Satis>[] = [
    {
      key: 'satisNo',
      header: 'Satış No',
      render: (item) => (
        <span className="font-medium text-primary-600">{item.satisNo}</span>
      ),
    },
    {
      key: 'tarih',
      header: 'Tarih',
      render: (item) => format(new Date(item.satisTarihi), 'dd MMM yyyy HH:mm', { locale: tr }),
    },
    {
      key: 'musteri',
      header: 'Müşteri',
      render: (item) => item.musteriAd || 'Perakende',
    },
    {
      key: 'urunSayisi',
      header: 'Ürün',
      render: (item) => `${item.kalemler?.length || 0} adet`,
    },
    {
      key: 'odeme',
      header: 'Ödeme',
      render: (item) => <Badge variant="secondary">{item.odemeTipiAd}</Badge>,
    },
    {
      key: 'tutar',
      header: 'Tutar',
      render: (item) => (
        <span className="font-semibold">{formatCurrency(item.genelToplam)}</span>
      ),
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) =>
        item.iptalEdildi ? (
          <Badge variant="danger">İptal</Badge>
        ) : (
          <Badge variant="success">Tamamlandı</Badge>
        ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Satışlar</h1>
          <p className="text-secondary-500">Satış işlemlerini yönetin</p>
        </div>
        <button onClick={() => setShowPOS(true)} className="btn-primary">
          <ShoppingCart className="w-4 h-4 mr-2" />
          Yeni Satış
        </button>
      </div>

      {/* Table */}
      <Card padding={false}>
        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={data.items}
            keyExtractor={(item) => item.id}
            emptyMessage="Satış bulunamadı"
            pagination={{
              page: data.page,
              pageSize: data.pageSize,
              totalCount: data.totalCount,
              totalPages: data.totalPages,
              onPageChange: (page) => setData({ ...data, page }),
            }}
          />
        )}
      </Card>

      {/* POS Modal */}
      <Modal
        isOpen={showPOS}
        onClose={() => setShowPOS(false)}
        title="Yeni Satış"
        size="xl"
      >
        <div className="grid grid-cols-3 gap-6">
          {/* Sol - Barkod ve Sepet */}
          <div className="col-span-2 space-y-4">
            {/* Barkod Input */}
            <div className="flex gap-2">
              <div className="relative flex-1">
                <QrCode className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-secondary-400" />
                <input
                  type="text"
                  value={barkodInput}
                  onChange={(e) => setBarkodInput(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && handleBarkodTara()}
                  placeholder="Seri numarası okutun veya yazın..."
                  className="input pl-10"
                  autoFocus
                />
              </div>
              <button onClick={handleBarkodTara} className="btn-primary">
                Ekle
              </button>
            </div>

            {/* Sepet */}
            <div className="border border-secondary-200 rounded-lg overflow-hidden">
              <div className="bg-secondary-50 px-4 py-2 font-medium text-secondary-700">
                Sepet ({sepet.length} ürün)
              </div>
              <div className="max-h-[300px] overflow-auto">
                {sepet.length === 0 ? (
                  <div className="p-8 text-center text-secondary-500">
                    <ShoppingCart className="w-12 h-12 mx-auto mb-2 opacity-50" />
                    <p>Sepet boş</p>
                    <p className="text-sm">Ürün eklemek için seri numarası okutun</p>
                  </div>
                ) : (
                  <table className="w-full">
                    <tbody>
                      {sepet.map((item, idx) => (
                        <tr key={item.seriNo} className="border-b border-secondary-100 last:border-0">
                          <td className="px-4 py-3">
                            <span className="text-secondary-500 text-sm">{idx + 1}.</span>
                          </td>
                          <td className="px-4 py-3">
                            <p className="font-medium text-secondary-900">{item.urunAd}</p>
                            <p className="text-xs text-secondary-500 font-mono">{item.seriNo}</p>
                          </td>
                          <td className="px-4 py-3 text-right font-medium">
                            {formatCurrency(item.fiyat)}
                          </td>
                          <td className="px-4 py-3 w-10">
                            <button
                              onClick={() => handleSepettenCikar(item.seriNo)}
                              className="p-1 text-secondary-400 hover:text-danger"
                            >
                              <X className="w-4 h-4" />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            </div>
          </div>

          {/* Sağ - Özet ve Ödeme */}
          <div className="space-y-4">
            {/* Müşteri */}
            <div className="space-y-3">
              <h4 className="font-medium text-secondary-900">Müşteri (Opsiyonel)</h4>
              <input
                type="text"
                value={musteriAd}
                onChange={(e) => setMusteriAd(e.target.value)}
                placeholder="Müşteri Adı"
                className="input"
              />
              <input
                type="text"
                value={musteriTelefon}
                onChange={(e) => setMusteriTelefon(e.target.value)}
                placeholder="Telefon"
                className="input"
              />
            </div>

            {/* Ödeme Tipi */}
            <div className="space-y-3">
              <h4 className="font-medium text-secondary-900">Ödeme Tipi</h4>
              <div className="grid grid-cols-2 gap-2">
                <button
                  onClick={() => setOdemeTipi(OdemeTipi.Nakit)}
                  className={`p-3 rounded-lg border-2 transition-all ${
                    odemeTipi === OdemeTipi.Nakit
                      ? 'border-primary-500 bg-primary-50'
                      : 'border-secondary-200 hover:border-secondary-300'
                  }`}
                >
                  <Banknote className={`w-6 h-6 mx-auto mb-1 ${odemeTipi === OdemeTipi.Nakit ? 'text-primary-600' : 'text-secondary-400'}`} />
                  <span className={`text-sm font-medium ${odemeTipi === OdemeTipi.Nakit ? 'text-primary-600' : 'text-secondary-600'}`}>
                    Nakit
                  </span>
                </button>
                <button
                  onClick={() => setOdemeTipi(OdemeTipi.KrediKarti)}
                  className={`p-3 rounded-lg border-2 transition-all ${
                    odemeTipi === OdemeTipi.KrediKarti
                      ? 'border-primary-500 bg-primary-50'
                      : 'border-secondary-200 hover:border-secondary-300'
                  }`}
                >
                  <CreditCard className={`w-6 h-6 mx-auto mb-1 ${odemeTipi === OdemeTipi.KrediKarti ? 'text-primary-600' : 'text-secondary-400'}`} />
                  <span className={`text-sm font-medium ${odemeTipi === OdemeTipi.KrediKarti ? 'text-primary-600' : 'text-secondary-600'}`}>
                    Kart
                  </span>
                </button>
              </div>
            </div>

            {/* Toplam */}
            <div className="p-4 bg-secondary-900 rounded-lg text-white">
              <div className="flex justify-between text-secondary-400 text-sm mb-1">
                <span>Ara Toplam</span>
                <span>{formatCurrency(toplamTutar - toplamKdv)}</span>
              </div>
              <div className="flex justify-between text-secondary-400 text-sm mb-3">
                <span>KDV</span>
                <span>{formatCurrency(toplamKdv)}</span>
              </div>
              <div className="flex justify-between text-2xl font-bold">
                <span>TOPLAM</span>
                <span>{formatCurrency(toplamTutar)}</span>
              </div>
            </div>

            {/* Satış Butonu */}
            <button
              onClick={handleSatisYap}
              disabled={sepet.length === 0 || saving}
              className="btn-success w-full py-4 text-lg"
            >
              {saving ? 'İşleniyor...' : 'Satışı Tamamla'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
