"use client";
import { useState, useEffect } from "react";
import { Card, Modal, Badge, DataTable } from "@/components/ui";
import { faturaApi, musteriApi } from "@/lib/api";

interface Fatura {
  id: string;
  faturaNo: string;
  tip: number;
  tipAd: string;
  musteriId?: string;
  musteriAd?: string;
  satisId?: string;
  satisNo?: string;
  araToplam: number;
  kdvToplam: number;
  indirimTutar: number;
  genelToplam: number;
  onaylandi: boolean;
  olusturmaTarihi: string;
  kalemler?: FaturaKalem[];
}

interface FaturaKalem {
  id: string;
  urunAd: string;
  miktar: number;
  birimFiyat: number;
  kdvOran: number;
  kdvTutar: number;
  toplam: number;
}

interface Musteri {
  id: string;
  adSoyad: string;
  telefon: string;
}

export default function FaturalarPage() {
  const [faturalar, setFaturalar] = useState<Fatura[]>([]);
  const [musteriler, setMusteriler] = useState<Musteri[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [detayModalOpen, setDetayModalOpen] = useState(false);
  const [selectedFatura, setSelectedFatura] = useState<Fatura | null>(null);
  const [filterTip, setFilterTip] = useState<string>("");

  useEffect(() => {
    loadData();
  }, [filterTip]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [faturaRes, musteriRes] = await Promise.all([
        faturaApi.getList(1, 100, filterTip || undefined),
        musteriApi.getList(1, 100),
      ]);

      setFaturalar(faturaRes.data.data?.items || faturaRes.data.items || []);
      setMusteriler(musteriRes.data.data?.items || musteriRes.data.items || []);
    } catch (error) {
      console.error("Veri yükleme hatası:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleOnayla = async (id: string) => {
    try {
      await faturaApi.onayla(id);
      loadData();
    } catch (error) {
      console.error("Onaylama hatası:", error);
    }
  };

  const handleIptal = async (id: string) => {
    const neden = prompt("İptal nedeni:");
    if (!neden) return;
    try {
      await faturaApi.iptal(id, neden);
      loadData();
    } catch (error) {
      console.error("İptal hatası:", error);
    }
  };

  const handleViewDetail = async (fatura: Fatura) => {
    try {
      const res = await faturaApi.getById(fatura.id);
      setSelectedFatura(res.data.data || res.data);
      setDetayModalOpen(true);
    } catch (error) {
      console.error("Detay yükleme hatası:", error);
    }
  };

  const getTipBadge = (tip: number) => {
    const tipler: Record<
      number,
      { variant: "info" | "success" | "warning" | "danger"; text: string }
    > = {
      0: { variant: "info", text: "Satış" },
      1: { variant: "warning", text: "Alış" },
      2: { variant: "danger", text: "İade" },
    };
    const t = tipler[tip] || { variant: "info" as const, text: "Bilinmiyor" };
    return <Badge variant={t.variant}>{t.text}</Badge>;
  };

  const columns = [
    { key: "faturaNo", label: "Fatura No", sortable: true },
    {
      key: "tip",
      label: "Tip",
      render: (row: Fatura) => getTipBadge(row.tip),
    },
    {
      key: "musteriAd",
      label: "Müşteri",
      render: (row: Fatura) => row.musteriAd || "-",
    },
    {
      key: "genelToplam",
      label: "Toplam",
      render: (row: Fatura) =>
        `₺${row.genelToplam.toLocaleString("tr-TR", { minimumFractionDigits: 2 })}`,
    },
    {
      key: "onaylandi",
      label: "Durum",
      render: (row: Fatura) => (
        <Badge variant={row.onaylandi ? "success" : "warning"}>
          {row.onaylandi ? "Onaylandı" : "Beklemede"}
        </Badge>
      ),
    },
    {
      key: "olusturmaTarihi",
      label: "Tarih",
      render: (row: Fatura) =>
        new Date(row.olusturmaTarihi).toLocaleDateString("tr-TR"),
    },
    {
      key: "actions",
      label: "İşlemler",
      render: (row: Fatura) => (
        <div className="flex gap-2">
          <button
            onClick={() => handleViewDetail(row)}
            className="text-blue-600 hover:text-blue-800 text-sm"
          >
            Detay
          </button>
          {!row.onaylandi && (
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
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Faturalar</h1>
        <div className="flex gap-2">
          <select
            value={filterTip}
            onChange={(e) => setFilterTip(e.target.value)}
            className="border rounded-lg px-3 py-2"
          >
            <option value="">Tüm Tipler</option>
            <option value="0">Satış Faturaları</option>
            <option value="1">Alış Faturaları</option>
            <option value="2">İade Faturaları</option>
          </select>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="p-4">
          <div className="text-sm text-gray-500">Toplam Fatura</div>
          <div className="text-2xl font-bold text-gray-900">
            {faturalar.length}
          </div>
        </Card>
        <Card className="p-4">
          <div className="text-sm text-gray-500">Onay Bekleyen</div>
          <div className="text-2xl font-bold text-yellow-600">
            {faturalar.filter((f) => !f.onaylandi).length}
          </div>
        </Card>
        <Card className="p-4">
          <div className="text-sm text-gray-500">Satış Faturaları</div>
          <div className="text-2xl font-bold text-blue-600">
            {faturalar.filter((f) => f.tip === 0).length}
          </div>
        </Card>
        <Card className="p-4">
          <div className="text-sm text-gray-500">Toplam Tutar</div>
          <div className="text-2xl font-bold text-green-600">
            ₺
            {faturalar
              .reduce((sum, f) => sum + f.genelToplam, 0)
              .toLocaleString("tr-TR", { minimumFractionDigits: 2 })}
          </div>
        </Card>
      </div>

      <Card>
        <DataTable
          data={faturalar}
          columns={columns}
          loading={loading}
          searchable
          searchPlaceholder="Fatura ara..."
        />
      </Card>

      <Modal
        isOpen={detayModalOpen}
        onClose={() => setDetayModalOpen(false)}
        title={`Fatura Detayı - ${selectedFatura?.faturaNo}`}
        size="lg"
      >
        {selectedFatura && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <span className="text-gray-500">Fatura Tipi:</span>
                <p>{getTipBadge(selectedFatura.tip)}</p>
              </div>
              <div>
                <span className="text-gray-500">Müşteri:</span>
                <p className="font-medium">{selectedFatura.musteriAd || "-"}</p>
              </div>
              <div>
                <span className="text-gray-500">Durum:</span>
                <p>
                  <Badge
                    variant={selectedFatura.onaylandi ? "success" : "warning"}
                  >
                    {selectedFatura.onaylandi ? "Onaylandı" : "Beklemede"}
                  </Badge>
                </p>
              </div>
              <div>
                <span className="text-gray-500">Tarih:</span>
                <p className="font-medium">
                  {new Date(selectedFatura.olusturmaTarihi).toLocaleString(
                    "tr-TR",
                  )}
                </p>
              </div>
            </div>

            {selectedFatura.kalemler && selectedFatura.kalemler.length > 0 && (
              <div>
                <h4 className="font-medium mb-2">Kalemler</h4>
                <div className="border rounded-lg overflow-hidden">
                  <table className="w-full text-sm">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="text-left p-2">Ürün</th>
                        <th className="text-right p-2">Miktar</th>
                        <th className="text-right p-2">Birim Fiyat</th>
                        <th className="text-right p-2">KDV</th>
                        <th className="text-right p-2">Toplam</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y">
                      {selectedFatura.kalemler.map((kalem, index) => (
                        <tr key={index}>
                          <td className="p-2">{kalem.urunAd}</td>
                          <td className="p-2 text-right">{kalem.miktar}</td>
                          <td className="p-2 text-right">
                            ₺{kalem.birimFiyat.toLocaleString("tr-TR")}
                          </td>
                          <td className="p-2 text-right">%{kalem.kdvOran}</td>
                          <td className="p-2 text-right font-medium">
                            ₺{kalem.toplam.toLocaleString("tr-TR")}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            <div className="border-t pt-4">
              <div className="flex justify-end">
                <div className="w-64 space-y-1">
                  <div className="flex justify-between">
                    <span className="text-gray-500">Ara Toplam:</span>
                    <span>
                      ₺{selectedFatura.araToplam.toLocaleString("tr-TR")}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-500">KDV:</span>
                    <span>
                      ₺{selectedFatura.kdvToplam.toLocaleString("tr-TR")}
                    </span>
                  </div>
                  {selectedFatura.indirimTutar > 0 && (
                    <div className="flex justify-between text-red-600">
                      <span>İndirim:</span>
                      <span>
                        -₺{selectedFatura.indirimTutar.toLocaleString("tr-TR")}
                      </span>
                    </div>
                  )}
                  <div className="flex justify-between font-bold text-lg border-t pt-1">
                    <span>Genel Toplam:</span>
                    <span>
                      ₺{selectedFatura.genelToplam.toLocaleString("tr-TR")}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}
