'use client';

import { useEffect, useState } from 'react';
import { Plus, Tag, Edit, Trash2, ChevronDown, ChevronRight, Package } from 'lucide-react';
import { Card, Modal, Badge, PageLoading, ConfirmDialog } from '@/components/ui';
import { markaApi, modelApi } from '@/lib/api';
import { Marka, Model, MarkaCreate, ModelCreate } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

export default function MarkalarPage() {
  const [loading, setLoading] = useState(true);
  const [markalar, setMarkalar] = useState<Marka[]>([]);
  const [modeller, setModeller] = useState<Record<string, Model[]>>({});
  const [expandedMarkalar, setExpandedMarkalar] = useState<Set<string>>(new Set());

  // Marka modal state
  const [showMarkaModal, setShowMarkaModal] = useState(false);
  const [editingMarka, setEditingMarka] = useState<Marka | null>(null);
  const [deleteMarka, setDeleteMarka] = useState<Marka | null>(null);

  // Model modal state
  const [showModelModal, setShowModelModal] = useState(false);
  const [editingModel, setEditingModel] = useState<Model | null>(null);
  const [deleteModel, setDeleteModel] = useState<Model | null>(null);
  const [selectedMarkaId, setSelectedMarkaId] = useState<string>('');

  const [saving, setSaving] = useState(false);

  const markaForm = useForm<MarkaCreate>();
  const modelForm = useForm<ModelCreate>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const response = await markaApi.getAll();
      if (response.data.basarili) {
        setMarkalar(response.data.data);
      }
    } catch (error) {
      toast.error('Markalar yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const loadModeller = async (markaId: string) => {
    try {
      const response = await modelApi.getByMarkaId(markaId);
      if (response.data.basarili) {
        setModeller(prev => ({ ...prev, [markaId]: response.data.data }));
      }
    } catch (error) {
      toast.error('Modeller yüklenirken hata oluştu');
    }
  };

  const toggleMarka = async (markaId: string) => {
    const newExpanded = new Set(expandedMarkalar);
    if (newExpanded.has(markaId)) {
      newExpanded.delete(markaId);
    } else {
      newExpanded.add(markaId);
      if (!modeller[markaId]) {
        await loadModeller(markaId);
      }
    }
    setExpandedMarkalar(newExpanded);
  };

  // Marka CRUD
  const openMarkaModal = (marka?: Marka) => {
    if (marka) {
      setEditingMarka(marka);
      markaForm.reset({ ad: marka.ad, logo: marka.logo || '', sira: marka.sira });
    } else {
      setEditingMarka(null);
      markaForm.reset({ ad: '', logo: '', sira: markalar.length + 1 });
    }
    setShowMarkaModal(true);
  };

  const onMarkaSubmit = async (data: MarkaCreate) => {
    setSaving(true);
    try {
      if (editingMarka) {
        const response = await markaApi.update(editingMarka.id, data);
        if (response.data.basarili) {
          toast.success('Marka güncellendi');
          setShowMarkaModal(false);
          loadData();
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await markaApi.create(data);
        if (response.data.basarili) {
          toast.success('Marka oluşturuldu');
          setShowMarkaModal(false);
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

  const handleDeleteMarka = async () => {
    if (!deleteMarka) return;
    try {
      const response = await markaApi.delete(deleteMarka.id);
      if (response.data.basarili) {
        toast.success('Marka silindi');
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteMarka(null);
    }
  };

  // Model CRUD
  const openModelModal = (markaId: string, model?: Model) => {
    setSelectedMarkaId(markaId);
    if (model) {
      setEditingModel(model);
      modelForm.reset({ markaId: model.markaId, ad: model.ad, kod: model.kod || '', sira: model.sira });
    } else {
      setEditingModel(null);
      const markaModeller = modeller[markaId] || [];
      modelForm.reset({ markaId, ad: '', kod: '', sira: markaModeller.length + 1 });
    }
    setShowModelModal(true);
  };

  const onModelSubmit = async (data: ModelCreate) => {
    setSaving(true);
    try {
      if (editingModel) {
        const response = await modelApi.update(editingModel.id, data);
        if (response.data.basarili) {
          toast.success('Model güncellendi');
          setShowModelModal(false);
          loadModeller(selectedMarkaId);
        } else {
          toast.error(response.data.mesaj);
        }
      } else {
        const response = await modelApi.create(data);
        if (response.data.basarili) {
          toast.success('Model oluşturuldu');
          setShowModelModal(false);
          loadModeller(selectedMarkaId);
          loadData(); // Model sayısını güncellemek için
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

  const handleDeleteModel = async () => {
    if (!deleteModel) return;
    try {
      const response = await modelApi.delete(deleteModel.id);
      if (response.data.basarili) {
        toast.success('Model silindi');
        loadModeller(deleteModel.markaId);
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Silme işlemi başarısız');
    } finally {
      setDeleteModel(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Markalar ve Modeller</h1>
          <p className="text-secondary-500">Telefon marka ve modellerini yönetin</p>
        </div>
        <button onClick={() => openMarkaModal()} className="btn-primary">
          <Plus className="w-4 h-4 mr-2" />
          Yeni Marka
        </button>
      </div>

      <Card padding={false}>
        {loading ? (
          <PageLoading />
        ) : markalar.length === 0 ? (
          <div className="p-8 text-center text-secondary-500">
            Henüz marka eklenmemiş
          </div>
        ) : (
          <div className="divide-y divide-secondary-200">
            {markalar.map((marka) => (
              <div key={marka.id}>
                <div className="flex items-center justify-between p-4 hover:bg-secondary-50 cursor-pointer"
                     onClick={() => toggleMarka(marka.id)}>
                  <div className="flex items-center gap-3">
                    {expandedMarkalar.has(marka.id) ? (
                      <ChevronDown className="w-5 h-5 text-secondary-400" />
                    ) : (
                      <ChevronRight className="w-5 h-5 text-secondary-400" />
                    )}
                    <div className="w-10 h-10 rounded-lg bg-primary-100 flex items-center justify-center">
                      <Tag className="w-5 h-5 text-primary-600" />
                    </div>
                    <div>
                      <p className="font-medium text-secondary-900">{marka.ad}</p>
                      <p className="text-xs text-secondary-500">{marka.modelSayisi} model</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <Badge variant={marka.aktif ? 'success' : 'secondary'}>
                      {marka.aktif ? 'Aktif' : 'Pasif'}
                    </Badge>
                    <button
                      onClick={(e) => { e.stopPropagation(); openMarkaModal(marka); }}
                      className="p-1.5 text-secondary-500 hover:text-primary-600 hover:bg-primary-50 rounded"
                    >
                      <Edit className="w-4 h-4" />
                    </button>
                    <button
                      onClick={(e) => { e.stopPropagation(); setDeleteMarka(marka); }}
                      className="p-1.5 text-secondary-500 hover:text-danger hover:bg-red-50 rounded"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>

                {/* Modeller */}
                {expandedMarkalar.has(marka.id) && (
                  <div className="bg-secondary-50 border-t border-secondary-200">
                    <div className="p-3 flex justify-end">
                      <button
                        onClick={() => openModelModal(marka.id)}
                        className="text-sm text-primary-600 hover:text-primary-700 flex items-center gap-1"
                      >
                        <Plus className="w-4 h-4" />
                        Model Ekle
                      </button>
                    </div>
                    {modeller[marka.id]?.length > 0 ? (
                      <div className="px-4 pb-4 space-y-2">
                        {modeller[marka.id].map((model) => (
                          <div key={model.id} className="flex items-center justify-between p-3 bg-white rounded-lg border border-secondary-200">
                            <div className="flex items-center gap-3">
                              <Package className="w-4 h-4 text-secondary-400" />
                              <div>
                                <p className="text-sm font-medium text-secondary-900">{model.ad}</p>
                                {model.kod && <p className="text-xs text-secondary-500">{model.kod}</p>}
                              </div>
                            </div>
                            <div className="flex items-center gap-2">
                              <Badge variant={model.aktif ? 'success' : 'secondary'} className="text-xs">
                                {model.aktif ? 'Aktif' : 'Pasif'}
                              </Badge>
                              <button
                                onClick={() => openModelModal(marka.id, model)}
                                className="p-1 text-secondary-400 hover:text-primary-600"
                              >
                                <Edit className="w-3.5 h-3.5" />
                              </button>
                              <button
                                onClick={() => setDeleteModel(model)}
                                className="p-1 text-secondary-400 hover:text-danger"
                              >
                                <Trash2 className="w-3.5 h-3.5" />
                              </button>
                            </div>
                          </div>
                        ))}
                      </div>
                    ) : (
                      <div className="px-4 pb-4 text-sm text-secondary-500 text-center">
                        Bu markaya ait model bulunmuyor
                      </div>
                    )}
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </Card>

      {/* Marka Modal */}
      <Modal
        isOpen={showMarkaModal}
        onClose={() => setShowMarkaModal(false)}
        title={editingMarka ? 'Marka Düzenle' : 'Yeni Marka'}
        size="md"
        footer={
          <>
            <button onClick={() => setShowMarkaModal(false)} className="btn-secondary">İptal</button>
            <button onClick={markaForm.handleSubmit(onMarkaSubmit)} className="btn-primary" disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          <div>
            <label className="label">Marka Adı *</label>
            <input
              {...markaForm.register('ad', { required: 'Marka adı gerekli' })}
              className={markaForm.formState.errors.ad ? 'input-error' : 'input'}
              placeholder="örn: Apple, Samsung"
            />
            {markaForm.formState.errors.ad && (
              <p className="text-sm text-danger mt-1">{markaForm.formState.errors.ad.message}</p>
            )}
          </div>
          <div>
            <label className="label">Sıra</label>
            <input
              type="number"
              {...markaForm.register('sira', { valueAsNumber: true })}
              className="input"
            />
          </div>
        </form>
      </Modal>

      {/* Model Modal */}
      <Modal
        isOpen={showModelModal}
        onClose={() => setShowModelModal(false)}
        title={editingModel ? 'Model Düzenle' : 'Yeni Model'}
        size="md"
        footer={
          <>
            <button onClick={() => setShowModelModal(false)} className="btn-secondary">İptal</button>
            <button onClick={modelForm.handleSubmit(onModelSubmit)} className="btn-primary" disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          <div>
            <label className="label">Model Adı *</label>
            <input
              {...modelForm.register('ad', { required: 'Model adı gerekli' })}
              className={modelForm.formState.errors.ad ? 'input-error' : 'input'}
              placeholder="örn: iPhone 15 Pro, Galaxy S24"
            />
            {modelForm.formState.errors.ad && (
              <p className="text-sm text-danger mt-1">{modelForm.formState.errors.ad.message}</p>
            )}
          </div>
          <div>
            <label className="label">Model Kodu</label>
            <input
              {...modelForm.register('kod')}
              className="input"
              placeholder="örn: A3102, SM-S921B"
            />
          </div>
          <div>
            <label className="label">Sıra</label>
            <input
              type="number"
              {...modelForm.register('sira', { valueAsNumber: true })}
              className="input"
            />
          </div>
          <input type="hidden" {...modelForm.register('markaId')} value={selectedMarkaId} />
        </form>
      </Modal>

      {/* Delete Marka Confirm */}
      <ConfirmDialog
        isOpen={!!deleteMarka}
        onClose={() => setDeleteMarka(null)}
        onConfirm={handleDeleteMarka}
        title="Markayı Sil"
        message={`"${deleteMarka?.ad}" markasını silmek istediğinize emin misiniz? Bu markaya ait modeller de silinecektir.`}
        confirmText="Sil"
      />

      {/* Delete Model Confirm */}
      <ConfirmDialog
        isOpen={!!deleteModel}
        onClose={() => setDeleteModel(null)}
        onConfirm={handleDeleteModel}
        title="Modeli Sil"
        message={`"${deleteModel?.ad}" modelini silmek istediğinize emin misiniz?`}
        confirmText="Sil"
      />
    </div>
  );
}
