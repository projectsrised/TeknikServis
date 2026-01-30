'use client';

import { useEffect, useState } from 'react';
import { Plus, Search, Store, MapPin, Phone, Mail, Edit, Trash2, Building2 } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, ConfirmDialog } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { bayiApi } from '@/lib/api';
import { Bayi, BayiCreate } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

export default function BayilerPage() {
  const [loading, setLoading] = useState(true);
  const [bayiler, setBayiler] = useState<Bayi[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [editingBayi, setEditingBayi] = useState<Bayi | null>(null);
  const [deleteBayi, setDeleteBayi] = useState<Bayi | null>(null);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<BayiCreate>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await bayiApi.getAll();
      if (response.data.basarili) {
        setBayiler(response.data.data);
      }
    } catch (error) {
      toast.error('Bayiler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (bayi?: Bayi) => {
    if (bayi) {
      setEditingBayi(bayi);
      reset({
        ad: bayi.ad,
        kod: bayi.kod || '',
        adres: bayi.adres || '',
        il: bayi.il || '',
        ilce: bayi.ilce || '',
        telefon: bayi.telefon || '',
        email: bayi.email || '',
        yetkiliAd: bayi.yetkiliAd || '',
        merkezMi: bayi.merkezMi,
      });
    } else {
      setEditingBayi(null);
      reset({
        ad: '',
        kod: '',
        adres: '',
        il: '',
        ilce: '',
        telefon: '',
        email: '',
        yetkiliAd: '',
        merkezMi: false,
      });
    }
    setShowModal(true);
  };

  const onSubmit = async (formData: BayiCreate) => {
    setSaving(true);
    try {
      if (editingBayi) {
        const response = await bayiApi.update(editingBayi.id, formData);
        if (response.data.basarili) {
          toast.success('Bayi güncellendi');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await bayiApi.create(formData);
        if (response.data.basarili) {
          toast.success('Bayi oluşturuldu');
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
    if (!deleteBayi) return;
    try {
      const response = await bayiApi.delete(deleteBayi.id);
      if (response.data.basarili) {
        toast.success('Bayi silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteBayi(null);
    }
  };

  const columns: Column<Bayi>[] = [
    {
      key: 'bayi',
      header: 'Bayi',
      render: (item) => (
        <div className="flex items-center gap-3">
          <div className={`w-10 h-10 rounded-lg flex items-center justify-center ${item.merkezMi ? 'bg-primary-100' : 'bg-secondary-100'}`}>
            {item.merkezMi ? (
              <Building2 className="w-5 h-5 text-primary-600" />
            ) : (
              <Store className="w-5 h-5 text-secondary-500" />
            )}
          </div>
          <div>
            <p className="font-medium text-secondary-900">{item.ad}</p>
            <p className="text-xs text-secondary-500">{item.kod}</p>
          </div>
        </div>
      ),
    },
    {
      key: 'konum',
      header: 'Konum',
      render: (item) => (
        <div className="flex items-center gap-2 text-secondary-600">
          <MapPin className="w-4 h-4" />
          {item.il ? `${item.ilce}, ${item.il}` : '-'}
        </div>
      ),
    },
    {
      key: 'iletisim',
      header: 'İletişim',
      render: (item) => (
        <div className="space-y-1">
          {item.telefon && (
            <div className="flex items-center gap-2 text-sm text-secondary-600">
              <Phone className="w-3 h-3" />
              {item.telefon}
            </div>
          )}
          {item.email && (
            <div className="flex items-center gap-2 text-sm text-secondary-600">
              <Mail className="w-3 h-3" />
              {item.email}
            </div>
          )}
        </div>
      ),
    },
    {
      key: 'yetkili',
      header: 'Yetkili',
      render: (item) => item.yetkiliAd || '-',
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) => (
        <div className="flex gap-2">
          {item.merkezMi && <Badge variant="primary">Merkez</Badge>}
          <Badge variant={item.aktif ? 'success' : 'secondary'}>
            {item.aktif ? 'Aktif' : 'Pasif'}
          </Badge>
        </div>
      ),
    },
    {
      key: 'actions',
      header: '',
      className: 'w-24',
      render: (item) => (
        <div className="flex items-center gap-1">
          <button
            onClick={() => openModal(item)}
            className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
          >
            <Edit className="w-4 h-4" />
          </button>
          <button
            onClick={() => setDeleteBayi(item)}
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
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Bayiler</h1>
          <p className="text-secondary-500">Bayi ve şube yönetimi</p>
        </div>
        <button onClick={() => openModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Bayi
        </button>
      </div>

      <Card padding={false}>
        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={bayiler}
            keyExtractor={(item) => item.id}
            emptyMessage="Bayi bulunamadı"
          />
        )}
      </Card>

      {/* Create/Edit Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title={editingBayi ? 'Bayi Düzenle' : 'Yeni Bayi'}
        size="lg"
        footer={
          <>
            <button onClick={() => setShowModal(false)} className="btn-secondary">İptal</button>
            <button onClick={handleSubmit(onSubmit)} className="btn-primary" disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Bayi Adı *</label>
              <input {...register('ad', { required: 'Bayi adı gerekli' })} className={errors.ad ? 'input-error' : 'input'} />
              {errors.ad && <p className="text-sm text-danger mt-1">{errors.ad.message}</p>}
            </div>
            <div>
              <label className="label">Bayi Kodu</label>
              <input {...register('kod')} className="input" placeholder="MRK001" />
            </div>
          </div>

          <div>
            <label className="label">Adres</label>
            <textarea {...register('adres')} className="input" rows={2} />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">İl</label>
              <input {...register('il')} className="input" />
            </div>
            <div>
              <label className="label">İlçe</label>
              <input {...register('ilce')} className="input" />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Telefon</label>
              <input {...register('telefon')} className="input" />
            </div>
            <div>
              <label className="label">E-posta</label>
              <input {...register('email')} type="email" className="input" />
            </div>
          </div>

          <div>
            <label className="label">Yetkili Adı</label>
            <input {...register('yetkiliAd')} className="input" />
          </div>

          <div>
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="checkbox" {...register('merkezMi')} className="w-4 h-4 rounded" />
              <span className="text-sm text-secondary-700">Merkez Şube</span>
            </label>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!deleteBayi}
        onClose={() => setDeleteBayi(null)}
        onConfirm={handleDelete}
        title="Bayiyi Sil"
        message={`"${deleteBayi?.ad}" bayisini silmek istediğinize emin misiniz?`}
        confirmText="Sil"
      />
    </div>
  );
}
