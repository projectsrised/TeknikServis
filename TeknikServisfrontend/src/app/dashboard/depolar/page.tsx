'use client';

import { useEffect, useState } from 'react';
import { Plus, Warehouse, MapPin, Edit, Trash2, Package } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, ConfirmDialog } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { depoApi, bayiApi } from '@/lib/api';
import { Depo, Bayi } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

interface DepoForm {
  bayiId: string;
  ad: string;
  kod?: string;
  adres?: string;
  merkezDepoMu: boolean;
}

export default function DepolarPage() {
  const [loading, setLoading] = useState(true);
  const [depolar, setDepolar] = useState<Depo[]>([]);
  const [bayiler, setBayiler] = useState<Bayi[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [editingDepo, setEditingDepo] = useState<Depo | null>(null);
  const [deleteDepo, setDeleteDepo] = useState<Depo | null>(null);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<DepoForm>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const bayiRes = await bayiApi.getAll();
      if (bayiRes.data.basarili) {
        setBayiler(bayiRes.data.data);
        
        // Tüm bayilerin depolarını yükle
        const allDepolar: Depo[] = [];
        for (const bayi of bayiRes.data.data) {
          const depoRes = await depoApi.getByBayiId(bayi.id);
          if (depoRes.data.basarili) {
            allDepolar.push(...depoRes.data.data);
          }
        }
        setDepolar(allDepolar);
      }
    } catch (error) {
      toast.error('Depolar yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (depo?: Depo) => {
    if (depo) {
      setEditingDepo(depo);
      reset({
        bayiId: depo.bayiId,
        ad: depo.ad,
        kod: depo.kod || '',
        adres: depo.adres || '',
        merkezDepoMu: depo.merkezDepoMu,
      });
    } else {
      setEditingDepo(null);
      reset({
        bayiId: '',
        ad: '',
        kod: '',
        adres: '',
        merkezDepoMu: false,
      });
    }
    setShowModal(true);
  };

  const onSubmit = async (formData: DepoForm) => {
    setSaving(true);
    try {
      if (editingDepo) {
        const response = await depoApi.update(editingDepo.id, formData);
        if (response.data.basarili) {
          toast.success('Depo güncellendi');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await depoApi.create(formData);
        if (response.data.basarili) {
          toast.success('Depo oluşturuldu');
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
    if (!deleteDepo) return;
    try {
      const response = await depoApi.delete(deleteDepo.id);
      if (response.data.basarili) {
        toast.success('Depo silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteDepo(null);
    }
  };

  const columns: Column<Depo>[] = [
    {
      key: 'depo',
      header: 'Depo',
      render: (item) => (
        <div className="flex items-center gap-3">
          <div className={`w-10 h-10 rounded-lg flex items-center justify-center ${item.merkezDepoMu ? 'bg-primary-100' : 'bg-secondary-100'}`}>
            <Warehouse className={`w-5 h-5 ${item.merkezDepoMu ? 'text-primary-600' : 'text-secondary-500'}`} />
          </div>
          <div>
            <p className="font-medium text-secondary-900">{item.ad}</p>
            <p className="text-xs text-secondary-500">{item.kod}</p>
          </div>
        </div>
      ),
    },
    {
      key: 'bayi',
      header: 'Bayi',
      render: (item) => item.bayiAd || '-',
    },
    {
      key: 'adres',
      header: 'Adres',
      render: (item) => (
        item.adres ? (
          <div className="flex items-center gap-2 text-secondary-600">
            <MapPin className="w-4 h-4" />
            <span className="max-w-[200px] truncate">{item.adres}</span>
          </div>
        ) : (
          <span className="text-secondary-400">-</span>
        )
      ),
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) => (
        <div className="flex gap-2">
          {item.merkezDepoMu && <Badge variant="primary">Merkez</Badge>}
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
            onClick={() => setDeleteDepo(item)}
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
          <h1 className="text-2xl font-bold text-secondary-900">Depolar</h1>
          <p className="text-secondary-500">Depo ve stok lokasyonlarını yönetin</p>
        </div>
        <button onClick={() => openModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Depo
        </button>
      </div>

      <Card padding={false}>
        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={depolar}
            keyExtractor={(item) => item.id}
            emptyMessage="Depo bulunamadı"
          />
        )}
      </Card>

      {/* Create/Edit Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title={editingDepo ? 'Depo Düzenle' : 'Yeni Depo'}
        size="md"
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
          <div>
            <label className="label">Bayi *</label>
            <select {...register('bayiId', { required: 'Bayi seçin' })} className="select">
              <option value="">Bayi Seçin</option>
              {bayiler.map((b) => (
                <option key={b.id} value={b.id}>{b.ad}</option>
              ))}
            </select>
            {errors.bayiId && <p className="text-sm text-danger mt-1">{errors.bayiId.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Depo Adı *</label>
              <input {...register('ad', { required: 'Depo adı gerekli' })} className={errors.ad ? 'input-error' : 'input'} />
            </div>
            <div>
              <label className="label">Depo Kodu</label>
              <input {...register('kod')} className="input" placeholder="DEP001" />
            </div>
          </div>

          <div>
            <label className="label">Adres</label>
            <textarea {...register('adres')} className="input" rows={2} />
          </div>

          <div>
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="checkbox" {...register('merkezDepoMu')} className="w-4 h-4 rounded" />
              <span className="text-sm text-secondary-700">Merkez Depo</span>
            </label>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!deleteDepo}
        onClose={() => setDeleteDepo(null)}
        onConfirm={handleDelete}
        title="Depoyu Sil"
        message={`"${deleteDepo?.ad}" deposunu silmek istediğinize emin misiniz? Depodaki stoklar da etkilenebilir.`}
        confirmText="Sil"
      />
    </div>
  );
}
