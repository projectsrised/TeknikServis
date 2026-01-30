'use client';

import { useEffect, useState } from 'react';
import { Plus, Search, Phone, Mail, Edit, Trash2, Eye } from 'lucide-react';
import { Card, CardHeader, Modal, Badge, PageLoading, ConfirmDialog, EmptyState } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { musteriApi } from '@/lib/api';
import { Musteri, MusteriCreate, PagedResult } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

export default function MusterilerPage() {
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<PagedResult<Musteri>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
  });
  const [search, setSearch] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingMusteri, setEditingMusteri] = useState<Musteri | null>(null);
  const [deleteMusteri, setDeleteMusteri] = useState<Musteri | null>(null);
  const [saving, setSaving] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<MusteriCreate>();

  useEffect(() => {
    loadData();
  }, [data.page, search]);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await musteriApi.getList(data.page, data.pageSize, search || undefined);
      if (response.data.basarili) {
        setData(response.data.data);
      }
    } catch (error) {
      toast.error('Müşteriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (musteri?: Musteri) => {
    if (musteri) {
      setEditingMusteri(musteri);
      reset({
        ad: musteri.ad,
        soyad: musteri.soyad,
        telefon: musteri.telefon,
        email: musteri.email || '',
        adres: musteri.adres || '',
        tcNo: musteri.tcNo || '',
        vergiNo: musteri.vergiNo || '',
        kurumsal: musteri.kurumsal,
      });
    } else {
      setEditingMusteri(null);
      reset({
        ad: '',
        soyad: '',
        telefon: '',
        email: '',
        adres: '',
        tcNo: '',
        vergiNo: '',
        kurumsal: false,
      });
    }
    setShowModal(true);
  };

  const onSubmit = async (formData: MusteriCreate) => {
    setSaving(true);
    try {
      if (editingMusteri) {
        const response = await musteriApi.update(editingMusteri.id, formData);
        if (response.data.basarili) {
          toast.success('Müşteri güncellendi');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await musteriApi.create(formData);
        if (response.data.basarili) {
          toast.success('Müşteri oluşturuldu');
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
    if (!deleteMusteri) return;
    try {
      const response = await musteriApi.delete(deleteMusteri.id);
      if (response.data.basarili) {
        toast.success('Müşteri silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteMusteri(null);
    }
  };

  const columns: Column<Musteri>[] = [
    {
      key: 'adSoyad',
      header: 'Müşteri',
      render: (item) => (
        <div>
          <p className="font-medium text-secondary-900">{item.adSoyad}</p>
          {item.kurumsal && <Badge variant="primary">Kurumsal</Badge>}
        </div>
      ),
    },
    {
      key: 'telefon',
      header: 'Telefon',
      render: (item) => (
        <div className="flex items-center gap-2 text-secondary-600">
          <Phone className="w-4 h-4" />
          {item.telefon}
        </div>
      ),
    },
    {
      key: 'email',
      header: 'E-posta',
      render: (item) =>
        item.email ? (
          <div className="flex items-center gap-2 text-secondary-600">
            <Mail className="w-4 h-4" />
            {item.email}
          </div>
        ) : (
          <span className="text-secondary-400">-</span>
        ),
    },
    {
      key: 'adres',
      header: 'Adres',
      render: (item) => (
        <span className="text-secondary-600 max-w-[200px] truncate block">
          {item.adres || '-'}
        </span>
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
              setDeleteMusteri(item);
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
          <h1 className="text-2xl font-bold text-secondary-900">Müşteriler</h1>
          <p className="text-secondary-500">Müşteri kayıtlarını yönetin</p>
        </div>
        <button onClick={() => openModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Müşteri
        </button>
      </div>

      {/* Search & Filters */}
      <Card padding={false}>
        <div className="p-4 border-b border-secondary-200">
          <div className="relative max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-secondary-400" />
            <input
              type="text"
              placeholder="Müşteri ara... (Ad, Telefon)"
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
            emptyMessage="Müşteri bulunamadı"
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
        title={editingMusteri ? 'Müşteri Düzenle' : 'Yeni Müşteri'}
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
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Ad *</label>
              <input
                {...register('ad', { required: 'Ad gerekli' })}
                className={errors.ad ? 'input-error' : 'input'}
              />
              {errors.ad && <p className="text-sm text-danger mt-1">{errors.ad.message}</p>}
            </div>
            <div>
              <label className="label">Soyad *</label>
              <input
                {...register('soyad', { required: 'Soyad gerekli' })}
                className={errors.soyad ? 'input-error' : 'input'}
              />
              {errors.soyad && <p className="text-sm text-danger mt-1">{errors.soyad.message}</p>}
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Telefon *</label>
              <input
                {...register('telefon', { required: 'Telefon gerekli' })}
                className={errors.telefon ? 'input-error' : 'input'}
                placeholder="05XX XXX XX XX"
              />
              {errors.telefon && <p className="text-sm text-danger mt-1">{errors.telefon.message}</p>}
            </div>
            <div>
              <label className="label">E-posta</label>
              <input {...register('email')} type="email" className="input" />
            </div>
          </div>

          <div>
            <label className="label">Adres</label>
            <textarea {...register('adres')} className="input" rows={2} />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">TC Kimlik No</label>
              <input {...register('tcNo')} className="input" maxLength={11} />
            </div>
            <div>
              <label className="label">Vergi No</label>
              <input {...register('vergiNo')} className="input" />
            </div>
          </div>

          <div>
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="checkbox" {...register('kurumsal')} className="w-4 h-4 rounded" />
              <span className="text-sm text-secondary-700">Kurumsal Müşteri</span>
            </label>
          </div>
        </form>
      </Modal>

      {/* Delete Confirm */}
      <ConfirmDialog
        isOpen={!!deleteMusteri}
        onClose={() => setDeleteMusteri(null)}
        onConfirm={handleDelete}
        title="Müşteriyi Sil"
        message={`"${deleteMusteri?.adSoyad}" müşterisini silmek istediğinize emin misiniz?`}
        confirmText="Sil"
      />
    </div>
  );
}
