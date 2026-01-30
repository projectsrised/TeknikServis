'use client';

import { useEffect, useState } from 'react';
import { Plus, Search, UserCog, Mail, Phone, Edit, Trash2, Shield } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, ConfirmDialog } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { personelApi, bayiApi } from '@/lib/api';
import { Personel, UserRole, Bayi } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

interface PersonelForm {
  bayiId: string;
  ad: string;
  soyad: string;
  email: string;
  sifre: string;
  telefon?: string;
  rol: UserRole;
  maas?: number;
}

const roller = [
  { value: UserRole.TenantAdmin, label: 'Yönetici' },
  { value: UserRole.BayiAdmin, label: 'Bayi Yöneticisi' },
  { value: UserRole.SatisSorumlusu, label: 'Satış Sorumlusu' },
  { value: UserRole.TeknikServisPersoneli, label: 'Teknik Servis Personeli' },
];

export default function PersonelPage() {
  const [loading, setLoading] = useState(true);
  const [personeller, setPersoneller] = useState<Personel[]>([]);
  const [bayiler, setBayiler] = useState<Bayi[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [editingPersonel, setEditingPersonel] = useState<Personel | null>(null);
  const [deletePersonel, setDeletePersonel] = useState<Personel | null>(null);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<PersonelForm>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [personelRes, bayiRes] = await Promise.all([
        personelApi.getList(),
        bayiApi.getAll(),
      ]);

      // Personel: PagedResult veya direkt array olabilir
      if (personelRes.data) {
        if (personelRes.data.basarili && personelRes.data.data) {
          // ApiResponse<PagedResult> formatı
          const data = personelRes.data.data;
          setPersoneller(data.items || data || []);
        } else if (Array.isArray(personelRes.data)) {
          setPersoneller(personelRes.data);
        }
      }

      // Bayi: Array veya ApiResponse olabilir
      if (bayiRes.data) {
        if (Array.isArray(bayiRes.data)) {
          setBayiler(bayiRes.data);
        } else if (bayiRes.data.basarili && bayiRes.data.data) {
          setBayiler(Array.isArray(bayiRes.data.data) ? bayiRes.data.data : []);
        } else if (bayiRes.data.data) {
          setBayiler(Array.isArray(bayiRes.data.data) ? bayiRes.data.data : []);
        }
      }
    } catch (error) {
      console.error('Personel yükleme hatası:', error);
      toast.error('Personel verileri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (personel?: Personel) => {
    if (personel) {
      setEditingPersonel(personel);
      reset({
        bayiId: personel.bayiId || '',
        ad: personel.ad,
        soyad: personel.soyad,
        email: personel.email,
        sifre: '',
        telefon: personel.telefon || '',
        rol: personel.rol,
        maas: personel.maas || undefined,
      });
    } else {
      setEditingPersonel(null);
      reset({
        bayiId: '',
        ad: '',
        soyad: '',
        email: '',
        sifre: '',
        telefon: '',
        rol: UserRole.SatisSorumlusu,
        maas: undefined,
      });
    }
    setShowModal(true);
  };

  const onSubmit = async (formData: PersonelForm) => {
    setSaving(true);
    try {
      if (editingPersonel) {
        const response = await personelApi.update(editingPersonel.id, formData);
        if (response.data.basarili) {
          toast.success('Personel güncellendi');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await personelApi.create(formData);
        if (response.data.basarili) {
          toast.success('Personel oluşturuldu');
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
    if (!deletePersonel) return;
    try {
      const response = await personelApi.delete(deletePersonel.id);
      if (response.data.basarili) {
        toast.success('Personel silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeletePersonel(null);
    }
  };

  const getRolBadge = (rol: UserRole) => {
    const variants: Record<number, 'primary' | 'success' | 'warning' | 'secondary'> = {
      1: 'primary',
      2: 'primary',
      3: 'success',
      4: 'warning',
      5: 'secondary',
    };
    return variants[rol] || 'secondary';
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  const columns: Column<Personel>[] = [
    {
      key: 'personel',
      header: 'Personel',
      render: (item) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
            <span className="text-primary-600 font-semibold text-sm">
              {item.ad.charAt(0)}{item.soyad.charAt(0)}
            </span>
          </div>
          <div>
            <p className="font-medium text-secondary-900">{item.adSoyad}</p>
            <p className="text-xs text-secondary-500">{item.bayiAd}</p>
          </div>
        </div>
      ),
    },
    {
      key: 'iletisim',
      header: 'İletişim',
      render: (item) => (
        <div className="space-y-1">
          <div className="flex items-center gap-2 text-sm text-secondary-600">
            <Mail className="w-3 h-3" />
            {item.email}
          </div>
          {item.telefon && (
            <div className="flex items-center gap-2 text-sm text-secondary-600">
              <Phone className="w-3 h-3" />
              {item.telefon}
            </div>
          )}
        </div>
      ),
    },
    {
      key: 'rol',
      header: 'Rol',
      render: (item) => (
        <div className="flex items-center gap-2">
          <Shield className="w-4 h-4 text-secondary-400" />
          <Badge variant={getRolBadge(item.rol)}>{item.rolAd}</Badge>
        </div>
      ),
    },
    {
      key: 'maas',
      header: 'Maaş',
      render: (item) => (
        <span className="font-medium">
          {item.maas ? formatCurrency(item.maas) : '-'}
        </span>
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
            onClick={() => openModal(item)}
            className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
          >
            <Edit className="w-4 h-4" />
          </button>
          <button
            onClick={() => setDeletePersonel(item)}
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
          <h1 className="text-2xl font-bold text-secondary-900">Personel</h1>
          <p className="text-secondary-500">Personel kayıtlarını yönetin</p>
        </div>
        <button onClick={() => openModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Personel
        </button>
      </div>

      <Card padding={false}>
        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={personeller}
            keyExtractor={(item) => item.id}
            emptyMessage="Personel bulunamadı"
          />
        )}
      </Card>

      {/* Create/Edit Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title={editingPersonel ? 'Personel Düzenle' : 'Yeni Personel'}
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
              <label className="label">Ad *</label>
              <input {...register('ad', { required: 'Ad gerekli' })} className={errors.ad ? 'input-error' : 'input'} />
            </div>
            <div>
              <label className="label">Soyad *</label>
              <input {...register('soyad', { required: 'Soyad gerekli' })} className={errors.soyad ? 'input-error' : 'input'} />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">E-posta *</label>
              <input type="email" {...register('email', { required: 'E-posta gerekli' })} className={errors.email ? 'input-error' : 'input'} />
            </div>
            <div>
              <label className="label">Telefon</label>
              <input {...register('telefon')} className="input" />
            </div>
          </div>

          {!editingPersonel && (
            <div>
              <label className="label">Şifre *</label>
              <input type="password" {...register('sifre', { required: !editingPersonel })} className="input" />
            </div>
          )}

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Rol *</label>
              <select {...register('rol', { valueAsNumber: true })} className="select">
                {roller.map((r) => (
                  <option key={r.value} value={r.value}>{r.label}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="label">Maaş</label>
              <input type="number" step="0.01" {...register('maas', { valueAsNumber: true })} className="input" />
            </div>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!deletePersonel}
        onClose={() => setDeletePersonel(null)}
        onConfirm={handleDelete}
        title="Personeli Sil"
        message={`"${deletePersonel?.adSoyad}" personelini silmek istediğinize emin misiniz?`}
        confirmText="Sil"
      />
    </div>
  );
}
