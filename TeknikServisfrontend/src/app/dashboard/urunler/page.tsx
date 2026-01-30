'use client';

import { useEffect, useState } from 'react';
import { Plus, Search, Edit, Trash2, Package, AlertTriangle, Filter } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, ConfirmDialog } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { urunApi, kategoriApi } from '@/lib/api';
import { Urun, UrunCreate, Kategori, PagedResult } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

export default function UrunlerPage() {
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<PagedResult<Urun>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
  });
  const [kategoriler, setKategoriler] = useState<Kategori[]>([]);
  const [search, setSearch] = useState('');
  const [selectedKategori, setSelectedKategori] = useState<string>('');
  const [showModal, setShowModal] = useState(false);
  const [editingUrun, setEditingUrun] = useState<Urun | null>(null);
  const [deleteUrun, setDeleteUrun] = useState<Urun | null>(null);
  const [saving, setSaving] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<UrunCreate>();

  useEffect(() => {
    loadKategoriler();
  }, []);

  useEffect(() => {
    loadData();
  }, [data.page, search, selectedKategori]);

  const loadKategoriler = async () => {
    try {
      const response = await kategoriApi.getAll();
      if (response.data.basarili) {
        setKategoriler(response.data.data);
      }
    } catch (error) {
      console.error('Kategoriler yüklenirken hata:', error);
    }
  };

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await urunApi.getList(
        data.page,
        data.pageSize,
        selectedKategori || undefined,
        search || undefined
      );
      if (response.data.basarili) {
        setData(response.data.data);
      }
    } catch (error) {
      toast.error('Ürünler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (urun?: Urun) => {
    if (urun) {
      setEditingUrun(urun);
      reset({
        kategoriId: urun.kategoriId,
        ad: urun.ad,
        kritikStok: urun.kritikStok,
        seriTakipli: urun.seriTakipli,
      });
    } else {
      setEditingUrun(null);
      reset({
        kategoriId: '',
        ad: '',
        kritikStok: 5,
        seriTakipli: true,
      });
    }
    setShowModal(true);
  };

  const onSubmit = async (formData: UrunCreate) => {
    setSaving(true);
    try {
      if (editingUrun) {
        const response = await urunApi.update(editingUrun.id, formData);
        if (response.data.basarili) {
          toast.success('Ürün güncellendi');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await urunApi.create(formData);
        if (response.data.basarili) {
          toast.success('Ürün oluşturuldu');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Bir hata oluştu');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteUrun) return;
    try {
      const response = await urunApi.delete(deleteUrun.id);
      if (response.data.basarili) {
        toast.success('Ürün silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteUrun(null);
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  const columns: Column<Urun>[] = [
    {
      key: 'urun',
      header: 'Ürün',
      render: (item) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-secondary-100 rounded-lg flex items-center justify-center">
            <Package className="w-5 h-5 text-secondary-500" />
          </div>
          <div>
            <p className="font-medium text-secondary-900">{item.ad}</p>
            <p className="text-xs text-secondary-500">
              {item.marka} {item.model}
            </p>
          </div>
        </div>
      ),
    },
    {
      key: 'kategori',
      header: 'Kategori',
      render: (item) => <Badge variant="secondary">{item.kategoriAd}</Badge>,
    },
    {
      key: 'barkod',
      header: 'Barkod',
      render: (item) => (
        <span className="font-mono text-sm text-secondary-600">{item.barkod || '-'}</span>
      ),
    },
    {
      key: 'fiyat',
      header: 'Fiyat',
      render: (item) => (
        <div>
          <p className="font-medium text-secondary-900">{formatCurrency(item.satisFiyat)}</p>
          <p className="text-xs text-secondary-500">Alış: {formatCurrency(item.alisFiyat)}</p>
        </div>
      ),
    },
    {
      key: 'stok',
      header: 'Stok',
      render: (item) => (
        <div className="flex items-center gap-2">
          <span
            className={
              item.mevcutStok <= item.kritikStok ? 'text-danger font-medium' : 'text-secondary-900'
            }
          >
            {item.mevcutStok}
          </span>
          {item.mevcutStok <= item.kritikStok && (
            <AlertTriangle className="w-4 h-4 text-danger" />
          )}
        </div>
      ),
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) => (
        <Badge variant={item.aktif ? 'success' : 'secondary'}>
          {item.aktif ? 'Aktif' : 'Pasif'}
        </Badge>
      ),
    },
    {
      key: 'actions',
      header: '',
      className: 'w-24',
      render: (item) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => {
              e.stopPropagation();
              openModal(item);
            }}
            className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
          >
            <Edit className="w-4 h-4" />
          </button>
          <button
            onClick={(e) => {
              e.stopPropagation();
              setDeleteUrun(item);
            }}
            className="p-1.5 text-secondary-500 hover:text-danger hover:bg-red-50 rounded"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Ürünler</h1>
          <p className="text-secondary-500">Ürün kataloğunuzu yönetin</p>
        </div>
        <button onClick={() => openModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Ürün
        </button>
      </div>

      {/* Filters */}
      <Card padding={false}>
        <div className="p-4 border-b border-secondary-200 flex flex-wrap gap-4">
          <div className="relative flex-1 min-w-[200px] max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-secondary-400" />
            <input
              type="text"
              placeholder="Ürün ara... (Ad, Barkod, Kod)"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="input pl-10"
            />
          </div>
          <div className="flex items-center gap-2">
            <Filter className="w-5 h-5 text-secondary-400" />
            <select
              value={selectedKategori}
              onChange={(e) => setSelectedKategori(e.target.value)}
              className="select"
            >
              <option value="">Tüm Kategoriler</option>
              {kategoriler.map((k) => (
                <option key={k.id} value={k.id}>
                  {k.ad}
                </option>
              ))}
            </select>
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
            emptyMessage="Ürün bulunamadı"
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

      {/* Create/Edit Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title={editingUrun ? 'Ürün Düzenle' : 'Yeni Ürün'}
        size="md"
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
          <div>
            <label className="label">Kategori *</label>
            <select
              {...register('kategoriId', { required: 'Kategori seçin' })}
              className={errors.kategoriId ? 'select input-error' : 'select'}
            >
              <option value="">Kategori Seçin</option>
              {kategoriler.map((k) => (
                <option key={k.id} value={k.id}>
                  {k.ad}
                </option>
              ))}
            </select>
            {errors.kategoriId && (
              <p className="text-sm text-danger mt-1">{errors.kategoriId.message}</p>
            )}
          </div>

          <div>
            <label className="label">Ürün Adı *</label>
            <input
              {...register('ad', { required: 'Ürün adı gerekli' })}
              className={errors.ad ? 'input-error' : 'input'}
              placeholder="Ürün adını girin"
            />
            {errors.ad && <p className="text-sm text-danger mt-1">{errors.ad.message}</p>}
          </div>

          {editingUrun && (
            <div>
              <label className="label">Ürün Kodu</label>
              <input
                value={editingUrun.kod || ''}
                className="input bg-secondary-50"
                disabled
              />
              <p className="text-xs text-secondary-500 mt-1">Kod otomatik oluşturulur</p>
            </div>
          )}

          <div>
            <label className="label">Kritik Stok Seviyesi</label>
            <input
              type="number"
              {...register('kritikStok', { valueAsNumber: true })}
              className="input"
              placeholder="5"
            />
            <p className="text-xs text-secondary-500 mt-1">Bu seviyenin altına düşünce uyarı verilir</p>
          </div>

          <div className="flex items-center">
            <label className="flex items-center gap-3 cursor-pointer p-3 bg-secondary-50 rounded-lg w-full">
              <input type="checkbox" {...register('seriTakipli')} className="w-5 h-5 rounded" />
              <div>
                <span className="text-sm font-medium text-secondary-900">Seri Numarası Takibi</span>
                <p className="text-xs text-secondary-500">Her ürün için ayrı seri numarası takip edilir</p>
              </div>
            </label>
          </div>
        </form>
      </Modal>

      {/* Delete Confirm */}
      <ConfirmDialog
        isOpen={!!deleteUrun}
        onClose={() => setDeleteUrun(null)}
        onConfirm={handleDelete}
        title="Ürünü Sil"
        message={`"${deleteUrun?.ad}" ürününü silmek istediğinize emin misiniz?`}
        confirmText="Sil"
      />
    </div>
  );
}
