'use client';

import { useEffect, useState, useCallback, useRef } from 'react';
import { useRouter } from 'next/navigation';
import { ArrowLeft, Plus, Trash2, Package, Search, Save, Calendar, FileText, User, Building2, Palette, Globe, Factory, X } from 'lucide-react';
import { Card, PageLoading, Modal } from '@/components/ui';
import { stokApi, bayiApi, depoApi, urunApi, musteriApi } from '@/lib/api';
import { Musteri, Bayi, Depo, Urun } from '@/types';
import { useForm, useFieldArray } from 'react-hook-form';
import toast from 'react-hot-toast';
import { useAuthStore } from '@/store/authStore';

interface StokGirisForm {
  tedarikciId: string;
  bayiId: string;
  depoId: string;
  faturaNo: string;
  faturaTarihi: string;
  aciklama: string;
  kalemler: {
    urunId: string;
    renk: string;
    adet: number;
    kdvOran: number;
    alisFiyati: number;
    satisFiyati: number;
    barkod: string;
    uretimTarihi: string;
    mensei: string;
  }[];
}

interface TedarikciForm {
  ad: string;
  soyad: string;
  telefon: string;
  email: string;
  adres: string;
  kurumsal: boolean;
}

// Renk listesi
const RENKLER = [
  'Siyah',
  'Beyaz',
  'Gri',
  'Gümüş',
  'Altın',
  'Kırmızı',
  'Mavi',
  'Yeşil',
  'Sarı',
  'Turuncu',
  'Pembe',
  'Mor',
  'Kahverengi',
  'Lacivert',
  'Turkuaz',
  'Bordo',
  'Bej',
  'Krem',
  'Diğer'
];

export default function StokGirisPage() {
  const router = useRouter();
  const user = useAuthStore((state) => state.user);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [tedarikciler, setTedarikciler] = useState<Musteri[]>([]);
  const [bayiler, setBayiler] = useState<Bayi[]>([]);
  const [depolar, setDepolar] = useState<Depo[]>([]);
  const [urunler, setUrunler] = useState<Urun[]>([]);
  const [merkezBayi, setMerkezBayi] = useState<Bayi | null>(null);

  // Tedarikçi Modal
  const [showTedarikciModal, setShowTedarikciModal] = useState(false);
  const [savingTedarikci, setSavingTedarikci] = useState(false);
  const [tedarikciForm, setTedarikciForm] = useState<TedarikciForm>({
    ad: '',
    soyad: '',
    telefon: '',
    email: '',
    adres: '',
    kurumsal: false
  });

  // Urun arama - her satır için ayrı state
  const [searchTexts, setSearchTexts] = useState<{ [key: number]: string }>({});
  const [activeSearchIndex, setActiveSearchIndex] = useState<number | null>(null);
  const [filteredUrunler, setFilteredUrunler] = useState<Urun[]>([]);
  const [selectedUrunler, setSelectedUrunler] = useState<{ [key: number]: Urun | null }>({});
  const [dropdownPosition, setDropdownPosition] = useState<{ top: number; left: number; width: number } | null>(null);
  const searchInputRefs = useRef<{ [key: number]: HTMLInputElement | null }>({});

  const { register, handleSubmit, watch, setValue, control } = useForm<StokGirisForm>({
    defaultValues: {
      tedarikciId: '',
      bayiId: '',
      depoId: '',
      faturaNo: '',
      faturaTarihi: new Date().toISOString().split('T')[0],
      aciklama: '',
      kalemler: [{ urunId: '', renk: '', adet: 1, kdvOran: 20, alisFiyati: 0, satisFiyati: 0, barkod: '', uretimTarihi: '', mensei: '' }]
    }
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'kalemler'
  });

  const selectedBayiId = watch('bayiId');
  const kalemler = watch('kalemler');

  useEffect(() => {
    loadInitialData();
  }, []);

  useEffect(() => {
    if (selectedBayiId) {
      loadDepolar(selectedBayiId);
    } else {
      setDepolar([]);
      setValue('depoId', '');
    }
  }, [selectedBayiId]);

  // Dropdown pozisyonunu scroll ve resize'da güncelle
  useEffect(() => {
    if (activeSearchIndex !== null && dropdownPosition) {
      const updatePosition = () => {
        const input = searchInputRefs.current[activeSearchIndex];
        if (input) {
          const inputRect = input.getBoundingClientRect();
          setDropdownPosition({
            top: inputRect.bottom + window.scrollY + 4,
            left: inputRect.left + window.scrollX,
            width: inputRect.width
          });
        }
      };

      window.addEventListener('scroll', updatePosition, true);
      window.addEventListener('resize', updatePosition);

      return () => {
        window.removeEventListener('scroll', updatePosition, true);
        window.removeEventListener('resize', updatePosition);
      };
    }
  }, [activeSearchIndex, dropdownPosition]);

  const loadInitialData = async () => {
    try {
      setLoading(true);
      const [tedarikciRes, bayiRes, urunRes] = await Promise.all([
        stokApi.getTedarikciler(),
        bayiApi.getAll(),
        urunApi.getList(1, 500),
      ]);

      if (tedarikciRes.data.basarili) setTedarikciler(tedarikciRes.data.data || []);
      
      // Ürün listesi yükleme - farklı response formatlarını kontrol et
      if (urunRes.data) {
        const urunData = urunRes.data.basarili 
          ? (urunRes.data.data?.items || urunRes.data.data || [])
          : (urunRes.data.items || urunRes.data || []);
        setUrunler(Array.isArray(urunData) ? urunData : []);
        console.log('Yüklenen ürün sayısı:', urunData.length);
      }

      // Bayileri ayarla ve merkez bayiyi bul
      if (bayiRes.data) {
        const bayiList = Array.isArray(bayiRes.data) ? bayiRes.data : bayiRes.data.data || [];
        setBayiler(bayiList);

        // Merkez bayi (merkezMi = true olan)
        const merkez = bayiList.find((b: Bayi) => b.merkezMi);
        if (merkez) {
          setMerkezBayi(merkez);
          setValue('bayiId', merkez.id);
          await loadDepolar(merkez.id);
        } else if (bayiList.length > 0) {
          setValue('bayiId', bayiList[0].id);
          await loadDepolar(bayiList[0].id);
        }
      }
    } catch (error) {
      console.error('Veri yükleme hatası:', error);
      toast.error('Veriler yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const loadDepolar = async (bayiId: string) => {
    try {
      const response = await depoApi.getByBayiId(bayiId);
      if (response.data) {
        const depoList = Array.isArray(response.data) ? response.data : response.data.data || [];
        setDepolar(depoList);
        const merkezDepo = depoList.find((d: Depo) => d.merkezDepoMu);
        if (merkezDepo) {
          setValue('depoId', merkezDepo.id);
        } else if (depoList.length > 0) {
          setValue('depoId', depoList[0].id);
        }
      }
    } catch (error) {
      console.error('Depo yükleme hatası:', error);
      setDepolar([]);
    }
  };

  // Ürün arama - düzeltilmiş versiyon
  const handleUrunSearch = useCallback((search: string, index: number) => {
    setSearchTexts(prev => ({ ...prev, [index]: search }));
    
    if (search.length >= 2 && urunler.length > 0) {
      const searchLower = search.toLowerCase();
      const filtered = urunler.filter(u =>
        (u.ad && u.ad.toLowerCase().includes(searchLower)) ||
        (u.kod && u.kod.toLowerCase().includes(searchLower)) ||
        (u.barkod && u.barkod.toString().includes(search))
      );
      const limited = filtered.slice(0, 10);
      
      // State'leri aynı anda güncelle
      setFilteredUrunler(limited);
      setActiveSearchIndex(index); // Dropdown'ı göster
      
      console.log(`Arama: "${search}" - Bulunan: ${filtered.length} ürün, Gösterilen: ${limited.length}`);
      console.log('Active Search Index set to:', index, 'Filtered count:', limited.length);
    } else {
      setFilteredUrunler([]);
      if (search.length === 0) {
        setActiveSearchIndex(null);
      } else {
        // 2 karakterden az ama yazılıyorsa index'i aktif tut
        setActiveSearchIndex(index);
      }
    }
  }, [urunler]);

  const selectUrun = (urun: Urun, index: number) => {
    setValue(`kalemler.${index}.urunId`, urun.id);
    setValue(`kalemler.${index}.alisFiyati`, urun.alisFiyat || 0);
    setValue(`kalemler.${index}.satisFiyati`, urun.satisFiyat || 0);
    setValue(`kalemler.${index}.kdvOran`, urun.kdvOran || 20);
    setSelectedUrunler(prev => ({ ...prev, [index]: urun }));
    setActiveSearchIndex(null);
    setDropdownPosition(null);
    setSearchTexts(prev => ({ ...prev, [index]: '' }));
    setFilteredUrunler([]);
  };

  const clearUrunSelection = (index: number) => {
    setValue(`kalemler.${index}.urunId`, '');
    setValue(`kalemler.${index}.alisFiyati`, 0);
    setValue(`kalemler.${index}.satisFiyati`, 0);
    setSelectedUrunler(prev => {
      const updated = { ...prev };
      delete updated[index];
      return updated;
    });
  };

  // Tedarikçi kaydetme
  const handleSaveTedarikci = async () => {
    if (!tedarikciForm.ad.trim()) {
      toast.error('Ad zorunludur');
      return;
    }
    if (!tedarikciForm.telefon.trim()) {
      toast.error('Telefon zorunludur');
      return;
    }
    if (!tedarikciForm.email.trim()) {
      toast.error('E-posta zorunludur');
      return;
    }

    setSavingTedarikci(true);
    try {
      const response = await musteriApi.create({
        ad: tedarikciForm.ad,
        soyad: tedarikciForm.soyad || (tedarikciForm.kurumsal ? '' : 'Yok'),
        telefon: tedarikciForm.telefon,
        email: tedarikciForm.email,
        adres: tedarikciForm.adres || undefined,
        kurumsal: tedarikciForm.kurumsal,
        tedarikci: true // Tedarikçi olarak işaretle
      });

      if (response.data.basarili || response.data.id) {
        const yeniTedarikci = response.data.data || response.data;
        setTedarikciler(prev => [...prev, yeniTedarikci]);
        setValue('tedarikciId', yeniTedarikci.id);
        toast.success('Tedarikçi eklendi');
        setShowTedarikciModal(false);
        setTedarikciForm({ ad: '', soyad: '', telefon: '', email: '', adres: '', kurumsal: false });
      } else {
        toast.error(response.data.mesaj || 'Tedarikçi eklenemedi');
      }
    } catch (error: any) {
      console.error('Tedarikçi kaydetme hatası:', error);
      toast.error(error.response?.data?.mesaj || 'Tedarikçi eklenemedi');
    } finally {
      setSavingTedarikci(false);
    }
  };

  const onSubmit = async (data: StokGirisForm) => {
    if (!data.tedarikciId) {
      toast.error('Tedarikçi seçin');
      return;
    }
    if (!data.bayiId) {
      toast.error('Bayi seçin');
      return;
    }
    if (!data.depoId) {
      toast.error('Depo seçin');
      return;
    }
    if (data.kalemler.length === 0) {
      toast.error('En az bir ürün ekleyin');
      return;
    }

    for (let i = 0; i < data.kalemler.length; i++) {
      const kalem = data.kalemler[i];
      if (!kalem.urunId) {
        toast.error(`${i + 1}. satırda ürün seçin`);
        return;
      }
      if (kalem.adet < 1) {
        toast.error(`${i + 1}. satırda adet en az 1 olmalı`);
        return;
      }
    }

    setSaving(true);
    try {
      const payload = {
        tedarikciId: data.tedarikciId,
        bayiId: data.bayiId,
        depoId: data.depoId,
        faturaNo: data.faturaNo || undefined,
        faturaTarihi: data.faturaTarihi ? new Date(data.faturaTarihi).toISOString() : undefined,
        aciklama: data.aciklama || undefined,
        kalemler: data.kalemler.map(k => ({
          urunId: k.urunId,
          alisFiyati: k.alisFiyati,
          satisFiyati: k.satisFiyati,
          adet: k.adet,
          barkod: k.barkod || undefined,
          renk: k.renk || undefined,
          uretimTarihi: k.uretimTarihi ? new Date(k.uretimTarihi).toISOString() : undefined,
          mensei: k.mensei || undefined
        }))
      };

      const response = await stokApi.girisWithFatura(payload);

      if (response.data.basarili) {
        const sonuc = response.data.data;
        toast.success(`Stok girişi tamamlandı! Fatura No: ${sonuc.faturaNo}, Toplam: ${sonuc.toplamAdet} adet`);

        if (sonuc.olusturulanBarkodlar && sonuc.olusturulanBarkodlar.length > 0) {
          toast.success(`Otomatik barkodlar oluşturuldu: ${sonuc.olusturulanBarkodlar.length} adet`);
        }

        router.push('/dashboard/stok');
      } else {
        toast.error(response.data.mesaj || 'Stok girişi başarısız');
      }
    } catch (error: any) {
      console.error('Stok girişi hatası:', error);
      toast.error(error.response?.data?.mesaj || 'Stok girişi başarısız');
    } finally {
      setSaving(false);
    }
  };

  const addKalem = () => {
    append({ urunId: '', renk: '', adet: 1, kdvOran: 20, alisFiyati: 0, satisFiyati: 0, barkod: '', uretimTarihi: '', mensei: '' });
  };

  const removeKalem = (index: number) => {
    if (fields.length > 1) {
      remove(index);
      setSelectedUrunler(prev => {
        const updated = { ...prev };
        delete updated[index];
        return updated;
      });
      setSearchTexts(prev => {
        const updated = { ...prev };
        delete updated[index];
        return updated;
      });
    }
  };

  // Hesaplamalar
  const calculateTotals = () => {
    let toplamMaliyet = 0;
    let toplamSatis = 0;
    let toplamKDV = 0;

    kalemler.forEach(k => {
      const maliyet = (k.alisFiyati || 0) * (k.adet || 0);
      const satis = (k.satisFiyati || 0) * (k.adet || 0);
      const kdv = maliyet * ((k.kdvOran || 0) / 100);

      toplamMaliyet += maliyet;
      toplamSatis += satis;
      toplamKDV += kdv;
    });

    const kar = toplamSatis - toplamMaliyet;

    return { toplamMaliyet, toplamSatis, toplamKDV, kar };
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  if (loading) {
    return <PageLoading />;
  }

  const totals = calculateTotals();
  const selectedTedarikci = tedarikciler.find(t => t.id === watch('tedarikciId'));
  const selectedDepo = depolar.find(d => d.id === watch('depoId'));

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex items-center gap-4">
        <button onClick={() => router.back()} className="p-2 hover:bg-secondary-100 rounded-lg">
          <ArrowLeft className="w-5 h-5" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Stok Girişi (Fatura)</h1>
          <p className="text-secondary-500">
            Tedarikçiden alınan ürünleri stoğa girin
            {merkezBayi && <span className="text-primary-600 ml-1">• Merkez: {merkezBayi.ad}</span>}
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Card padding={false}>
          {/* Fatura Üst Bilgileri */}
          <div className="p-4 border-b border-secondary-200 bg-secondary-50">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              {/* Cari Seçimi */}
              <div>
                <label className="label flex items-center gap-1">
                  <User className="w-4 h-4" />
                  Cari Seçiniz *
                </label>
                <div className="flex gap-2">
                  <select {...register('tedarikciId')} className="select flex-1">
                    <option value="">Tedarikçi Seçin</option>
                    {tedarikciler.map(t => (
                      <option key={t.id} value={t.id}>{t.adSoyad}</option>
                    ))}
                  </select>
                  <button
                    type="button"
                    onClick={() => setShowTedarikciModal(true)}
                    className="btn-secondary px-3"
                    title="Yeni Tedarikçi Ekle"
                  >
                    <Plus className="w-4 h-4" />
                  </button>
                </div>
                {selectedTedarikci && (
                  <p className="text-xs text-secondary-500 mt-1">{selectedTedarikci.telefon}</p>
                )}
              </div>

              {/* Fatura No */}
              <div>
                <label className="label flex items-center gap-1">
                  <FileText className="w-4 h-4" />
                  Fatura No
                </label>
                <input
                  type="text"
                  {...register('faturaNo')}
                  className="input"
                  placeholder="Otomatik"
                />
              </div>

              {/* Fatura Tarihi */}
              <div>
                <label className="label flex items-center gap-1">
                  <Calendar className="w-4 h-4" />
                  Fatura Tarihi
                </label>
                <input
                  type="date"
                  {...register('faturaTarihi')}
                  className="input"
                />
              </div>

              {/* Şube/Depo */}
              <div>
                <label className="label flex items-center gap-1">
                  <Building2 className="w-4 h-4" />
                  Şube / Depo *
                </label>
                <div className="flex gap-2">
                  <select {...register('bayiId')} className="select">
                    <option value="">Bayi</option>
                    {bayiler.map(b => (
                      <option key={b.id} value={b.id}>
                        {b.ad} {b.merkezMi && '(Merkez)'}
                      </option>
                    ))}
                  </select>
                  <select {...register('depoId')} className="select">
                    <option value="">Depo</option>
                    {depolar.map(d => (
                      <option key={d.id} value={d.id}>
                        {d.ad} {d.merkezDepoMu && '(Merkez)'}
                      </option>
                    ))}
                  </select>
                </div>
                {selectedDepo?.merkezDepoMu && (
                  <p className="text-xs text-green-600 mt-1">Merkez depoya ekleniyor</p>
                )}
              </div>
            </div>
          </div>

          {/* Tablo Başlığı */}
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="bg-secondary-800 text-white text-sm">
                  <th className="px-3 py-3 text-left font-medium min-w-[200px]">STOK</th>
                  <th className="px-3 py-3 text-left font-medium w-24">
                    <div className="flex items-center gap-1">
                      <Palette className="w-3 h-3" />
                      RENK
                    </div>
                  </th>
                  <th className="px-3 py-3 text-center font-medium w-20">ADET</th>
                  <th className="px-3 py-3 text-center font-medium w-20">KDV %</th>
                  <th className="px-3 py-3 text-right font-medium w-28">MALİYET</th>
                  <th className="px-3 py-3 text-right font-medium w-28">SATIŞ FİYATI</th>
                  <th className="px-3 py-3 text-left font-medium w-28">
                    <div className="flex items-center gap-1">
                      <Factory className="w-3 h-3" />
                      ÜRETİM T.
                    </div>
                  </th>
                  <th className="px-3 py-3 text-left font-medium w-24">
                    <div className="flex items-center gap-1">
                      <Globe className="w-3 h-3" />
                      MENŞEİ
                    </div>
                  </th>
                  <th className="px-3 py-3 text-left font-medium w-28">BARKOD</th>
                  <th className="px-3 py-3 text-center font-medium w-16">İŞLEM</th>
                </tr>
              </thead>
              <tbody>
                {fields.map((field, index) => (
                  <tr key={field.id} className="border-b border-secondary-100 hover:bg-secondary-50">
                    {/* Stok/Ürün Seçimi */}
                    <td className="px-3 py-2">
                      {selectedUrunler[index] ? (
                        <div className="flex items-center gap-2">
                          <Package className="w-4 h-4 text-secondary-400 flex-shrink-0" />
                          <div className="min-w-0 flex-1">
                            <p className="font-medium text-secondary-900 text-sm truncate">{selectedUrunler[index]?.ad}</p>
                            <p className="text-xs text-secondary-500">{selectedUrunler[index]?.kod}</p>
                          </div>
                          <button
                            type="button"
                            onClick={() => clearUrunSelection(index)}
                            className="text-secondary-400 hover:text-danger flex-shrink-0 p-1"
                          >
                            <X className="w-4 h-4" />
                          </button>
                        </div>
                      ) : (
                        <div className="relative">
                          <div className="relative">
                            <Search className="absolute left-2 top-1/2 -translate-y-1/2 w-3 h-3 text-secondary-400 pointer-events-none z-10" />
                            <input
                              ref={(el) => { searchInputRefs.current[index] = el; }}
                              type="text"
                              className="input input-sm pl-7 w-full"
                              placeholder="Ürün ara... (min 2 karakter)"
                              value={searchTexts[index] || ''}
                              onChange={(e) => {
                                const value = e.target.value;
                                // Input pozisyonunu güncelle
                                const inputRect = e.currentTarget.getBoundingClientRect();
                                setDropdownPosition({
                                  top: inputRect.bottom + window.scrollY + 4,
                                  left: inputRect.left + window.scrollX,
                                  width: inputRect.width
                                });
                                handleUrunSearch(value, index);
                              }}
                              onFocus={(e) => {
                                setActiveSearchIndex(index);
                                // Input pozisyonunu hesapla
                                const inputRect = e.currentTarget.getBoundingClientRect();
                                setDropdownPosition({
                                  top: inputRect.bottom + window.scrollY + 4,
                                  left: inputRect.left + window.scrollX,
                                  width: inputRect.width
                                });
                                if ((searchTexts[index] || '').length >= 2) {
                                  handleUrunSearch(searchTexts[index] || '', index);
                                }
                              }}
                              onBlur={(e) => {
                                // Dropdown'a tıklanıyorsa blur'u engelle
                                const relatedTarget = e.relatedTarget as HTMLElement;
                                if (relatedTarget && relatedTarget.closest('.urun-dropdown')) {
                                  console.log('Blur engellendi - dropdown tıklanıyor');
                                  return;
                                }
                                // Blur'u geciktir ki dropdown tıklaması çalışsın
                                setTimeout(() => {
                                  // Eğer hala aynı index aktifse ve dropdown'a tıklanmadıysa kapat
                                  if (activeSearchIndex === index) {
                                    const activeElement = document.activeElement;
                                    if (!activeElement || !activeElement.closest('.urun-dropdown')) {
                                      console.log('Dropdown kapatılıyor - blur timeout');
                                      setActiveSearchIndex(null);
                                      setDropdownPosition(null);
                                    }
                                  }
                                }, 250);
                              }}
                              onKeyDown={(e) => {
                                if (e.key === 'Escape') {
                                  setActiveSearchIndex(null);
                                  setDropdownPosition(null);
                                  setFilteredUrunler([]);
                                }
                              }}
                            />
                          </div>
                          {activeSearchIndex === index && filteredUrunler.length > 0 && dropdownPosition && (
                            <div 
                              className="urun-dropdown fixed bg-white border-2 border-primary-200 rounded-lg shadow-2xl max-h-48 overflow-auto"
                              style={{ 
                                position: 'fixed',
                                top: `${dropdownPosition.top}px`,
                                left: `${dropdownPosition.left}px`,
                                width: `${dropdownPosition.width}px`,
                                zIndex: 99999,
                                maxHeight: '12rem'
                              }}
                              onMouseDown={(e) => {
                                // Prevent input blur when clicking dropdown
                                e.preventDefault();
                                e.stopPropagation();
                                console.log('Dropdown mouseDown - blur engellendi');
                              }}
                            >
                              {filteredUrunler.map(urun => (
                                <button
                                  key={urun.id}
                                  type="button"
                                  onClick={(e) => {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    selectUrun(urun, index);
                                  }}
                                  className="w-full p-2 text-left hover:bg-secondary-50 flex items-center gap-2 border-b border-secondary-100 last:border-b-0 cursor-pointer"
                                >
                                  <Package className="w-4 h-4 text-secondary-400 flex-shrink-0" />
                                  <div className="min-w-0 flex-1">
                                    <p className="font-medium text-secondary-900 text-sm truncate">{urun.ad}</p>
                                    <p className="text-xs text-secondary-500">
                                      {urun.kod || 'Kod yok'} | Alış: {formatCurrency(urun.alisFiyat || 0)}
                                    </p>
                                  </div>
                                </button>
                              ))}
                            </div>
                          )}
                          {activeSearchIndex === index && (searchTexts[index] || '').length >= 2 && filteredUrunler.length === 0 && dropdownPosition && (
                            <div 
                              className="urun-dropdown fixed bg-white border border-secondary-200 rounded-lg shadow-xl p-3 text-sm text-secondary-500"
                              style={{ 
                                position: 'fixed',
                                top: `${dropdownPosition.top}px`,
                                left: `${dropdownPosition.left}px`,
                                width: `${dropdownPosition.width}px`,
                                zIndex: 99999
                              }}
                              onMouseDown={(e) => {
                                e.preventDefault();
                              }}
                            >
                              Ürün bulunamadı
                            </div>
                          )}
                        </div>
                      )}
                      <input type="hidden" {...register(`kalemler.${index}.urunId`)} />
                    </td>

                    {/* Renk */}
                    <td className="px-3 py-2">
                      <select
                        {...register(`kalemler.${index}.renk`)}
                        className="select input-sm w-full text-xs"
                      >
                        <option value="">Renk Seçin</option>
                        {RENKLER.map(renk => (
                          <option key={renk} value={renk}>{renk}</option>
                        ))}
                      </select>
                    </td>

                    {/* Adet */}
                    <td className="px-3 py-2">
                      <input
                        type="number"
                        min="1"
                        {...register(`kalemler.${index}.adet`, { valueAsNumber: true })}
                        className="input input-sm w-full text-center"
                      />
                    </td>

                    {/* KDV */}
                    <td className="px-3 py-2">
                      <input
                        type="number"
                        {...register(`kalemler.${index}.kdvOran`, { valueAsNumber: true })}
                        className="input input-sm w-full text-center"
                      />
                    </td>

                    {/* Maliyet */}
                    <td className="px-3 py-2">
                      <input
                        type="number"
                        step="0.01"
                        {...register(`kalemler.${index}.alisFiyati`, { valueAsNumber: true })}
                        className="input input-sm w-full text-right"
                      />
                    </td>

                    {/* Satış Fiyatı */}
                    <td className="px-3 py-2">
                      <input
                        type="number"
                        step="0.01"
                        {...register(`kalemler.${index}.satisFiyati`, { valueAsNumber: true })}
                        className="input input-sm w-full text-right"
                      />
                    </td>

                    {/* Üretim Tarihi */}
                    <td className="px-3 py-2">
                      <input
                        type="date"
                        {...register(`kalemler.${index}.uretimTarihi`)}
                        className="input input-sm w-full text-xs"
                      />
                    </td>

                    {/* Menşei */}
                    <td className="px-3 py-2">
                      <input
                        type="text"
                        {...register(`kalemler.${index}.mensei`)}
                        className="input input-sm w-full text-xs"
                        placeholder="Ülke"
                      />
                    </td>

                    {/* Barkod */}
                    <td className="px-3 py-2">
                      <input
                        type="text"
                        {...register(`kalemler.${index}.barkod`)}
                        className="input input-sm w-full font-mono text-xs"
                        placeholder="Otomatik"
                      />
                    </td>

                    {/* İşlem */}
                    <td className="px-3 py-2 text-center">
                      {fields.length > 1 && (
                        <button
                          type="button"
                          onClick={() => removeKalem(index)}
                          className="p-1.5 text-danger hover:bg-red-50 rounded"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Yeni Satır Ekle */}
          <div className="px-4 py-3 border-b border-secondary-200">
            <button
              type="button"
              onClick={addKalem}
              className="flex items-center gap-2 text-primary-600 hover:text-primary-700 text-sm font-medium"
            >
              <Plus className="w-4 h-4" />
              Yeni Satır Ekle
            </button>
          </div>

          {/* Alt Bölüm - Toplamlar */}
          <div className="p-4 bg-secondary-50">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-end gap-4">
              {/* Açıklama */}
              <div className="w-full md:w-1/2">
                <label className="label">Açıklama</label>
                <textarea
                  {...register('aciklama')}
                  className="input"
                  rows={2}
                  placeholder="Fatura notu (opsiyonel)"
                />
              </div>

              {/* Toplamlar */}
              <div className="w-full md:w-auto">
                <div className="bg-white rounded-lg border border-secondary-200 p-4 min-w-[280px]">
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-secondary-600">Toplam Maliyet:</span>
                      <span className="font-medium">{formatCurrency(totals.toplamMaliyet)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-secondary-600">Toplam KDV:</span>
                      <span className="font-medium">{formatCurrency(totals.toplamKDV)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-secondary-600">Toplam Satış:</span>
                      <span className="font-medium">{formatCurrency(totals.toplamSatis)}</span>
                    </div>
                    <div className="border-t border-secondary-200 pt-2 mt-2">
                      <div className="flex justify-between">
                        <span className="text-secondary-900 font-medium">Kar:</span>
                        <span className={`font-bold ${totals.kar >= 0 ? 'text-green-600' : 'text-danger'}`}>
                          {formatCurrency(totals.kar)}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Kaydet Butonu */}
          <div className="p-4">
            <button
              type="submit"
              disabled={saving}
              className="w-full btn-primary py-3 text-base font-semibold"
            >
              {saving ? (
                'Kaydediliyor...'
              ) : (
                <>
                  <Save className="w-5 h-5 mr-2" />
                  Faturayı Kaydet
                </>
              )}
            </button>
          </div>
        </Card>
      </form>

      {/* Click-away handler for search dropdown - removed, using onBlur instead */}

      {/* Tedarikçi Ekleme Modal */}
      <Modal
        isOpen={showTedarikciModal}
        onClose={() => setShowTedarikciModal(false)}
        title="Yeni Tedarikçi Ekle"
      >
        <div className="space-y-4">
          <div>
            <label className="flex items-center gap-2 label">
              <input
                type="checkbox"
                checked={tedarikciForm.kurumsal}
                onChange={(e) => setTedarikciForm(prev => ({ ...prev, kurumsal: e.target.checked }))}
                className="w-4 h-4 rounded border-secondary-300 text-primary-500"
              />
              <span>Kurumsal / Firma</span>
            </label>
          </div>

          <div>
            <label className="label">Ad / Firma Adı *</label>
            <input
              type="text"
              className="input"
              placeholder={tedarikciForm.kurumsal ? "Firma adı" : "Ad"}
              value={tedarikciForm.ad}
              onChange={(e) => setTedarikciForm(prev => ({ ...prev, ad: e.target.value }))}
            />
          </div>

          {!tedarikciForm.kurumsal && (
            <div>
              <label className="label">Soyad</label>
              <input
                type="text"
                className="input"
                placeholder="Soyad"
                value={tedarikciForm.soyad}
                onChange={(e) => setTedarikciForm(prev => ({ ...prev, soyad: e.target.value }))}
              />
            </div>
          )}

          <div>
            <label className="label">Telefon *</label>
            <input
              type="tel"
              className="input"
              placeholder="05XX XXX XX XX"
              value={tedarikciForm.telefon}
              onChange={(e) => setTedarikciForm(prev => ({ ...prev, telefon: e.target.value }))}
            />
          </div>

          <div>
            <label className="label">E-posta *</label>
            <input
              type="email"
              className="input"
              placeholder="ornek@email.com"
              value={tedarikciForm.email}
              onChange={(e) => setTedarikciForm(prev => ({ ...prev, email: e.target.value }))}
            />
          </div>

          <div>
            <label className="label">Adres</label>
            <textarea
              className="input"
              rows={2}
              placeholder="Adres bilgisi"
              value={tedarikciForm.adres}
              onChange={(e) => setTedarikciForm(prev => ({ ...prev, adres: e.target.value }))}
            />
          </div>

          <div className="flex gap-3 pt-4">
            <button
              type="button"
              onClick={() => setShowTedarikciModal(false)}
              className="btn-secondary flex-1"
            >
              İptal
            </button>
            <button
              type="button"
              onClick={handleSaveTedarikci}
              disabled={savingTedarikci}
              className="btn-primary flex-1"
            >
              {savingTedarikci ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
