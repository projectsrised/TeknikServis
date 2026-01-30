"use client";
import { useState, useEffect } from "react";
import { Card, Modal, Badge, DataTable, ConfirmDialog } from "@/components/ui";
import { transferApi, depoApi, urunApi, bayiApi } from "@/lib/api";
import { useAuthStore } from "@/store/authStore";

interface Transfer {
  id: string;
  transferNo: string;
  kaynakDepoId: string;
  kaynakDepoAd: string;
  hedefDepoId: string;
  hedefDepoAd: string;
  durum: number;
  durumAd: string;
  aciklama?: string;
  olusturmaTarihi: string;
  kalemler?: TransferKalem[];
}

interface TransferKalem {
  id: string;
  urunId: string;
  urunAd: string;
  seriNo: string;
  teslimAlindi: boolean;
}

interface Depo {
  id: string;
  ad: string;
  bayiId: string;
  bayiAd?: string;
}

interface Urun {
  id: string;
  ad: string;
  barkod: string;
}

export default function TransferlerPage() {
  const { user } = useAuthStore();
  const [transferler, setTransferler] = useState<Transfer[]>([]);
  const [depolar, setDepolar] = useState<Depo[]>([]);
  const [urunler, setUrunler] = useState<Urun[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [detayModalOpen, setDetayModalOpen] = useState(false);
  const [selectedTransfer, setSelectedTransfer] = useState<Transfer | null>(
    null,
  );
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [confirmAction, setConfirmAction] = useState<{
    type: string;
    id: string;
  } | null>(null);

  const [formData, setFormData] = useState({
    kaynakDepoId: "",
    hedefDepoId: "",
    aciklama: "",
    kalemler: [] as { urunId: string; seriNo: string }[],
  });
  const [seriInput, setSeriInput] = useState("");
  const [selectedUrun, setSelectedUrun] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [transferRes, bayiRes] = await Promise.all([
        transferApi.getList(1, 100),
        bayiApi.getAll(),
      ]);

      setTransferler(
        transferRes.data.data?.items || transferRes.data.items || [],
      );

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

      const urunRes = await urunApi.getList(1, 100);
      setUrunler(urunRes.data.data?.items || urunRes.data.items || []);
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
      await transferApi.create(formData);
      setModalOpen(false);
      resetForm();
      loadData();
    } catch (error) {
      console.error("Transfer oluşturma hatası:", error);
    }
  };

  const handleAddSeri = () => {
    if (!selectedUrun || !seriInput.trim()) return;
    const urun = urunler.find((u) => u.id === selectedUrun);
    if (!urun) return;

    setFormData((prev) => ({
      ...prev,
      kalemler: [
        ...prev.kalemler,
        { urunId: selectedUrun, seriNo: seriInput.trim() },
      ],
    }));
    setSeriInput("");
  };

  const handleRemoveSeri = (index: number) => {
    setFormData((prev) => ({
      ...prev,
      kalemler: prev.kalemler.filter((_, i) => i !== index),
    }));
  };

  const handleOnayla = async (id: string) => {
    try {
      await transferApi.onayla(id);
      loadData();
    } catch (error) {
      console.error("Onaylama hatası:", error);
    }
  };

  const handleTamamla = async (id: string) => {
    try {
      await transferApi.tamamla(id);
      loadData();
    } catch (error) {
      console.error("Tamamlama hatası:", error);
    }
  };

  const handleIptal = async (id: string) => {
    try {
      await transferApi.iptal(id, "İptal edildi");
      loadData();
    } catch (error) {
      console.error("İptal hatası:", error);
    }
  };

  const handleViewDetail = async (transfer: Transfer) => {
    try {
      const res = await transferApi.getById(transfer.id);
      setSelectedTransfer(res.data.data || res.data);
      setDetayModalOpen(true);
    } catch (error) {
      console.error("Detay yükleme hatası:", error);
    }
  };

  const resetForm = () => {
    setFormData({
      kaynakDepoId: "",
      hedefDepoId: "",
      aciklama: "",
      kalemler: [],
    });
    setSeriInput("");
    setSelectedUrun("");
  };

  const getDurumBadge = (durum: number) => {
    const durumlar: Record<
      number,
      { variant: "warning" | "info" | "success" | "danger"; text: string }
    > = {
      0: { variant: "warning", text: "Beklemede" },
      1: { variant: "info", text: "Onaylandı" },
      2: { variant: "info", text: "Yolda" },
      3: { variant: "success", text: "Tamamlandı" },
      4: { variant: "danger", text: "İptal" },
    };
    const d = durumlar[durum] || {
      variant: "warning" as const,
      text: "Bilinmiyor",
    };
    return <Badge variant={d.variant}>{d.text}</Badge>;
  };

  const columns = [
    { key: "transferNo", label: "Transfer No", sortable: true },
    { key: "kaynakDepoAd", label: "Kaynak Depo", sortable: true },
    { key: "hedefDepoAd", label: "Hedef Depo", sortable: true },
    {
      key: "durum",
      label: "Durum",
      render: (row: Transfer) => getDurumBadge(row.durum),
    },
    {
      key: "olusturmaTarihi",
      label: "Tarih",
      render: (row: Transfer) =>
        new Date(row.olusturmaTarihi).toLocaleDateString("tr-TR"),
    },
    {
      key: "actions",
      label: "İşlemler",
      render: (row: Transfer) => (
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
                onClick={() => handleIptal(row.id)}
                className="text-red-600 hover:text-red-800 text-sm"
              >
                İptal
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
        <h1 className="text-2xl font-bold text-gray-900">Transferler</h1>
        <button
          onClick={() => setModalOpen(true)}
          className="bg-[#0a66c2] text-white px-4 py-2 rounded-lg hover:bg-[#004182] transition-colors"
        >
          + Yeni Transfer
        </button>
      </div>

      <Card>
        <DataTable
          data={transferler}
          columns={columns}
          loading={loading}
          searchable
          searchPlaceholder="Transfer ara..."
        />
      </Card>

      <Modal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          resetForm();
        }}
        title="Yeni Transfer"
        size="lg"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Kaynak Depo
              </label>
              <select
                value={formData.kaynakDepoId}
                onChange={(e) =>
                  setFormData({ ...formData, kaynakDepoId: e.target.value })
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
                Hedef Depo
              </label>
              <select
                value={formData.hedefDepoId}
                onChange={(e) =>
                  setFormData({ ...formData, hedefDepoId: e.target.value })
                }
                className="w-full border rounded-lg px-3 py-2"
                required
              >
                <option value="">Seçiniz</option>
                {depolar
                  .filter((d) => d.id !== formData.kaynakDepoId)
                  .map((depo) => (
                    <option key={depo.id} value={depo.id}>
                      {depo.bayiAd} - {depo.ad}
                    </option>
                  ))}
              </select>
            </div>
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

          <div className="border-t pt-4">
            <h4 className="font-medium mb-2">Ürün Ekle</h4>
            <div className="flex gap-2 mb-2">
              <select
                value={selectedUrun}
                onChange={(e) => setSelectedUrun(e.target.value)}
                className="flex-1 border rounded-lg px-3 py-2"
              >
                <option value="">Ürün Seçin</option>
                {urunler.map((urun) => (
                  <option key={urun.id} value={urun.id}>
                    {urun.ad} ({urun.barkod})
                  </option>
                ))}
              </select>
              <input
                type="text"
                value={seriInput}
                onChange={(e) => setSeriInput(e.target.value)}
                placeholder="Seri No"
                className="flex-1 border rounded-lg px-3 py-2"
              />
              <button
                type="button"
                onClick={handleAddSeri}
                className="bg-gray-200 px-4 py-2 rounded-lg hover:bg-gray-300"
              >
                Ekle
              </button>
            </div>

            {formData.kalemler.length > 0 && (
              <div className="border rounded-lg divide-y max-h-40 overflow-y-auto">
                {formData.kalemler.map((kalem, index) => {
                  const urun = urunler.find((u) => u.id === kalem.urunId);
                  return (
                    <div
                      key={index}
                      className="flex justify-between items-center p-2"
                    >
                      <span>
                        {urun?.ad} - {kalem.seriNo}
                      </span>
                      <button
                        type="button"
                        onClick={() => handleRemoveSeri(index)}
                        className="text-red-600 hover:text-red-800"
                      >
                        ✕
                      </button>
                    </div>
                  );
                })}
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
        title={`Transfer Detayı - ${selectedTransfer?.transferNo}`}
        size="lg"
      >
        {selectedTransfer && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <span className="text-gray-500">Kaynak Depo:</span>
                <p className="font-medium">{selectedTransfer.kaynakDepoAd}</p>
              </div>
              <div>
                <span className="text-gray-500">Hedef Depo:</span>
                <p className="font-medium">{selectedTransfer.hedefDepoAd}</p>
              </div>
              <div>
                <span className="text-gray-500">Durum:</span>
                <p>{getDurumBadge(selectedTransfer.durum)}</p>
              </div>
              <div>
                <span className="text-gray-500">Tarih:</span>
                <p className="font-medium">
                  {new Date(selectedTransfer.olusturmaTarihi).toLocaleString(
                    "tr-TR",
                  )}
                </p>
              </div>
            </div>

            {selectedTransfer.kalemler &&
              selectedTransfer.kalemler.length > 0 && (
                <div>
                  <h4 className="font-medium mb-2">Ürünler</h4>
                  <div className="border rounded-lg divide-y">
                    {selectedTransfer.kalemler.map((kalem, index) => (
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
                        <Badge
                          variant={kalem.teslimAlindi ? "success" : "warning"}
                        >
                          {kalem.teslimAlindi ? "Teslim Alındı" : "Bekliyor"}
                        </Badge>
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
