'use client';

import { useEffect, useState } from 'react';
import { Edit, Trash2, Printer, Search, Package } from 'lucide-react';
import { Card, Modal, Badge, PageLoading } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { stokApi, urunApi, depoApi, bayiApi } from '@/lib/api';
import { SeriNumarasiDetay, Urun, Depo } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import { printBarcodeLabel } from '@/lib/barcode';
import { useAuthStore } from '@/store/authStore';

interface StokKalemForm {
  depoId?: string;
  alisFiyati?: number;
  satisFiyati?: number;
  aciklama?: string;
}

export default function StokKalemleriPage() {
  const { user } = useAuthStore();
  const [loading, setLoading] = useState(true);
  const [kalemler, setKalemler] = useState<SeriNumarasiDetay[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);
  const [selectedKalem, setSelectedKalem] = useState<SeriNumarasiDetay | null>(null);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [saving, setSaving] = useState(false);
  const [urunler, setUrunler] = useState<Urun[]>([]);
  const [depolar, setDepolar] = useState<Depo[]>([]);
  const [filters, setFilters] = useState({
    urunId: '',
    depoId: '',
    satildi: '',
  });

  const { register, handleSubmit, reset, formState: { errors } } = useForm<StokKalemForm>();

  useEffect(() => {
    loadFilters();
  }, []);

  useEffect(() => {
    loadData();
  }, [page, filters]);

  const loadFilters = async () => {
    try {
      const [urunRes, bayiRes] = await Promise.all([
        urunApi.getList(1, 100),
        bayiApi.getAll(),
      ]);
      
      if (urunRes.data?.basarili) {
        setUrunler(urunRes.data.data?.items || []);
      } else if (urunRes.data?.items) {
        setUrunler(urunRes.data.items);
      }

      // Tüm bayilerin depolarını topla
      if (bayiRes.data) {
        const bayiList = Array.isArray(bayiRes.data) ? bayiRes.data : bayiRes.data.data || [];
        const allDepolar: Depo[] = [];
        
        for (const bayi of bayiList) {
          try {
            const depoRes = await depoApi.getByBayiId(bayi.id);
            if (depoRes.data) {
              const depoList = Array.isArray(depoRes.data) ? depoRes.data : depoRes.data.data || [];
              allDepolar.push(...depoList);
            }
          } catch (error) {
            console.error(`Depo yükleme hatası (Bayi: ${bayi.id}):`, error);
          }
        }
        
        setDepolar(allDepolar);
      }
    } catch (error) {
      console.error('Filter data load error:', error);
      toast.error('Filtre verileri yüklenirken hata oluştu');
    }
  };

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await stokApi.getKalemler(
        page,
        pageSize,
        filters.urunId || undefined,
        filters.depoId || undefined,
        filters.satildi === 'true' ? true : filters.satildi === 'false' ? false : undefined
      );
      
      // API response formatını kontrol et
      if (response.data) {
        // ApiResponseDto içinde PagedResultDto formatı: { basarili: true, data: { items: [], totalCount: 0 } }
        if (response.data.basarili && response.data.data) {
          const pagedData = response.data.data;
          if (pagedData.items) {
            setKalemler(pagedData.items);
            setTotalCount(pagedData.totalCount || 0);
          } else if (Array.isArray(pagedData)) {
            setKalemler(pagedData);
            setTotalCount(pagedData.length);
          }
        }
        // Direkt PagedResultDto formatı: { items: [], totalCount: 0 }
        else if (response.data.items) {
          setKalemler(response.data.items);
          setTotalCount(response.data.totalCount || 0);
        } 
        // Direkt array formatı
        else if (Array.isArray(response.data)) {
          setKalemler(response.data);
          setTotalCount(response.data.length);
        }
      }
    } catch (error: any) {
      console.error('Stok kalemleri yükleme hatası:', error);
      toast.error(error.response?.data?.mesaj || 'Stok kalemleri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (kalem: SeriNumarasiDetay) => {
    setSelectedKalem(kalem);
    reset({
      depoId: kalem.mevcutDepoAd ? depolar.find(d => d.ad === kalem.mevcutDepoAd)?.id : '',
      alisFiyati: kalem.alisFiyati,
      satisFiyati: kalem.satisFiyati,
      aciklama: '',
    });
    setShowEditModal(true);
  };

  const handleDelete = (kalem: SeriNumarasiDetay) => {
    setSelectedKalem(kalem);
    setShowDeleteModal(true);
  };

  const handleUpdate = async (formData: StokKalemForm) => {
    if (!selectedKalem) return;
    setSaving(true);
    try {
      const response = await stokApi.updateKalem(selectedKalem.id, formData);
      if (response.data.basarili) {
        toast.success('Stok kalemi güncellendi');
        setShowEditModal(false);
        loadData();
      } else {
        toast.error(response.data.mesaj || 'Güncelleme başarısız');
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Güncelleme başarısız');
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!selectedKalem) return;
    setSaving(true);
    try {
      const response = await stokApi.deleteKalem(selectedKalem.id);
      if (response.data.basarili) {
        toast.success('Stok kalemi silindi');
        setShowDeleteModal(false);
        loadData();
      } else {
        toast.error(response.data.mesaj || 'Silme başarısız');
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme başarısız');
    } finally {
      setSaving(false);
    }
  };

  const handlePrintBarcode = (kalem: SeriNumarasiDetay) => {
    printBarcodeLabel(kalem);
  };

  const columns: Column<SeriNumarasiDetay>[] = [
    {
      key: 'seriNo',
      header: 'Seri No',
      render: (item) => (
        <div className="font-mono text-sm">{item.seriNo}</div>
      ),
    },
    {
      key: 'urun',
      header: 'Ürün',
      render: (item) => (
        <div>
          <p className="font-medium">{item.urunAd}</p>
          <p className="text-xs text-secondary-500">{item.barkod}</p>
        </div>
      ),
    },
    {
      key: 'kategori',
      header: 'Kategori',
      render: (item) => <span className="text-sm">{item.kategoriAd || '-'}</span>,
    },
    {
      key: 'depo',
      header: 'Depo',
      render: (item) => <span className="text-sm">{item.mevcutDepoAd || '-'}</span>,
    },
    {
      key: 'fiyat',
      header: 'Fiyat',
      render: (item) => (
        <div>
          {item.satisFiyati && (
            <p className="text-sm font-medium">{item.satisFiyati.toFixed(2)} ₺</p>
          )}
          {item.alisFiyati && (
            <p className="text-xs text-secondary-500">Alış: {item.alisFiyati.toFixed(2)} ₺</p>
          )}
        </div>
      ),
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) =>
        item.satildi ? (
          <Badge variant="danger">Satıldı</Badge>
        ) : (
          <Badge variant="success">Stokta</Badge>
        ),
    },
    {
      key: 'actions',
      header: '',
      className: 'w-32',
      render: (item) => (
        <div className="flex items-center gap-1">
          <button
            onClick={() => handlePrintBarcode(item)}
            className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
            title="Karekod Yazdır"
          >
            <Printer className="w-4 h-4" />
          </button>
          {!item.satildi && (
            <>
              <button
                onClick={() => handleEdit(item)}
                className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
                title="Düzenle"
              >
                <Edit className="w-4 h-4" />
              </button>
              <button
                onClick={() => handleDelete(item)}
                className="p-1.5 text-secondary-500 hover:text-danger hover:bg-red-50 rounded"
                title="Sil"
              >
                <Trash2 className="w-4 h-4" />
              </button>
            </>
          )}
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Stok Kalemleri</h1>
          <p className="text-secondary-500">Stok kalemlerini yönetin ve karekod yazdırın</p>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <div className="p-4 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="label">Ürün</label>
              <select
                className="select"
                value={filters.urunId}
                onChange={(e) => setFilters({ ...filters, urunId: e.target.value })}
              >
                <option value="">Tümü</option>
                {urunler.map((u) => (
                  <option key={u.id} value={u.id}>{u.ad}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="label">Depo</label>
              <select
                className="select"
                value={filters.depoId}
                onChange={(e) => setFilters({ ...filters, depoId: e.target.value })}
              >
                <option value="">Tümü</option>
                {depolar.map((d) => (
                  <option key={d.id} value={d.id}>{d.ad}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="label">Durum</label>
              <select
                className="select"
                value={filters.satildi}
                onChange={(e) => setFilters({ ...filters, satildi: e.target.value })}
              >
                <option value="">Tümü</option>
                <option value="false">Stokta</option>
                <option value="true">Satıldı</option>
              </select>
            </div>
          </div>
        </div>
      </Card>

      {/* Table */}
      <Card padding={false}>
        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={kalemler}
            keyExtractor={(item) => item.id}
            emptyMessage="Stok kalemi bulunamadı"
            pagination={{
              page,
              pageSize,
              totalCount,
              onPageChange: setPage,
            }}
          />
        )}
      </Card>

      {/* Edit Modal */}
      <Modal
        isOpen={showEditModal}
        onClose={() => setShowEditModal(false)}
        title="Stok Kalemi Düzenle"
        size="md"
        footer={
          <>
            <button onClick={() => setShowEditModal(false)} className="btn-secondary">İptal</button>
            <button onClick={handleSubmit(handleUpdate)} className="btn-primary" disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          <div>
            <label className="label">Depo</label>
            <select {...register('depoId')} className="select">
              <option value="">Depo Seçin</option>
              {depolar.map((d) => (
                <option key={d.id} value={d.id}>{d.ad}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="label">Alış Fiyatı</label>
            <input
              type="number"
              step="0.01"
              {...register('alisFiyati', { valueAsNumber: true })}
              className="input"
            />
          </div>
          <div>
            <label className="label">Satış Fiyatı</label>
            <input
              type="number"
              step="0.01"
              {...register('satisFiyati', { valueAsNumber: true })}
              className="input"
            />
          </div>
          <div>
            <label className="label">Açıklama</label>
            <textarea {...register('aciklama')} className="input" rows={3} />
          </div>
        </form>
      </Modal>

      {/* Delete Modal */}
      <Modal
        isOpen={showDeleteModal}
        onClose={() => setShowDeleteModal(false)}
        title="Stok Kalemi Sil"
        size="sm"
        footer={
          <>
            <button onClick={() => setShowDeleteModal(false)} className="btn-secondary">İptal</button>
            <button onClick={handleDeleteConfirm} className="btn-danger" disabled={saving}>
              {saving ? 'Siliniyor...' : 'Sil'}
            </button>
          </>
        }
      >
        <p className="text-secondary-700">
          <strong>{selectedKalem?.seriNo}</strong> seri numaralı stok kalemini silmek istediğinize emin misiniz?
        </p>
      </Modal>
    </div>
  );
}
