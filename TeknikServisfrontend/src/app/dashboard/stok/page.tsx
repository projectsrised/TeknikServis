'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Search, Package, AlertTriangle, ArrowDownToLine, ArrowUpFromLine, Boxes, FileText, Trash2 } from 'lucide-react';
import { Card, CardHeader, Modal, Badge, PageLoading, Tabs } from '@/components/ui';
import { DataTable, Column } from '@/components/ui/DataTable';
import { stokApi, urunApi, depoApi } from '@/lib/api';
import { StokDurum, Urun, Depo } from '@/types';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';

interface StokGirisForm {
  urunId: string;
  depoId: string;
  seriNumaralari: string;
  alisFiyati: number;
}

export default function StokPage() {
  const router = useRouter();
  const [loading, setLoading] = useState(true);
  const [stoklar, setStoklar] = useState<StokDurum[]>([]);
  const [kritikStoklar, setKritikStoklar] = useState<StokDurum[]>([]);
  const [urunler, setUrunler] = useState<Urun[]>([]);
  const [depolar, setDepolar] = useState<Depo[]>([]);
  const [activeTab, setActiveTab] = useState('all');
  const [showGirisModal, setShowGirisModal] = useState(false);
  const [saving, setSaving] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [selectedStok, setSelectedStok] = useState<StokDurum | null>(null);
  const [deleting, setDeleting] = useState(false);

  const { register, handleSubmit, reset, formState: { errors } } = useForm<StokGirisForm>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [stokRes, kritikRes, urunRes] = await Promise.all([
        stokApi.getDurum(),
        stokApi.getKritik(),
        urunApi.getList(1, 100),
      ]);

      if (stokRes.data.basarili) setStoklar(stokRes.data.data);
      if (kritikRes.data.basarili) setKritikStoklar(kritikRes.data.data);
      if (urunRes.data.basarili) setUrunler(urunRes.data.data.items);
    } catch (error) {
      toast.error('Stok verileri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleStokGiris = async (formData: StokGirisForm) => {
    setSaving(true);
    try {
      const seriNumaralari = formData.seriNumaralari
        .split('\n')
        .map(s => s.trim())
        .filter(s => s.length > 0);

      if (seriNumaralari.length === 0) {
        toast.error('En az bir seri numarası girin');
        return;
      }

      const response = await stokApi.giris({
        urunId: formData.urunId,
        depoId: formData.depoId,
        seriNumaralari,
        alisFiyati: formData.alisFiyati,
      });

      if (response.data.basarili) {
        toast.success(`${seriNumaralari.length} adet stok girişi yapıldı`);
        setShowGirisModal(false);
        reset();
        loadData();
      } else {
        toast.error(response.data.mesaj);
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Stok girişi başarısız');
    } finally {
      setSaving(false);
    }
  };

  const handleDeleteStok = (stok: StokDurum) => {
    setSelectedStok(stok);
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    if (!selectedStok) return;
    
    setDeleting(true);
    try {
      const response = await stokApi.deleteUrunStoklari(selectedStok.urunId);
      if (response.data.basarili) {
        toast.success(response.data.mesaj || `${response.data.data} adet stok kalemi silindi`);
        setShowDeleteModal(false);
        setSelectedStok(null);
        loadData();
      } else {
        toast.error(response.data.mesaj || 'Stok silme başarısız');
      }
    } catch (error: any) {
      toast.error(error.response?.data?.mesaj || 'Stok silme başarısız');
    } finally {
      setDeleting(false);
    }
  };

  const displayData = activeTab === 'kritik' ? kritikStoklar : stoklar;

  const columns: Column<StokDurum>[] = [
    {
      key: 'urun',
      header: 'Ürün',
      render: (item) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-secondary-100 rounded-lg flex items-center justify-center">
            <Package className="w-5 h-5 text-secondary-500" />
          </div>
          <div>
            <p className="font-medium text-secondary-900">{item.urunAd}</p>
            <p className="text-xs text-secondary-500 font-mono">{item.barkod}</p>
          </div>
        </div>
      ),
    },
    {
      key: 'toplamStok',
      header: 'Toplam Stok',
      render: (item) => (
        <div className="flex items-center gap-2">
          <span className={`text-lg font-bold ${item.kritikStokAlti ? 'text-danger' : 'text-secondary-900'}`}>
            {item.toplamStok}
          </span>
          {item.kritikStokAlti && <AlertTriangle className="w-4 h-4 text-danger" />}
        </div>
      ),
    },
    {
      key: 'kritikStok',
      header: 'Kritik Seviye',
      render: (item) => (
        <span className="text-secondary-600">{item.kritikStok}</span>
      ),
    },
    {
      key: 'depoler',
      header: 'Depo Dağılımı',
      render: (item) => (
        <div className="flex flex-wrap gap-1">
          {item.depoBazliStoklar?.map((depo) => (
            <Badge key={depo.depoId} variant="secondary">
              {depo.depoAd}: {depo.miktar}
            </Badge>
          ))}
        </div>
      ),
    },
    {
      key: 'durum',
      header: 'Durum',
      render: (item) =>
        item.kritikStokAlti ? (
          <Badge variant="danger">Kritik</Badge>
        ) : (
          <Badge variant="success">Normal</Badge>
        ),
    },
    {
      key: 'actions',
      header: '',
      className: 'w-20',
      render: (item) => (
        <div className="flex items-center justify-end">
          {item.toplamStok > 0 && (
            <button
              onClick={(e) => {
                e.stopPropagation();
                handleDeleteStok(item);
              }}
              className="p-1.5 text-secondary-500 hover:text-danger hover:bg-red-50 rounded"
              title="Stokları Sil"
            >
              <Trash2 className="w-4 h-4" />
            </button>
          )}
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Stok Yönetimi</h1>
          <p className="text-secondary-500">Depo stoklarını takip edin ve yönetin</p>
        </div>
        <div className="flex gap-2">
          <button onClick={() => setShowGirisModal(true)} className="btn-secondary">
            <ArrowDownToLine className="w-4 h-4 mr-2" />
            Hızlı Giriş
          </button>
          <button onClick={() => router.push('/dashboard/stok/giris')} className="btn-primary">
            <FileText className="w-4 h-4 mr-2" />
            Faturalı Giriş
          </button>
        </div>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-primary-100 rounded-lg flex items-center justify-center">
              <Boxes className="w-6 h-6 text-primary-600" />
            </div>
            <div>
              <p className="text-2xl font-bold text-secondary-900">
                {stoklar.reduce((acc, s) => acc + s.toplamStok, 0)}
              </p>
              <p className="text-sm text-secondary-500">Toplam Stok</p>
            </div>
          </div>
        </Card>
        <Card>
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
              <Package className="w-6 h-6 text-green-600" />
            </div>
            <div>
              <p className="text-2xl font-bold text-secondary-900">{stoklar.length}</p>
              <p className="text-sm text-secondary-500">Ürün Çeşidi</p>
            </div>
          </div>
        </Card>
        <Card>
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
              <AlertTriangle className="w-6 h-6 text-red-600" />
            </div>
            <div>
              <p className="text-2xl font-bold text-danger">{kritikStoklar.length}</p>
              <p className="text-sm text-secondary-500">Kritik Stok</p>
            </div>
          </div>
        </Card>
      </div>

      {/* Table */}
      <Card padding={false}>
        <div className="p-4 border-b border-secondary-200">
          <Tabs
            tabs={[
              { id: 'all', label: 'Tüm Stoklar', count: stoklar.length },
              { id: 'kritik', label: 'Kritik Stoklar', count: kritikStoklar.length },
            ]}
            activeTab={activeTab}
            onChange={setActiveTab}
          />
        </div>

        {loading ? (
          <PageLoading />
        ) : (
          <DataTable
            columns={columns}
            data={displayData}
            keyExtractor={(item) => item.urunId}
            emptyMessage="Stok bulunamadı"
          />
        )}
      </Card>

      {/* Stok Giriş Modal */}
      <Modal
        isOpen={showGirisModal}
        onClose={() => setShowGirisModal(false)}
        title="Stok Girişi"
        size="md"
        footer={
          <>
            <button onClick={() => setShowGirisModal(false)} className="btn-secondary">İptal</button>
            <button onClick={handleSubmit(handleStokGiris)} className="btn-primary" disabled={saving}>
              {saving ? 'Kaydediliyor...' : 'Stok Girişi Yap'}
            </button>
          </>
        }
      >
        <form className="space-y-4">
          <div>
            <label className="label">Ürün *</label>
            <select {...register('urunId', { required: 'Ürün seçin' })} className="select">
              <option value="">Ürün Seçin</option>
              {urunler.map((u) => (
                <option key={u.id} value={u.id}>{u.ad} - {u.barkod}</option>
              ))}
            </select>
            {errors.urunId && <p className="text-sm text-danger mt-1">{errors.urunId.message}</p>}
          </div>

          <div>
            <label className="label">Depo *</label>
            <select {...register('depoId', { required: 'Depo seçin' })} className="select">
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
            <label className="label">Seri Numaraları * (Her satıra bir tane)</label>
            <textarea
              {...register('seriNumaralari', { required: 'Seri numaraları girin' })}
              className="input font-mono"
              rows={6}
              placeholder="SN001&#10;SN002&#10;SN003"
            />
            {errors.seriNumaralari && <p className="text-sm text-danger mt-1">{errors.seriNumaralari.message}</p>}
          </div>
        </form>
      </Modal>

      {/* Delete Modal */}
      <Modal
        isOpen={showDeleteModal}
        onClose={() => {
          setShowDeleteModal(false);
          setSelectedStok(null);
        }}
        title="Stokları Sil"
        size="sm"
        footer={
          <>
            <button 
              onClick={() => {
                setShowDeleteModal(false);
                setSelectedStok(null);
              }} 
              className="btn-secondary"
              disabled={deleting}
            >
              İptal
            </button>
            <button 
              onClick={handleDeleteConfirm} 
              className="btn-danger" 
              disabled={deleting}
            >
              {deleting ? 'Siliniyor...' : 'Sil'}
            </button>
          </>
        }
      >
        <div className="space-y-3">
          <p className="text-secondary-700">
            <strong>{selectedStok?.urunAd}</strong> ürününün tüm stok kalemlerini silmek istediğinize emin misiniz?
          </p>
          <div className="bg-secondary-50 p-3 rounded-lg">
            <p className="text-sm text-secondary-600">
              <strong>Toplam Stok:</strong> {selectedStok?.toplamStok} adet
            </p>
            {selectedStok?.depoBazliStoklar && selectedStok.depoBazliStoklar.length > 0 && (
              <div className="mt-2">
                <p className="text-sm text-secondary-600 font-medium mb-1">Depo Dağılımı:</p>
                <div className="space-y-1">
                  {selectedStok.depoBazliStoklar.map((depo) => (
                    <p key={depo.depoId} className="text-xs text-secondary-500">
                      • {depo.depoAd}: {depo.miktar} adet
                    </p>
                  ))}
                </div>
              </div>
            )}
          </div>
          <p className="text-xs text-danger">
            ⚠️ Bu işlem geri alınamaz. Satılmamış tüm stok kalemleri silinecektir.
          </p>
        </div>
      </Modal>
    </div>
  );
}
