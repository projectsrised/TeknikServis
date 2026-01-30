"use client";
import { useState, useEffect } from "react";
import { Card, Tabs } from "@/components/ui";
import { useAuthStore } from "@/store/authStore";
import { authApi } from "@/lib/api";

export default function AyarlarPage() {
  const { user } = useAuthStore();
  const [activeTab, setActiveTab] = useState("profil");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<{
    type: "success" | "error";
    text: string;
  } | null>(null);

  const [profilData, setProfilData] = useState({
    ad: user?.ad || "",
    soyad: user?.soyad || "",
    email: user?.email || "",
    telefon: user?.telefon || "",
  });

  const [sifreData, setSifreData] = useState({
    mevcutSifre: "",
    yeniSifre: "",
    yeniSifreTekrar: "",
  });

  useEffect(() => {
    if (user) {
      setProfilData({
        ad: user.ad || "",
        soyad: user.soyad || "",
        email: user.email || "",
        telefon: user.telefon || "",
      });
    }
  }, [user]);

  const handleSifreDegistir = async (e: React.FormEvent) => {
    e.preventDefault();

    if (sifreData.yeniSifre !== sifreData.yeniSifreTekrar) {
      setMessage({ type: "error", text: "Yeni şifreler eşleşmiyor" });
      return;
    }

    if (sifreData.yeniSifre.length < 6) {
      setMessage({ type: "error", text: "Şifre en az 6 karakter olmalıdır" });
      return;
    }

    try {
      setLoading(true);
      await authApi.changePassword({
        mevcutSifre: sifreData.mevcutSifre,
        yeniSifre: sifreData.yeniSifre,
      });
      setMessage({ type: "success", text: "Şifre başarıyla değiştirildi" });
      setSifreData({ mevcutSifre: "", yeniSifre: "", yeniSifreTekrar: "" });
    } catch (error: any) {
      setMessage({
        type: "error",
        text: error.response?.data?.mesaj || "Şifre değiştirme hatası",
      });
    } finally {
      setLoading(false);
    }
  };

  const tabs = [
    { id: "profil", label: "Profil" },
    { id: "sifre", label: "Şifre Değiştir" },
    { id: "bildirimler", label: "Bildirimler" },
    { id: "sistem", label: "Sistem" },
  ];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Ayarlar</h1>

      {message && (
        <div
          className={`p-4 rounded-lg ${message.type === "success" ? "bg-green-50 text-green-700" : "bg-red-50 text-red-700"}`}
        >
          {message.text}
        </div>
      )}

      <Tabs tabs={tabs} activeTab={activeTab} onChange={setActiveTab} />

      {activeTab === "profil" && (
        <Card className="p-6">
          <h2 className="text-lg font-semibold mb-4">Profil Bilgileri</h2>
          <div className="space-y-4 max-w-md">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Ad
              </label>
              <input
                type="text"
                value={profilData.ad}
                className="w-full border rounded-lg px-3 py-2"
                disabled
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Soyad
              </label>
              <input
                type="text"
                value={profilData.soyad}
                className="w-full border rounded-lg px-3 py-2"
                disabled
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                E-posta
              </label>
              <input
                type="email"
                value={profilData.email}
                className="w-full border rounded-lg px-3 py-2 bg-gray-50"
                disabled
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Telefon
              </label>
              <input
                type="text"
                value={profilData.telefon}
                className="w-full border rounded-lg px-3 py-2"
                disabled
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Rol
              </label>
              <input
                type="text"
                value={user?.rolAd || user?.rol || "-"}
                className="w-full border rounded-lg px-3 py-2 bg-gray-50"
                disabled
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Bayi
              </label>
              <input
                type="text"
                value={user?.bayiAd || "-"}
                className="w-full border rounded-lg px-3 py-2 bg-gray-50"
                disabled
              />
            </div>
          </div>
        </Card>
      )}

      {activeTab === "sifre" && (
        <Card className="p-6">
          <h2 className="text-lg font-semibold mb-4">Şifre Değiştir</h2>
          <form onSubmit={handleSifreDegistir} className="space-y-4 max-w-md">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Mevcut Şifre
              </label>
              <input
                type="password"
                value={sifreData.mevcutSifre}
                onChange={(e) =>
                  setSifreData({ ...sifreData, mevcutSifre: e.target.value })
                }
                className="w-full border rounded-lg px-3 py-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Yeni Şifre
              </label>
              <input
                type="password"
                value={sifreData.yeniSifre}
                onChange={(e) =>
                  setSifreData({ ...sifreData, yeniSifre: e.target.value })
                }
                className="w-full border rounded-lg px-3 py-2"
                required
                minLength={6}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Yeni Şifre (Tekrar)
              </label>
              <input
                type="password"
                value={sifreData.yeniSifreTekrar}
                onChange={(e) =>
                  setSifreData({
                    ...sifreData,
                    yeniSifreTekrar: e.target.value,
                  })
                }
                className="w-full border rounded-lg px-3 py-2"
                required
                minLength={6}
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="bg-[#0a66c2] text-white px-4 py-2 rounded-lg hover:bg-[#004182] disabled:opacity-50"
            >
              {loading ? "Kaydediliyor..." : "Şifreyi Değiştir"}
            </button>
          </form>
        </Card>
      )}

      {activeTab === "bildirimler" && (
        <Card className="p-6">
          <h2 className="text-lg font-semibold mb-4">Bildirim Ayarları</h2>
          <div className="space-y-4 max-w-md">
            <div className="flex items-center justify-between">
              <div>
                <p className="font-medium">E-posta Bildirimleri</p>
                <p className="text-sm text-gray-500">
                  Önemli güncellemeler için e-posta al
                </p>
              </div>
              <label className="relative inline-flex items-center cursor-pointer">
                <input
                  type="checkbox"
                  className="sr-only peer"
                  defaultChecked
                />
                <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-[#0a66c2]"></div>
              </label>
            </div>
            <div className="flex items-center justify-between">
              <div>
                <p className="font-medium">Kritik Stok Uyarıları</p>
                <p className="text-sm text-gray-500">
                  Stok kritik seviyeye düştüğünde bildir
                </p>
              </div>
              <label className="relative inline-flex items-center cursor-pointer">
                <input
                  type="checkbox"
                  className="sr-only peer"
                  defaultChecked
                />
                <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-[#0a66c2]"></div>
              </label>
            </div>
            <div className="flex items-center justify-between">
              <div>
                <p className="font-medium">Teknik Servis Güncellemeleri</p>
                <p className="text-sm text-gray-500">
                  Servis durumu değiştiğinde bildir
                </p>
              </div>
              <label className="relative inline-flex items-center cursor-pointer">
                <input
                  type="checkbox"
                  className="sr-only peer"
                  defaultChecked
                />
                <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-[#0a66c2]"></div>
              </label>
            </div>
          </div>
        </Card>
      )}

      {activeTab === "sistem" && (
        <Card className="p-6">
          <h2 className="text-lg font-semibold mb-4">Sistem Bilgileri</h2>
          <div className="space-y-4 max-w-md">
            <div className="flex justify-between py-2 border-b">
              <span className="text-gray-500">Uygulama Versiyonu</span>
              <span className="font-medium">1.0.0</span>
            </div>
            <div className="flex justify-between py-2 border-b">
              <span className="text-gray-500">Tenant</span>
              <span className="font-medium">
                {user?.tenantAd || "Demo Tenant"}
              </span>
            </div>
            <div className="flex justify-between py-2 border-b">
              <span className="text-gray-500">API URL</span>
              <span className="font-medium text-sm">http://localhost:5000</span>
            </div>
          </div>
        </Card>
      )}
    </div>
  );
}
