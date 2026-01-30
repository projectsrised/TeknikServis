'use client';

import { useEffect, useState } from 'react';
import { Wallet, ArrowUpCircle, ArrowDownCircle, Plus, TrendingUp, TrendingDown } from 'lucide-react';
import { Card, CardHeader, Modal, Badge, PageLoading, StatCard } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { kasaApi } from '@/lib/api';
import { Kasa, KasaHareket, KasaHareketTip, PagedResult } from '@/types';
import { useForm } from 'react-hook-form';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';
import toast from 'react-hot-toast';
import { useAuthStore } from '@/store/authStore';

interface HareketForm {
  tip: KasaHareketTip;
  tutar: number;
  belgeNo?: string;
  aciklama?: string;
}

export default function KasaPage() {
  const { user } = useAuthStore();
  const [loading, setLoading] = useState(true);
  const [kasa, setKasa] = useState<Kasa | null>(null);
  const [hareketler, setHareketler] = useState<PagedResult<KasaHareket>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
  });
  const [showModal, setShowModal] = useState(false);
  const [hareketTipi, setHareketTipi] = useState<'giris' | 'cikis'>('giris');
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, setValue } = useForm<HareketForm>();

  useEffect(() => {
    if (user?.bayiId) {
      loadData();
    }
  }, [user?.bayiId, hareketler.page]);

  const loadData = async () => {
    if (!user?.bayiId) return;
    
    try {
      setLoading(true);
      const kasaRes = await kasaApi.getByBayiId(user.bayiId);
      if (kasaRes.data.basarili && kasaRes.data.data) {
        setKasa(kasaRes.data.data);
        
        const hareketRes = await kasaApi.getHareketler(kasaRes.data.data.id, hareketler.page, hareketler.pageSize);
        if (hareketRes.data.basarili) {
          setHareketler(hareketRes.data.data);
        }
      }
    } catch (error) {
      toast.error('Kasa verileri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (tip: 'giris' | 'cikis') => {
    setHareketTipi(tip);
    setValue('tip', tip === 'giris' ? KasaHareketTip.Giris : KasaHareketTip.Cikis);
    reset({
      tip: tip === 'giris' ? KasaHareketTip.Giris : KasaHareketTip.Cikis,
      tutar: 0,
      belgeNo: '',
      aciklama: '',
    });
    setShowModal(true);
  };

  const onSubmit = async (formData: HareketForm) => {
    if (!kasa) return;
    
    setSaving(true);
    try {
      const response = await kasaApi.hareketEkle({
        kasaId: kasa.id,
        ...formData,
      });

      if (response.data.basarili) {
        toast.success('Kasa hareketi eklendi');
        setShowModal(false);
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Bir hata oluştu');
    } finally {
      setSaving(false);
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  const getHareketBadge = (tip: KasaHareketTip) => {
    const girisler = [KasaHareketTip.Giris, KasaHareketTip.NakitSatis, KasaHareketTip.KartSatis, KasaHareketTip.TeknikServisOdeme];
    return girisler.includes(tip) ? 'success' : 'danger';
  };

  const columns: Column<KasaHareket>[] = [
    {
      key: 'tarih',
      header: 'Tarih',
      render: (item) => format(new Date(item.islemTarihi), 'dd MMM yyyy HH:mm', { locale: tr }),
    },
    {
      key: 'tip',
      header: 'İşlem Tipi',
      render: (item) => (
        <div className="flex items-center gap-2">
          {[KasaHareketTip.Giris, KasaHareketTip.NakitSatis, KasaHareketTip.KartSatis, KasaHareketTip.TeknikServisOdeme].includes(item.tip) ? (
            <ArrowUpCircle className="w-4 h-4 text-success" />
          ) : (
            <ArrowDownCircle className="w-4 h-4 text-danger" />
          )}
          <Badge variant={getHareketBadge(item.tip)}>{item.tipAd}</Badge>
        </div>
      ),
    },
    {
      key: 'belgeNo',
      header: 'Belge No',
      render: (item) => item.belgeNo || '-',
    },
    {
      key: 'aciklama',
      header: 'Açıklama',
      render: (item) => (
        <span className="max-w-[200px] truncate block text-secondary-600">
          {item.aciklama || '-'}
        </span>
      ),
    },
    {
      key: 'tutar',
      header: 'Tutar',
      render: (item) => (
        <span className={`font-semibold ${getHareketBadge(item.tip) === 'success' ? 'text-success' : 'text-danger'}`}>
          {getHareketBadge(item.tip) === 'success' ? '+' : '-'}{formatCurrency(item.tutar)}
        </span>
      ),
    },
    {
      key: 'bakiye',
      header: 'Bakiye',
      render: (item) => (
        <span className="font-medium">{formatCurrency(item.sonrakiBakiye)}</span>
      ),
    },
  ];

  if (loading) return <PageLoading />;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Kasa</h1>
          <p className="text-secondary-500">Kasa hareketlerini takip edin</p>
        </div>
        <div className="flex gap-2">
          <button onClick={() => openModal('giris')} className="btn-success">
            <ArrowUpCircle className="w-4 h-4 mr-2" />
            Kasa Girişi
          </button>
          <button onClick={() => openModal('cikis')} className="btn-danger">
            <ArrowDownCircle className="w-4 h-4 mr-2" />
            Kasa Çıkışı
          </button>
        </div>
      </div>

      {/* Balance Card */}
      <Card className="bg-gradient-to-r from-primary-600 to-primary-700 text-white">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-primary-100 text-sm">Güncel Bakiye</p>
            <p className="text-4xl font-bold mt-1">{formatCurrency(kasa?.bakiye || 0)}</p>
            <p className="text-primary-200 text-sm mt-2">{kasa?.ad}</p>
          </div>
          <div className="w-20 h-20 bg-white/10 rounded-full flex items-center justify-center">
            <Wallet className="w-10 h-10 text-white" />
          </div>
        </div>
      </Card>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Card>
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-green-600" />
            </div>
            <div>
              <p className="text-2xl font-bold text-success">
                {formatCurrency(
                  hareketler.items
                    .filter(h => [KasaHareketTip.Giris, KasaHareketTip.NakitSatis, KasaHareketTip.KartSatis, KasaHareketTip.TeknikServisOdeme].includes(h.tip))
                    .reduce((acc, h) => acc + h.tutar, 0)
                )}
              </p>
              <p className="text-sm text-secondary-500">Toplam Giriş</p>
            </div>
          </div>
        </Card>
        <Card>
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
              <TrendingDown className="w-6 h-6 text-red-600" />
            </div>
            <div>
              <p className="text-2xl font-bold text-danger">
                {formatCurrency(
                  hareketler.items
                    .filter(h => ![KasaHareketTip.Giris, KasaHareketTip.NakitSatis, KasaHareketTip.KartSatis, KasaHareketTip.TeknikServisOdeme].includes(h.tip))
                    .reduce((acc, h) => acc + h.tutar, 0)
                )}
              </p>
              <p className="text-sm text-secondary-500">Toplam Çıkış</p>
            </div>
          </div>
        </Card>
      </div>

      {/* Transactions Table */}
      <Card padding={false}>
        <div className="p-4 border-b border-secondary-200">
          <h3 className="font-semibold text-secondary-900">Son Hareketler</h3>
        </div>
        <DataTable
          columns={columns}
          data={hareketler.items}
          keyExtractor={(item) => item.id}
          emptyMessage="Kasa hareketi bulunamadı"
          pagination={{
            page: hareketler.page,
            pageSize: hareketler.pageSize,
            totalCount: hareketler.totalCount,
            totalPages: hareketler.totalPages,
            onPageChange: (page) => setHareketler({ ...hareketler, page }),
          }}
        />
      </Card>

      {/* Hareket Ekle Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title={hareketTipi === 'giris' ? 'Kasa Girişi' : 'Kasa Çıkışı'}
        size="md"
        footer={
          <>
            <button onClick={() => setShowModal(false)} className="btn-secondary">İptal</button>
            <button 
              onClick={handleSubmit(onSubmit)} 
              className={hareketTipi === 'giris' ? 'btn-success' : 'btn-danger'} 
              disabled={saving}
            >
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          <div>
            <label className="label">Tutar *</label>
            <input
              type="number"
              step="0.01"
              {...register('tutar', { required: true, valueAsNumber: true, min: 0.01 })}
              className="input text-xl font-bold"
              placeholder="0.00"
            />
          </div>

          <div>
            <label className="label">Belge No</label>
            <input {...register('belgeNo')} className="input" placeholder="Fiş, makbuz no vs." />
          </div>

          <div>
            <label className="label">Açıklama</label>
            <textarea {...register('aciklama')} className="input" rows={3} placeholder="İşlem açıklaması..." />
          </div>
        </form>
      </Modal>
    </div>
  );
}
