"use client";
import { useState, useEffect } from "react";
import { Card, Modal, Badge, DataTable } from "@/components/ui";
import { sayimApi, depoApi, bayiApi } from "@/lib/api";

interface Sayim {
  id: string;
  sayimNo: string;
  depoId: string;
  depoAd: string;
  durum: number;
  durumAd: string;
  baslangicTarihi: string;
  bitisTarihi?: string;
  toplamTaranan: number;
  bulunanUrun: number;
  eksikUrun: number;
  fazlaUrun: number;
  olusturmaTarihi: string;
}

interface SayimRapor {
  sayimId: string;
  sayimNo: string;
  depoAd: string;
  toplamTaranan: number;
  bulunanUrun: number;
  eksikUrun: number;
  fazlaUrun: number;
  eksikler: { urunAd: string; seriNo: string }[];
  fazlalar: { urunAd: string; seriNo: string }[];
}

interface Depo {
  id: string;
  ad: string;
  bayiId: string;
  bayiAd?: string;
}

export default function SayimlarPage() {
  const [sayimlar, setSayimlar] = useState<Sayim[]>([]);
  const [depolar, setDepolar] = useState<Depo[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [taramaModalOpen, setTaramaModalOpen] = useState(false);
  const [raporModalOpen, setRaporModalOpen] = useState(false);
  const [selectedSayim, setSelectedSayim] = useState<Sayim | null>(null);
  const [sayimRapor, setSayimRapor] = useState<SayimRapor | null>(null);

  const [formData, setFormData] = useState({
    depoId: "",
    aciklama: "",
  });
  const [seriInput, setSeriInput] = useState("");
  const [taramaSonuc, setTaramaSonuc] = useState<{
    basarili: boolean;
    mesaj: string;
  } | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [sayimRes, bayiRes] = await Promise.all([
        sayimApi.getList(1, 100),
        bayiApi.getAll(),
      ]);

      setSayimlar(sayimRes.data.data?.items || sayimRes.data.items || []);

      const bayiler = bayiRes.data.data || bayiRes.data || [];
      const allDepolar: Depo[] = [];
      for (const bayi of bayiler) {
        const depoRes = await depoApi.getByBayiId(bayi.id);
        const depoData = depoRes.data.data || depoRes.data || [];
        depoData.forEach((d: any) =>
          allDepolar.push({ ...d, bayiAd: bayi.ad }),
        );
      }
      setDepolar(allDepolar);
    } catch (error) {
      console.error("Veri yükleme hatası:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleBaslat = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await sayimApi.baslat(formData);
      setModalOpen(false);
      setFormData({ depoId: "", aciklama: "" });
      loadData();

      const yeniSayim = res.data.data || res.data;
      if (yeniSayim) {
        setSelectedSayim(yeniSayim);
        setTaramaModalOpen(true);
      }
    } catch (error) {
      console.error("Sayım başlatma hatası:", error);
    }
  };

  const handleSeriTara = async () => {
    if (!selectedSayim || !seriInput.trim()) return;
    try {
      const res = await sayimApi.seriTara(selectedSayim.id, seriInput.trim());
      setTaramaSonuc({
        basarili: res.data.basarili,
        mesaj:
          res.data.mesaj ||
          (res.data.basarili ? "Ürün tarandı" : "Hata oluştu"),
      });
      setSeriInput("");

      const sayimRes = await sayimApi.getById(selectedSayim.id);
      setSelectedSayim(sayimRes.data.data || sayimRes.data);
    } catch (error: any) {
      setTaramaSonuc({
        basarili: false,
        mesaj: error.response?.data?.mesaj || "Tarama hatası",
      });
    }
  };

  const handleTamamla = async (id: string) => {
    try {
      const res = await sayimApi.tamamla(id);
      setSayimRapor(res.data.data || res.data);
      setTaramaModalOpen(false);
      setRaporModalOpen(true);
      loadData();
    } catch (error) {
      console.error("Sayım tamamlama hatası:", error);
    }
  };

  const handleIptal = async (id: string) => {
    if (!confirm("Sayımı iptal etmek istediğinize emin misiniz?")) return;
    try {
      await sayimApi.iptal(id);
      setTaramaModalOpen(false);
      loadData();
    } catch (error) {
      console.error("Sayım iptal hatası:", error);
    }
  };

  const handleViewRapor = async (sayim: Sayim) => {
    try {
      const res = await sayimApi.getRapor(sayim.id);
      setSayimRapor(res.data.data || res.data);
      setRaporModalOpen(true);
    } catch (error) {
      console.error("Rapor yükleme hatası:", error);
    }
  };

  const handleDevamEt = async (sayim: Sayim) => {
    try {
      const res = await sayimApi.getById(sayim.id);
      setSelectedSayim(res.data.data || res.data);
      setTaramaModalOpen(true);
    } catch (error) {
      console.error("Sayım yükleme hatası:", error);
    }
  };

  const getDurumBadge = (durum: number) => {
    const durumlar: Record<
      number,
      { variant: "warning" | "info" | "success" | "danger"; text: string }
    > = {
      0: { variant: "info", text: "Devam Ediyor" },
      1: { variant: "success", text: "Tamamlandı" },
      2: { variant: "danger", text: "İptal" },
    };
    const d = durumlar[durum] || {
      variant: "warning" as const,
      text: "Bilinmiyor",
    };
    return <Badge variant={d.variant}>{d.text}</Badge>;
  };

  const columns = [
    { key: "sayimNo", label: "Sayım No", sortable: true },
    { key: "depoAd", label: "Depo", sortable: true },
    {
      key: "durum",
      label: "Durum",
      render: (row: Sayim) => getDurumBadge(row.durum),
    },
    { key: "toplamTaranan", label: "Taranan" },
    {
      key: "olusturmaTarihi",
      label: "Başlangıç",
      render: (row: Sayim) =>
        new Date(row.olusturmaTarihi).toLocaleDateString("tr-TR"),
    },
    {
      key: "actions",
      label: "İşlemler",
      render: (row: Sayim) => (
        <div className="flex gap-2">
          {row.durum === 0 && (
            <>
              <button
                onClick={() => handleDevamEt(row)}
                className="text-blue-600 hover:text-blue-800 text-sm"
              >
                Devam Et
              </button>
              <button
                onClick={() => handleIptal(row.id)}
                className="text-red-600 hover:text-red-800 text-sm"
              >
                İptal
              </button>
            </>
          )}
          {row.durum === 1 && (
            <button
              onClick={() => handleViewRapor(row)}
              className="text-blue-600 hover:text-blue-800 text-sm"
            >
              Rapor
            </button>
          )}
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Stok Sayımları</h1>
        <button
          onClick={() => setModalOpen(true)}
          className="bg-[#0a66c2] text-white px-4 py-2 rounded-lg hover:bg-[#004182] transition-colors"
        >
          + Yeni Sayım
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="p-4">
          <div className="text-sm text-gray-500">Toplam Sayım</div>
          <div className="text-2xl font-bold text-gray-900">
            {sayimlar.length}
          </div>
        </Card>
        <Card className="p-4">
          <div className="text-sm text-gray-500">Devam Eden</div>
          <div className="text-2xl font-bold text-blue-600">
            {sayimlar.filter((s) => s.durum === 0).length}
          </div>
        </Card>
        <Card className="p-4">
          <div className="text-sm text-gray-500">Tamamlanan</div>
          <div className="text-2xl font-bold text-green-600">
            {sayimlar.filter((s) => s.durum === 1).length}
          </div>
        </Card>
        <Card className="p-4">
          <div className="text-sm text-gray-500">İptal Edilen</div>
          <div className="text-2xl font-bold text-red-600">
            {sayimlar.filter((s) => s.durum === 2).length}
          </div>
        </Card>
      </div>

      <Card>
        <DataTable
          data={sayimlar}
          columns={columns}
          loading={loading}
          searchable
          searchPlaceholder="Sayım ara..."
        />
      </Card>

      {/* Yeni Sayım Modal */}
      <Modal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        title="Yeni Sayım Başlat"
      >
        <form onSubmit={handleBaslat} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Depo
            </label>
            <select
              value={formData.depoId}
              onChange={(e) =>
                setFormData({ ...formData, depoId: e.target.value })
              }
              className="w-full border rounded-lg px-3 py-2"
              required
            >
              <option value="">Seçiniz</option>
              {depolar.map((depo) => (
                <option key={depo.id} value={depo.id}>
                  {depo.bayiAd} - {depo.ad}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Açıklama
            </label>
            <textarea
              value={formData.aciklama}
              onChange={(e) =>
                setFormData({ ...formData, aciklama: e.target.value })
              }
              className="w-full border rounded-lg px-3 py-2"
              rows={2}
            />
          </div>

          <div className="flex justify-end gap-2 pt-4">
            <button
              type="button"
              onClick={() => setModalOpen(false)}
              className="px-4 py-2 border rounded-lg hover:bg-gray-50"
            >
              İptal
            </button>
            <button
              type="submit"
              className="bg-[#0a66c2] text-white px-4 py-2 rounded-lg hover:bg-[#004182]"
            >
              Başlat
            </button>
          </div>
        </form>
      </Modal>

      {/* Tarama Modal */}
      <Modal
        isOpen={taramaModalOpen}
        onClose={() => setTaramaModalOpen(false)}
        title={`Sayım - ${selectedSayim?.sayimNo}`}
        size="lg"
      >
        {selectedSayim && (
          <div className="space-y-4">
            <div className="grid grid-cols-3 gap-4 text-center">
              <div className="bg-blue-50 rounded-lg p-3">
                <div className="text-2xl font-bold text-blue-600">
                  {selectedSayim.toplamTaranan || 0}
                </div>
                <div className="text-sm text-gray-500">Taranan</div>
              </div>
              <div className="bg-green-50 rounded-lg p-3">
                <div className="text-2xl font-bold text-green-600">
                  {selectedSayim.bulunanUrun || 0}
                </div>
                <div className="text-sm text-gray-500">Bulunan</div>
              </div>
              <div className="bg-red-50 rounded-lg p-3">
                <div className="text-2xl font-bold text-red-600">
                  {selectedSayim.eksikUrun || 0}
                </div>
                <div className="text-sm text-gray-500">Eksik</div>
              </div>
            </div>

            <div className="flex gap-2">
              <input
                type="text"
                value={seriInput}
                onChange={(e) => setSeriInput(e.target.value)}
                onKeyPress={(e) => e.key === "Enter" && handleSeriTara()}
                placeholder="Seri numarası tarayın veya girin..."
                className="flex-1 border rounded-lg px-3 py-3 text-lg"
                autoFocus
              />
              <button
                onClick={handleSeriTara}
                className="bg-[#0a66c2] text-white px-6 py-3 rounded-lg hover:bg-[#004182]"
              >
                Tara
              </button>
            </div>

            {taramaSonuc && (
              <div
                className={`p-3 rounded-lg ${taramaSonuc.basarili ? "bg-green-50 text-green-700" : "bg-red-50 text-red-700"}`}
              >
                {taramaSonuc.mesaj}
              </div>
            )}

            <div className="flex justify-between pt-4 border-t">
              <button
                onClick={() => handleIptal(selectedSayim.id)}
                className="px-4 py-2 text-red-600 hover:bg-red-50 rounded-lg"
              >
                İptal Et
              </button>
              <button
                onClick={() => handleTamamla(selectedSayim.id)}
                className="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700"
              >
                Sayımı Tamamla
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Rapor Modal */}
      <Modal
        isOpen={raporModalOpen}
        onClose={() => setRaporModalOpen(false)}
        title="Sayım Raporu"
        size="lg"
      >
        {sayimRapor && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <span className="text-gray-500">Sayım No:</span>
                <p className="font-medium">{sayimRapor.sayimNo}</p>
              </div>
              <div>
                <span className="text-gray-500">Depo:</span>
                <p className="font-medium">{sayimRapor.depoAd}</p>
              </div>
            </div>

            <div className="grid grid-cols-4 gap-4 text-center">
              <div className="bg-blue-50 rounded-lg p-3">
                <div className="text-xl font-bold text-blue-600">
                  {sayimRapor.toplamTaranan}
                </div>
                <div className="text-xs text-gray-500">Taranan</div>
              </div>
              <div className="bg-green-50 rounded-lg p-3">
                <div className="text-xl font-bold text-green-600">
                  {sayimRapor.bulunanUrun}
                </div>
                <div className="text-xs text-gray-500">Bulunan</div>
              </div>
              <div className="bg-red-50 rounded-lg p-3">
                <div className="text-xl font-bold text-red-600">
                  {sayimRapor.eksikUrun}
                </div>
                <div className="text-xs text-gray-500">Eksik</div>
              </div>
              <div className="bg-yellow-50 rounded-lg p-3">
                <div className="text-xl font-bold text-yellow-600">
                  {sayimRapor.fazlaUrun}
                </div>
                <div className="text-xs text-gray-500">Fazla</div>
              </div>
            </div>

            {sayimRapor.eksikler && sayimRapor.eksikler.length > 0 && (
              <div>
                <h4 className="font-medium text-red-600 mb-2">Eksik Ürünler</h4>
                <div className="border border-red-200 rounded-lg divide-y max-h-40 overflow-y-auto">
                  {sayimRapor.eksikler.map((item, index) => (
                    <div key={index} className="p-2 text-sm">
                      {item.urunAd} - {item.seriNo}
                    </div>
                  ))}
                </div>
              </div>
            )}

            {sayimRapor.fazlalar && sayimRapor.fazlalar.length > 0 && (
              <div>
                <h4 className="font-medium text-yellow-600 mb-2">
                  Fazla Ürünler
                </h4>
                <div className="border border-yellow-200 rounded-lg divide-y max-h-40 overflow-y-auto">
                  {sayimRapor.fazlalar.map((item, index) => (
                    <div key={index} className="p-2 text-sm">
                      {item.urunAd} - {item.seriNo}
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        )}
      </Modal>
    </div>
  );
}
