'use client';

import { useEffect, useState } from 'react';
import { Plus, Search, Filter, Eye, Edit, Wrench, Clock, CheckCircle, XCircle } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, Tabs } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { teknikServisApi } from '@/lib/api';
import { TeknikServis, TeknikServisCreate, TeknikServisDurum, PagedResult } from '@/types';
import { useForm } from 'react-hook-form';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';
import toast from 'react-hot-toast';

const durumTabs = [
  { id: 'all', label: 'Tümü' },
  { id: '1', label: 'Beklemede' },
  { id: '2', label: 'İnceleniyor' },
  { id: '3', label: 'Parça Bekleniyor' },
  { id: '4', label: 'Onarılıyor' },
  { id: '5', label: 'Tamamlandı' },
  { id: '6', label: 'Teslim Edildi' },
];

const getDurumBadge = (durum: TeknikServisDurum) => {
  const variants: Record<number, 'warning' | 'primary' | 'secondary' | 'success' | 'danger'> = {
    1: 'warning',
    2: 'primary',
    3: 'secondary',
    4: 'primary',
    5: 'success',
    6: 'success',
    7: 'danger',
  };
  return variants[durum] || 'secondary';
};

export default function TeknikServisPage() {
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<PagedResult<TeknikServis>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
  });
  const [search, setSearch] = useState('');
  const [activeTab, setActiveTab] = useState('all');
  const [showModal, setShowModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [selectedServis, setSelectedServis] = useState<TeknikServis | null>(null);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<TeknikServisCreate>();

  useEffect(() => {
    loadData();
  }, [data.page, activeTab]);

  const loadData = async () => {
    try {
      setLoading(true);
      const durum = activeTab === 'all' ? undefined : parseInt(activeTab);
      const response = await teknikServisApi.getList(data.page, data.pageSize, durum);
      if (response.data.basarili) {
        setData(response.data.data);
      }
    } catch (error) {
      toast.error('Servisler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = () => {
    reset({
      cihazTip: '',
      cihazMarka: '',
      cihazModel: '',
      seriNo: '',
      imeiNo: '',
      cihazSifre: '',
      ariza: '',
      arizaDetay: '',
      iscilikUcreti: 0,
      garantiKapsami: false,
    });
    setShowModal(true);
  };

  const onSubmit = async (formData: TeknikServisCreate) => {
    setSaving(true);
    try {
      const response = await teknikServisApi.create(formData);
      if (response.data.basarili) {
        toast.success('Servis kaydı oluşturuldu');
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

  const handleDurumGuncelle = async (id: string, durum: TeknikServisDurum) => {
    try {
      const response = await teknikServisApi.updateDurum(id, { durum });
      if (response.data.basarili) {
        toast.success('Durum güncellendi');
        loadData();
        setShowDetailModal(false);
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Bir hata oluştu');
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  const columns: Column<TeknikServis>[] = [
    {
      key: 'servisNo',
      header: 'Servis No',
      render: (item) => (
        <span className="font-medium text-primary-600">{item.servisNo}</span>
      ),
    },
    {
      key: 'cihaz',
      header: 'Cihaz',
      render: (item) => (
        <div>
          <p className="font-medium text-secondary-900">
            {item.cihazMarka} {item.cihazModel}
          </p>
          <p className="text-xs text-secondary-500">{item.cihazTip}</p>
        </div>
      ),
    },
    {
      key: 'musteri',
      header: 'Müşteri',
      render: (item) => (
        <div>
          <p className="text-secondary-900">{item.musteriAd || 'Belirtilmedi'}</p>
          <p className="text-xs text-secondary-500">{item.musteriTelefon}</p>
        </div>
      ),
    },
    {
      key: 'ariza',
      header: 'Arıza',
      render: (item) => (
        <span className="max-w-[200px] truncate block text-secondary-600">{item.ariza}</span>
      ),
    },
    {
      key: 'tarih',
      header: 'Giriş Tarihi',
      render: (item) => (
        <div className="flex items-center gap-2 text-secondary-600">
          <Clock className="w-4 h-4" />
          {format(new Date(item.girisTarihi), 'dd MMM yyyy', { locale: tr })}
        </div>
      ),
    },
    {
      key: 'tutar',
      header: 'Tutar',
      render: (item) => (
        <span className="font-medium">{formatCurrency(item.toplamTutar)}</span>
      ),
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) => <Badge variant={getDurumBadge(item.durum)}>{item.durumAd}</Badge>,
    },
    {
      key: 'actions',
      header: '',
      className: 'w-20',
      render: (item) => (
        <button
          onClick={() => {
            setSelectedServis(item);
            setShowDetailModal(true);
          }}
          className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
        >
          <Eye className="w-4 h-4" />
        </button>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Teknik Servis</h1>
          <p className="text-secondary-500">Servis kayıtlarını yönetin</p>
        </div>
        <button onClick={openModal} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Servis Kaydı
        </button>
      </div>

      {/* Tabs & Table */}
      <Card padding={false}>
        <div className="p-4 border-b border-secondary-200">
          <Tabs
            tabs={durumTabs}
            activeTab={activeTab}
            onChange={setActiveTab}
          />
        </div>

        {/* Search */}
        <div className="p-4 border-b border-secondary-200">
          <div className="relative max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-secondary-400" />
            <input
              type="text"
              placeholder="Servis No, Müşteri veya Telefon ara..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="input pl-10"
            />
          </div>
        </div>

        {/* Table */}
        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={data.items}
            keyExtractor={(item) => item.id}
            emptyMessage="Servis kaydı bulunamadı"
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

      {/* Create Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title="Yeni Servis Kaydı"
        size="lg"
        footer={
          <>
            <button onClick={() => setShowModal(false)} className="btn-secondary">
              İptal
            </button>
            <button onClick={handleSubmit(onSubmit)} className="btn-primary" disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          {/* Müşteri Bilgileri */}
          <div className="border-b border-secondary-200 pb-4 mb-4">
            <h3 className="font-medium text-secondary-900 mb-3">Müşteri Bilgileri</h3>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="label">Müşteri Adı</label>
                <input {...register('musteriAd')} className="input" />
              </div>
              <div>
                <label className="label">Telefon</label>
                <input {...register('musteriTelefon')} className="input" placeholder="05XX XXX XX XX" />
              </div>
            </div>
          </div>

          {/* Cihaz Bilgileri */}
          <div className="border-b border-secondary-200 pb-4 mb-4">
            <h3 className="font-medium text-secondary-900 mb-3">Cihaz Bilgileri</h3>
            <div className="grid grid-cols-3 gap-4">
              <div>
                <label className="label">Cihaz Tipi *</label>
                <select
                  {...register('cihazTip', { required: true })}
                  className="select"
                >
                  <option value="">Seçin</option>
                  <option value="Telefon">Telefon</option>
                  <option value="Tablet">Tablet</option>
                  <option value="Laptop">Laptop</option>
                  <option value="Diğer">Diğer</option>
                </select>
              </div>
              <div>
                <label className="label">Marka</label>
                <input {...register('cihazMarka')} className="input" placeholder="Apple, Samsung..." />
              </div>
              <div>
                <label className="label">Model</label>
                <input {...register('cihazModel')} className="input" placeholder="iPhone 15, S24..." />
              </div>
            </div>
            <div className="grid grid-cols-3 gap-4 mt-4">
              <div>
                <label className="label">Seri No</label>
                <input {...register('seriNo')} className="input" />
              </div>
              <div>
                <label className="label">IMEI No</label>
                <input {...register('imeiNo')} className="input" />
              </div>
              <div>
                <label className="label">Cihaz Şifresi</label>
                <input {...register('cihazSifre')} className="input" placeholder="Kilit açma şifresi" />
              </div>
            </div>
          </div>

          {/* Arıza Bilgileri */}
          <div>
            <h3 className="font-medium text-secondary-900 mb-3">Arıza Bilgileri</h3>
            <div>
              <label className="label">Arıza *</label>
              <input
                {...register('ariza', { required: 'Arıza bilgisi gerekli' })}
                className={errors.ariza ? 'input-error' : 'input'}
                placeholder="Ekran kırık, şarj olmuyor..."
              />
              {errors.ariza && <p className="text-sm text-danger mt-1">{errors.ariza.message}</p>}
            </div>
            <div className="mt-4">
              <label className="label">Arıza Detayı</label>
              <textarea
                {...register('arizaDetay')}
                className="input"
                rows={3}
                placeholder="Detaylı açıklama..."
              />
            </div>
            <div className="grid grid-cols-2 gap-4 mt-4">
              <div>
                <label className="label">İşçilik Ücreti</label>
                <input
                  type="number"
                  step="0.01"
                  {...register('iscilikUcreti', { valueAsNumber: true })}
                  className="input"
                />
              </div>
              <div className="flex items-center pt-7">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input type="checkbox" {...register('garantiKapsami')} className="w-4 h-4 rounded" />
                  <span className="text-sm text-secondary-700">Garanti Kapsamında</span>
                </label>
              </div>
            </div>
          </div>
        </form>
      </Modal>

      {/* Detail Modal */}
      <Modal
        isOpen={showDetailModal}
        onClose={() => setShowDetailModal(false)}
        title={`Servis Detayı - ${selectedServis?.servisNo}`}
        size="lg"
      >
        {selectedServis && (
          <div className="space-y-6">
            {/* Durum */}
            <div className="flex items-center justify-between p-4 bg-secondary-50 rounded-lg">
              <div>
                <p className="text-sm text-secondary-500">Mevcut Durum</p>
                <Badge variant={getDurumBadge(selectedServis.durum)}>{selectedServis.durumAd}</Badge>
              </div>
              <div className="flex gap-2">
                {selectedServis.durum < 5 && (
                  <button
                    onClick={() => handleDurumGuncelle(selectedServis.id, selectedServis.durum + 1)}
                    className="btn-primary btn-sm"
                  >
                    Sonraki Aşama
                  </button>
                )}
                {selectedServis.durum === 5 && (
                  <button
                    onClick={() => handleDurumGuncelle(selectedServis.id, 6)}
                    className="btn-success btn-sm"
                  >
                    <CheckCircle className="w-4 h-4 mr-1" />
                    Teslim Et
                  </button>
                )}
              </div>
            </div>

            {/* Müşteri */}
            <div>
              <h4 className="font-medium text-secondary-900 mb-2">Müşteri</h4>
              <p className="text-secondary-600">{selectedServis.musteriAd || 'Belirtilmedi'}</p>
              <p className="text-secondary-500 text-sm">{selectedServis.musteriTelefon}</p>
            </div>

            {/* Cihaz */}
            <div>
              <h4 className="font-medium text-secondary-900 mb-2">Cihaz Bilgileri</h4>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-secondary-500">Tip:</span>{' '}
                  <span className="text-secondary-900">{selectedServis.cihazTip}</span>
                </div>
                <div>
                  <span className="text-secondary-500">Marka/Model:</span>{' '}
                  <span className="text-secondary-900">
                    {selectedServis.cihazMarka} {selectedServis.cihazModel}
                  </span>
                </div>
                {selectedServis.seriNo && (
                  <div>
                    <span className="text-secondary-500">Seri No:</span>{' '}
                    <span className="text-secondary-900">{selectedServis.seriNo}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Arıza */}
            <div>
              <h4 className="font-medium text-secondary-900 mb-2">Arıza</h4>
              <p className="text-secondary-600">{selectedServis.ariza}</p>
              {selectedServis.arizaDetay && (
                <p className="text-secondary-500 text-sm mt-1">{selectedServis.arizaDetay}</p>
              )}
            </div>

            {/* Ücret */}
            <div className="p-4 bg-secondary-50 rounded-lg">
              <div className="flex justify-between mb-2">
                <span className="text-secondary-600">İşçilik</span>
                <span>{formatCurrency(selectedServis.iscilikUcreti)}</span>
              </div>
              <div className="flex justify-between mb-2">
                <span className="text-secondary-600">Parça Toplam</span>
                <span>{formatCurrency(selectedServis.parcaToplam)}</span>
              </div>
              <div className="flex justify-between font-semibold border-t border-secondary-200 pt-2 mt-2">
                <span>Toplam</span>
                <span className="text-primary-600">{formatCurrency(selectedServis.toplamTutar)}</span>
              </div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
