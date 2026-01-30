"use client";
import { useState, useEffect } from "react";
import { Card, Modal, Badge, DataTable } from "@/components/ui";
import { iadeApi, satisApi, musteriApi } from "@/lib/api";

interface Iade {
  id: string;
  iadeNo: string;
  satisId: string;
  satisNo: string;
  musteriId?: string;
  musteriAd?: string;
  durum: number;
  durumAd: string;
  toplamTutar: number;
  iadeEdilen: number;
  neden?: string;
  olusturmaTarihi: string;
  kalemler?: IadeKalem[];
}

interface IadeKalem {
  id: string;
  urunId: string;
  urunAd: string;
  seriNo: string;
  iadeTutar: number;
}

interface Satis {
  id: string;
  satisNo: string;
  musteriAd?: string;
  genelToplam: number;
}

export default function IadelerPage() {
  const [iadeler, setIadeler] = useState<Iade[]>([]);
  const [satislar, setSatislar] = useState<Satis[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [detayModalOpen, setDetayModalOpen] = useState(false);
  const [selectedIade, setSelectedIade] = useState<Iade | null>(null);

  const [formData, setFormData] = useState({
    satisId: "",
    neden: "",
    kalemler: [] as { seriNo: string; iadeTutar: number }[],
  });
  const [seriInput, setSeriInput] = useState("");
  const [tutarInput, setTutarInput] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [iadeRes, satisRes] = await Promise.all([
        iadeApi.getList(1, 100),
        satisApi.getList(1, 100),
      ]);

      setIadeler(iadeRes.data.data?.items || iadeRes.data.items || []);
      setSatislar(satisRes.data.data?.items || satisRes.data.items || []);
    } catch (error) {
      console.error("Veri yükleme hatası:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (formData.kalemler.length === 0) {
      alert("En az bir ürün ekleyin");
      return;
    }
    try {
      await iadeApi.create(formData);
      setModalOpen(false);
      resetForm();
      loadData();
    } catch (error) {
      console.error("İade oluşturma hatası:", error);
    }
  };

  const handleAddKalem = () => {
    if (!seriInput.trim() || !tutarInput) return;
    setFormData((prev) => ({
      ...prev,
      kalemler: [
        ...prev.kalemler,
        { seriNo: seriInput.trim(), iadeTutar: parseFloat(tutarInput) },
      ],
    }));
    setSeriInput("");
    setTutarInput("");
  };

  const handleRemoveKalem = (index: number) => {
    setFormData((prev) => ({
      ...prev,
      kalemler: prev.kalemler.filter((_, i) => i !== index),
    }));
  };

  const handleOnayla = async (id: string) => {
    try {
      await iadeApi.onayla(id);
      loadData();
    } catch (error) {
      console.error("Onaylama hatası:", error);
    }
  };

  const handleTamamla = async (id: string) => {
    try {
      await iadeApi.tamamla(id);
      loadData();
    } catch (error) {
      console.error("Tamamlama hatası:", error);
    }
  };

  const handleReddet = async (id: string) => {
    const neden = prompt("Ret nedeni:");
    if (!neden) return;
    try {
      await iadeApi.reddet(id, neden);
      loadData();
    } catch (error) {
      console.error("Ret hatası:", error);
    }
  };

  const handleViewDetail = async (iade: Iade) => {
    try {
      const res = await iadeApi.getById(iade.id);
      setSelectedIade(res.data.data || res.data);
      setDetayModalOpen(true);
    } catch (error) {
      console.error("Detay yükleme hatası:", error);
    }
  };

  const resetForm = () => {
    setFormData({
      satisId: "",
      neden: "",
      kalemler: [],
    });
    setSeriInput("");
    setTutarInput("");
  };

  const getDurumBadge = (durum: number) => {
    const durumlar: Record<
      number,
      { variant: "warning" | "info" | "success" | "danger"; text: string }
    > = {
      0: { variant: "warning", text: "Beklemede" },
      1: { variant: "info", text: "Onaylandı" },
      2: { variant: "success", text: "Tamamlandı" },
      3: { variant: "danger", text: "Reddedildi" },
    };
    const d = durumlar[durum] || {
      variant: "warning" as const,
      text: "Bilinmiyor",
    };
    return <Badge variant={d.variant}>{d.text}</Badge>;
  };

  const columns = [
    { key: "iadeNo", label: "İade No", sortable: true },
    { key: "satisNo", label: "Satış No", sortable: true },
    { key: "musteriAd", label: "Müşteri" },
    {
      key: "toplamTutar",
      label: "Tutar",
      render: (row: Iade) =>
        `₺${row.toplamTutar.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}`,
    },
    {
      key: "durum",
      label: "Durum",
      render: (row: Iade) => getDurumBadge(row.durum),
    },
    {
      key: "olusturmaTarihi",
      label: "Tarih",
      render: (row: Iade) =>
        new Date(row.olusturmaTarihi).toLocaleDateString("tr-TR"),
    },
    {
      key: "actions",
      label: "İşlemler",
      render: (row: Iade) => (
        <div className="flex gap-2">
          <button
            onClick={() => handleViewDetail(row)}
            className="text-blue-600 hover:text-blue-800 text-sm"
          >
            Detay
          </button>
          {row.durum === 0 && (
            <>
              <button
                onClick={() => handleOnayla(row.id)}
                className="text-green-600 hover:text-green-800 text-sm"
              >
                Onayla
              </button>
              <button
                onClick={() => handleReddet(row.id)}
                className="text-red-600 hover:text-red-800 text-sm"
              >
                Reddet
              </button>
            </>
          )}
          {row.durum === 1 && (
            <button
              onClick={() => handleTamamla(row.id)}
              className="text-green-600 hover:text-green-800 text-sm"
            >
              Tamamla
            </button>
          )}
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">İadeler</h1>
        <button
          onClick={() => setModalOpen(true)}
          className="bg-[#0a66c2] text-white px-4 py-2 rounded-lg hover:bg-[#004182] transition-colors"
        >
          + Yeni İade
        </button>
      </div>

      <Card>
        <DataTable
          data={iadeler}
          columns={columns}
          loading={loading}
          searchable
          searchPlaceholder="İade ara..."
        />
      </Card>

      <Modal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          resetForm();
        }}
        title="Yeni İade"
        size="lg"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Satış
            </label>
            <select
              value={formData.satisId}
              onChange={(e) =>
                setFormData({ ...formData, satisId: e.target.value })
              }
              className="w-full border rounded-lg px-3 py-2"
              required
            >
              <option value="">Seçiniz</option>
              {satislar.map((satis) => (
                <option key={satis.id} value={satis.id}>
                  {satis.satisNo} - {satis.musteriAd || "Anonim"} - ₺
                  {satis.genelToplam.toLocaleString("tr-TR")}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              İade Nedeni
            </label>
            <textarea
              value={formData.neden}
              onChange={(e) =>
                setFormData({ ...formData, neden: e.target.value })
              }
              className="w-full border rounded-lg px-3 py-2"
              rows={2}
              required
            />
          </div>

          <div className="border-t pt-4">
            <h4 className="font-medium mb-2">İade Edilecek Ürünler</h4>
            <div className="flex gap-2 mb-2">
              <input
                type="text"
                value={seriInput}
                onChange={(e) => setSeriInput(e.target.value)}
                placeholder="Seri No"
                className="flex-1 border rounded-lg px-3 py-2"
              />
              <input
                type="number"
                value={tutarInput}
                onChange={(e) => setTutarInput(e.target.value)}
                placeholder="İade Tutarı"
                className="w-32 border rounded-lg px-3 py-2"
                step="0.01"
              />
              <button
                type="button"
                onClick={handleAddKalem}
                className="bg-gray-200 px-4 py-2 rounded-lg hover:bg-gray-300"
              >
                Ekle
              </button>
            </div>

            {formData.kalemler.length > 0 && (
              <div className="border rounded-lg divide-y max-h-40 overflow-y-auto">
                {formData.kalemler.map((kalem, index) => (
                  <div
                    key={index}
                    className="flex justify-between items-center p-2"
                  >
                    <span>
                      {kalem.seriNo} - ₺
                      {kalem.iadeTutar.toLocaleString("tr-TR")}
                    </span>
                    <button
                      type="button"
                      onClick={() => handleRemoveKalem(index)}
                      className="text-red-600 hover:text-red-800"
                    >
                      ✕
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="flex justify-end gap-2 pt-4">
            <button
              type="button"
              onClick={() => {
                setModalOpen(false);
                resetForm();
              }}
              className="px-4 py-2 border rounded-lg hover:bg-gray-50"
            >
              İptal
            </button>
            <button
              type="submit"
              className="bg-[#0a66c2] text-white px-4 py-2 rounded-lg hover:bg-[#004182]"
            >
              Oluştur
            </button>
          </div>
        </form>
      </Modal>

      <Modal
        isOpen={detayModalOpen}
        onClose={() => setDetayModalOpen(false)}
        title={`İade Detayı - ${selectedIade?.iadeNo}`}
        size="lg"
      >
        {selectedIade && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <span className="text-gray-500">Satış No:</span>
                <p className="font-medium">{selectedIade.satisNo}</p>
              </div>
              <div>
                <span className="text-gray-500">Müşteri:</span>
                <p className="font-medium">
                  {selectedIade.musteriAd || "Anonim"}
                </p>
              </div>
              <div>
                <span className="text-gray-500">Durum:</span>
                <p>{getDurumBadge(selectedIade.durum)}</p>
              </div>
              <div>
                <span className="text-gray-500">Toplam Tutar:</span>
                <p className="font-medium">
                  ₺{selectedIade.toplamTutar.toLocaleString("tr-TR")}
                </p>
              </div>
              <div className="col-span-2">
                <span className="text-gray-500">Neden:</span>
                <p className="font-medium">{selectedIade.neden}</p>
              </div>
            </div>

            {selectedIade.kalemler && selectedIade.kalemler.length > 0 && (
              <div>
                <h4 className="font-medium mb-2">İade Edilen Ürünler</h4>
                <div className="border rounded-lg divide-y">
                  {selectedIade.kalemler.map((kalem, index) => (
                    <div
                      key={index}
                      className="flex justify-between items-center p-3"
                    >
                      <div>
                        <p className="font-medium">{kalem.urunAd}</p>
                        <p className="text-sm text-gray-500">
                          Seri: {kalem.seriNo}
                        </p>
                      </div>
                      <span className="font-medium">
                        ₺{kalem.iadeTutar.toLocaleString("tr-TR")}
                      </span>
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
