'use client';

import { useEffect, useState } from 'react';
import { Plus, Edit, Trash2, Tags, ChevronRight, FolderOpen } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, ConfirmDialog } from '@/components/ui';
import { kategoriApi } from '@/lib/api';
import { Kategori, KategoriCreate, KategoriTip } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

const kategoriTipler = [
  { value: KategoriTip.Telefon, label: 'Telefon' },
  { value: KategoriTip.Tablet, label: 'Tablet' },
  { value: KategoriTip.Aksesuar, label: 'Aksesuar' },
  { value: KategoriTip.YedekParca, label: 'Yedek Parça' },
  { value: KategoriTip.Diger, label: 'Diğer' },
];

export default function KategorilerPage() {
  const [loading, setLoading] = useState(true);
  const [kategoriler, setKategoriler] = useState<Kategori[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [editingKategori, setEditingKategori] = useState<Kategori | null>(null);
  const [deleteKategori, setDeleteKategori] = useState<Kategori | null>(null);
  const [saving, setSaving] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<KategoriCreate>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await kategoriApi.getAll();
      if (response.data.basarili) {
        setKategoriler(response.data.data);
      }
    } catch (error) {
      toast.error('Kategoriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const openModal = (kategori?: Kategori) => {
    if (kategori) {
      setEditingKategori(kategori);
      reset({
        ad: kategori.ad,
        kod: kategori.kod || '',
        aciklama: kategori.aciklama || '',
        tip: kategori.tip,
        ustKategoriId: kategori.ustKategoriId || undefined,
        sira: kategori.sira,
      });
    } else {
      setEditingKategori(null);
      reset({
        ad: '',
        kod: '',
        aciklama: '',
        tip: KategoriTip.Telefon,
        sira: kategoriler.length + 1,
      });
    }
    setShowModal(true);
  };

  const onSubmit = async (formData: KategoriCreate) => {
    setSaving(true);
    try {
      if (editingKategori) {
        const response = await kategoriApi.update(editingKategori.id, formData);
        if (response.data.basarili) {
          toast.success('Kategori güncellendi');
          setShowModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await kategoriApi.create(formData);
        if (response.data.basarili) {
          toast.success('Kategori oluşturuldu');
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
    if (!deleteKategori) return;
    try {
      const response = await kategoriApi.delete(deleteKategori.id);
      if (response.data.basarili) {
        toast.success('Kategori silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteKategori(null);
    }
  };

  const getTipColor = (tip: KategoriTip) => {
    const colors: Record<number, string> = {
      1: 'bg-blue-100 text-blue-700',
      2: 'bg-purple-100 text-purple-700',
      3: 'bg-green-100 text-green-700',
      4: 'bg-orange-100 text-orange-700',
      5: 'bg-gray-100 text-gray-700',
    };
    return colors[tip] || colors[5];
  };

  if (loading) return <PageLoading />;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Kategoriler</h1>
          <p className="text-secondary-500">Ürün kategorilerini yönetin</p>
        </div>
        <button onClick={() => openModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Kategori
        </button>
      </div>

      {/* Categories Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {kategoriler.map((kategori) => (
          <Card key={kategori.id} hover className="group">
            <div className="flex items-start justify-between">
              <div className="flex items-center gap-3">
                <div className={`w-12 h-12 rounded-lg flex items-center justify-center ${getTipColor(kategori.tip)}`}>
                  <Tags className="w-6 h-6" />
                </div>
                <div>
                  <h3 className="font-semibold text-secondary-900">{kategori.ad}</h3>
                  <p className="text-sm text-secondary-500">{kategori.kod}</p>
                </div>
              </div>
              <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                <button
                  onClick={() => openModal(kategori)}
                  className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
                >
                  <Edit className="w-4 h-4" />
                </button>
                <button
                  onClick={() => setDeleteKategori(kategori)}
                  className="p-1.5 text-secondary-500 hover:text-danger hover:bg-red-50 rounded"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t border-secondary-100">
              <div className="flex items-center justify-between text-sm">
                <Badge variant="secondary">{kategori.tipAd}</Badge>
                <Badge variant={kategori.aktif ? 'success' : 'secondary'}>
                  {kategori.aktif ? 'Aktif' : 'Pasif'}
                </Badge>
              </div>
              {kategori.aciklama && (
                <p className="mt-2 text-sm text-secondary-500 line-clamp-2">{kategori.aciklama}</p>
              )}
            </div>
          </Card>
        ))}

        {kategoriler.length === 0 && (
          <div className="col-span-full text-center py-12">
            <FolderOpen className="w-16 h-16 mx-auto text-secondary-300 mb-4" />
            <h3 className="text-lg font-medium text-secondary-900">Kategori Bulunamadı</h3>
            <p className="text-secondary-500 mt-1">Yeni kategori ekleyerek başlayın</p>
          </div>
        )}
      </div>

      {/* Create/Edit Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title={editingKategori ? 'Kategori Düzenle' : 'Yeni Kategori'}
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
            <label className="label">Kategori Adı *</label>
            <input
              {...register('ad', { required: 'Kategori adı gerekli' })}
              className={errors.ad ? 'input-error' : 'input'}
            />
            {errors.ad && <p className="text-sm text-danger mt-1">{errors.ad.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="label">Kategori Kodu</label>
              <input {...register('kod')} className="input" placeholder="TEL, AKS..." />
            </div>
            <div>
              <label className="label">Sıra</label>
              <input type="number" {...register('sira', { valueAsNumber: true })} className="input" />
            </div>
          </div>

          <div>
            <label className="label">Kategori Tipi *</label>
            <select {...register('tip', { valueAsNumber: true })} className="select">
              {kategoriTipler.map((t) => (
                <option key={t.value} value={t.value}>{t.label}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="label">Üst Kategori</label>
            <select {...register('ustKategoriId')} className="select">
              <option value="">Ana Kategori</option>
              {kategoriler
                .filter((k) => k.id !== editingKategori?.id)
                .map((k) => (
                  <option key={k.id} value={k.id}>{k.ad}</option>
                ))}
            </select>
          </div>

          <div>
            <label className="label">Açıklama</label>
            <textarea {...register('aciklama')} className="input" rows={2} />
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!deleteKategori}
        onClose={() => setDeleteKategori(null)}
        onConfirm={handleDelete}
        title="Kategoriyi Sil"
        message={`"${deleteKategori?.ad}" kategorisini silmek istediğinize emin misiniz? Bu kategorideki ürünler de etkilenebilir.`}
        confirmText="Sil"
      />
    </div>
  );
}
