'use client';

import { useEffect, useState } from 'react';
import {
  ShoppingCart,
  Wrench,
  AlertTriangle,
  Wallet,
  TrendingUp,
  Package,
  Users,
  ArrowUpRight,
  ArrowDownRight,
} from 'lucide-react';
import { Card, CardHeader, StatCard, PageLoading, Badge } from '@/components/ui';
import { raporApi, teknikServisApi, satisApi } from '@/lib/api';
import { OzetRapor, TeknikServis, Satis } from '@/types';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  BarChart,
  Bar,
} from 'recharts';
import Link from 'next/link';

export default function DashboardPage() {
  const [loading, setLoading] = useState(true);
  const [ozet, setOzet] = useState<OzetRapor | null>(null);
  const [sonSatislar, setSonSatislar] = useState<Satis[]>([]);
  const [bekleyenServisler, setBekleyenServisler] = useState<TeknikServis[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [ozetRes, satisRes, servisRes] = await Promise.all([
        raporApi.getOzet(),
        satisApi.getList(1, 5),
        teknikServisApi.getList(1, 5, 1), // Beklemede olanlar
      ]);

      if (ozetRes.data.basarili) setOzet(ozetRes.data.data);
      if (satisRes.data.basarili) setSonSatislar(satisRes.data.data.items);
      if (servisRes.data.basarili) setBekleyenServisler(servisRes.data.data.items);
    } catch (error) {
      console.error('Dashboard veri yüklenirken hata:', error);
    } finally {
      setLoading(false);
    }
  };

  // Demo chart data
  const chartData = [
    { gun: 'Pzt', satis: 4200, servis: 2400 },
    { gun: 'Sal', satis: 3800, servis: 1398 },
    { gun: 'Çar', satis: 5200, servis: 9800 },
    { gun: 'Per', satis: 4780, servis: 3908 },
    { gun: 'Cum', satis: 6890, servis: 4800 },
    { gun: 'Cmt', satis: 8390, servis: 3800 },
    { gun: 'Paz', satis: 3490, servis: 2300 },
  ];

  if (loading) return <PageLoading />;

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div>
        <h1 className="text-2xl font-bold text-secondary-900">Dashboard</h1>
        <p className="text-secondary-500">Hoş geldiniz! İşte bugünkü özet.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          title="Günlük Ciro"
          value={formatCurrency(ozet?.gunlukCiro || 0)}
          change={{ value: '%12.5 artış', type: 'up' }}
          icon={<TrendingUp className="w-6 h-6" />}
          color="success"
        />
        <StatCard
          title="Bugünkü Satış"
          value={ozet?.bugunkuSatisSayisi || 0}
          change={{ value: '5 yeni satış', type: 'up' }}
          icon={<ShoppingCart className="w-6 h-6" />}
          color="primary"
        />
        <StatCard
          title="Bekleyen Servis"
          value={ozet?.bekleyenTeknikServis || 0}
          icon={<Wrench className="w-6 h-6" />}
          color="warning"
        />
        <StatCard
          title="Kritik Stok"
          value={ozet?.kritikStokUrun || 0}
          icon={<AlertTriangle className="w-6 h-6" />}
          color="danger"
        />
      </div>

      {/* Charts Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Sales Chart */}
        <Card>
          <CardHeader
            title="Haftalık Satış Grafiği"
            subtitle="Son 7 günlük satış performansı"
          />
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis dataKey="gun" stroke="#64748b" fontSize={12} />
                <YAxis stroke="#64748b" fontSize={12} />
                <Tooltip
                  contentStyle={{
                    backgroundColor: '#fff',
                    border: '1px solid #e2e8f0',
                    borderRadius: '8px',
                  }}
                />
                <Line
                  type="monotone"
                  dataKey="satis"
                  stroke="#0a66c2"
                  strokeWidth={2}
                  dot={{ fill: '#0a66c2' }}
                  name="Satış (₺)"
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </Card>

        {/* Service Chart */}
        <Card>
          <CardHeader
            title="Servis İstatistikleri"
            subtitle="Haftalık servis sayıları"
          />
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis dataKey="gun" stroke="#64748b" fontSize={12} />
                <YAxis stroke="#64748b" fontSize={12} />
                <Tooltip
                  contentStyle={{
                    backgroundColor: '#fff',
                    border: '1px solid #e2e8f0',
                    borderRadius: '8px',
                  }}
                />
                <Bar dataKey="servis" fill="#057642" radius={[4, 4, 0, 0]} name="Servis" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>
      </div>

      {/* Tables Row */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Recent Sales */}
        <Card>
          <CardHeader
            title="Son Satışlar"
            action={
              <Link href="/dashboard/satislar" className="text-sm text-primary-500 hover:text-primary-600 font-medium">
                Tümünü Gör →
              </Link>
            }
          />
          <div className="overflow-x-auto">
            <table className="table">
              <thead>
                <tr>
                  <th>Satış No</th>
                  <th>Müşteri</th>
                  <th>Tutar</th>
                  <th>Durum</th>
                </tr>
              </thead>
              <tbody>
                {sonSatislar.length > 0 ? (
                  sonSatislar.map((satis) => (
                    <tr key={satis.id}>
                      <td className="font-medium text-primary-600">{satis.satisNo}</td>
                      <td>{satis.musteriAd || 'Perakende'}</td>
                      <td>{formatCurrency(satis.genelToplam)}</td>
                      <td>
                        {satis.iptalEdildi ? (
                          <Badge variant="danger">İptal</Badge>
                        ) : (
                          <Badge variant="success">Tamamlandı</Badge>
                        )}
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={4} className="text-center text-secondary-500">
                      Henüz satış bulunmuyor
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </Card>

        {/* Pending Services */}
        <Card>
          <CardHeader
            title="Bekleyen Servisler"
            action={
              <Link href="/dashboard/teknik-servis" className="text-sm text-primary-500 hover:text-primary-600 font-medium">
                Tümünü Gör →
              </Link>
            }
          />
          <div className="overflow-x-auto">
            <table className="table">
              <thead>
                <tr>
                  <th>Servis No</th>
                  <th>Cihaz</th>
                  <th>Arıza</th>
                  <th>Durum</th>
                </tr>
              </thead>
              <tbody>
                {bekleyenServisler.length > 0 ? (
                  bekleyenServisler.map((servis) => (
                    <tr key={servis.id}>
                      <td className="font-medium text-primary-600">{servis.servisNo}</td>
                      <td>
                        {servis.cihazMarka} {servis.cihazModel}
                      </td>
                      <td className="max-w-[150px] truncate">{servis.ariza}</td>
                      <td>
                        <Badge variant="warning">{servis.durumAd}</Badge>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={4} className="text-center text-secondary-500">
                      Bekleyen servis bulunmuyor
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </Card>
      </div>

      {/* Quick Actions */}
      <Card>
        <CardHeader title="Hızlı İşlemler" />
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <Link
            href="/dashboard/satislar?yeni=1"
            className="flex flex-col items-center gap-2 p-4 rounded-lg bg-primary-50 hover:bg-primary-100 transition-colors"
          >
            <ShoppingCart className="w-8 h-8 text-primary-500" />
            <span className="text-sm font-medium text-primary-700">Yeni Satış</span>
          </Link>
          <Link
            href="/dashboard/teknik-servis?yeni=1"
            className="flex flex-col items-center gap-2 p-4 rounded-lg bg-green-50 hover:bg-green-100 transition-colors"
          >
            <Wrench className="w-8 h-8 text-green-600" />
            <span className="text-sm font-medium text-green-700">Yeni Servis</span>
          </Link>
          <Link
            href="/dashboard/musteriler?yeni=1"
            className="flex flex-col items-center gap-2 p-4 rounded-lg bg-purple-50 hover:bg-purple-100 transition-colors"
          >
            <Users className="w-8 h-8 text-purple-600" />
            <span className="text-sm font-medium text-purple-700">Yeni Müşteri</span>
          </Link>
          <Link
            href="/dashboard/stok?giris=1"
            className="flex flex-col items-center gap-2 p-4 rounded-lg bg-orange-50 hover:bg-orange-100 transition-colors"
          >
            <Package className="w-8 h-8 text-orange-600" />
            <span className="text-sm font-medium text-orange-700">Stok Girişi</span>
          </Link>
        </div>
      </Card>
    </div>
  );
}
