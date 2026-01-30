'use client';

import { useEffect, useState } from 'react';
import { Calendar, TrendingUp, Users, Store, Package, Download } from 'lucide-react';
import { Card, CardHeader, PageLoading, StatCard, Tabs } from '@/components/ui';
import { raporApi } from '@/lib/api';
import { OzetRapor, BayiPerformans, PersonelPerformans } from '@/types';
import { format, subDays, startOfMonth, endOfMonth } from 'date-fns';
import { tr } from 'date-fns/locale';
import toast from 'react-hot-toast';
import {
  LineChart, Line, BarChart, Bar, PieChart, Pie, Cell,
  XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend
} from 'recharts';

const COLORS = ['#0a66c2', '#057642', '#b24020', '#5c3d99', '#0d9488'];

export default function RaporlarPage() {
  const [loading, setLoading] = useState(true);
  const [ozet, setOzet] = useState<OzetRapor | null>(null);
  const [bayiPerformans, setBayiPerformans] = useState<BayiPerformans[]>([]);
  const [personelPerformans, setPersonelPerformans] = useState<PersonelPerformans[]>([]);
  const [activeTab, setActiveTab] = useState('genel');
  const [dateRange, setDateRange] = useState({
    baslangic: format(startOfMonth(new Date()), 'yyyy-MM-dd'),
    bitis: format(endOfMonth(new Date()), 'yyyy-MM-dd'),
  });

  useEffect(() => {
    loadData();
  }, [dateRange]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [ozetRes, bayiRes, personelRes] = await Promise.all([
        raporApi.getOzet(),
        raporApi.getBayiPerformans(dateRange.baslangic, dateRange.bitis),
        raporApi.getPersonelPerformans(dateRange.baslangic, dateRange.bitis),
      ]);

      if (ozetRes.data.basarili) setOzet(ozetRes.data.data);
      if (bayiRes.data.basarili) setBayiPerformans(bayiRes.data.data);
      if (personelRes.data.basarili) setPersonelPerformans(personelRes.data.data);
    } catch (error) {
      toast.error('Rapor verileri yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(value);

  // Demo chart data
  const satisGrafik = [
    { gun: 'Pzt', satis: 12500, servis: 4200 },
    { gun: 'Sal', satis: 8900, servis: 3100 },
    { gun: 'Çar', satis: 15200, servis: 5800 },
    { gun: 'Per', satis: 11800, servis: 4500 },
    { gun: 'Cum', satis: 18900, servis: 6200 },
    { gun: 'Cmt', satis: 22400, servis: 7800 },
    { gun: 'Paz', satis: 9800, servis: 3400 },
  ];

  const kategoriDagilim = [
    { name: 'Telefonlar', value: 45 },
    { name: 'Tabletler', value: 20 },
    { name: 'Aksesuarlar', value: 25 },
    { name: 'Yedek Parça', value: 10 },
  ];

  if (loading) return <PageLoading />;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-secondary-900">Raporlar</h1>
          <p className="text-secondary-500">İşletme performansınızı analiz edin</p>
        </div>
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2 bg-white border border-secondary-200 rounded-lg px-3 py-2">
            <Calendar className="w-4 h-4 text-secondary-400" />
            <input
              type="date"
              value={dateRange.baslangic}
              onChange={(e) => setDateRange({ ...dateRange, baslangic: e.target.value })}
              className="border-0 bg-transparent text-sm focus:outline-none"
            />
            <span className="text-secondary-400">-</span>
            <input
              type="date"
              value={dateRange.bitis}
              onChange={(e) => setDateRange({ ...dateRange, bitis: e.target.value })}
              className="border-0 bg-transparent text-sm focus:outline-none"
            />
          </div>
          <button className="btn-secondary">
            <Download className="w-4 h-4 mr-2" />
            Rapor İndir
          </button>
        </div>
      </div>

      {/* Tabs */}
      <Card padding={false}>
        <div className="p-4 border-b border-secondary-200">
          <Tabs
            tabs={[
              { id: 'genel', label: 'Genel Bakış' },
              { id: 'satis', label: 'Satış Analizi' },
              { id: 'performans', label: 'Performans' },
            ]}
            activeTab={activeTab}
            onChange={setActiveTab}
          />
        </div>
      </Card>

      {activeTab === 'genel' && (
        <>
          {/* Summary Stats */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <StatCard
              title="Aylık Ciro"
              value={formatCurrency(ozet?.aylikCiro || 0)}
              change={{ value: '%18.2 artış', type: 'up' }}
              icon={<TrendingUp className="w-6 h-6" />}
              color="success"
            />
            <StatCard
              title="Günlük Ciro"
              value={formatCurrency(ozet?.gunlukCiro || 0)}
              icon={<TrendingUp className="w-6 h-6" />}
              color="primary"
            />
            <StatCard
              title="Bugünkü Satış"
              value={ozet?.bugunkuSatisSayisi || 0}
              icon={<Package className="w-6 h-6" />}
              color="primary"
            />
            <StatCard
              title="Kasa Bakiyesi"
              value={formatCurrency(ozet?.kasaBakiyesi || 0)}
              icon={<TrendingUp className="w-6 h-6" />}
              color="success"
            />
          </div>

          {/* Charts */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <Card>
              <CardHeader title="Haftalık Satış & Servis" subtitle="Son 7 günlük performans" />
              <div className="h-80">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={satisGrafik}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                    <XAxis dataKey="gun" stroke="#64748b" fontSize={12} />
                    <YAxis stroke="#64748b" fontSize={12} />
                    <Tooltip
                      contentStyle={{
                        backgroundColor: '#fff',
                        border: '1px solid #e2e8f0',
                        borderRadius: '8px',
                      }}
                      formatter={(value: number) => formatCurrency(value)}
                    />
                    <Legend />
                    <Line type="monotone" dataKey="satis" stroke="#0a66c2" strokeWidth={2} name="Satış" />
                    <Line type="monotone" dataKey="servis" stroke="#057642" strokeWidth={2} name="Servis" />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </Card>

            <Card>
              <CardHeader title="Kategori Dağılımı" subtitle="Satış bazlı kategori oranları" />
              <div className="h-80">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={kategoriDagilim}
                      cx="50%"
                      cy="50%"
                      innerRadius={60}
                      outerRadius={100}
                      paddingAngle={5}
                      dataKey="value"
                      label={({ name, value }) => `${name}: %${value}`}
                    >
                      {kategoriDagilim.map((entry, index) => (
                        <Cell key={entry.name} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </Card>
          </div>
        </>
      )}

      {activeTab === 'satis' && (
        <Card>
          <CardHeader title="Satış Trendi" subtitle="Günlük satış tutarları" />
          <div className="h-96">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={satisGrafik}>
                <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
                <XAxis dataKey="gun" stroke="#64748b" fontSize={12} />
                <YAxis stroke="#64748b" fontSize={12} />
                <Tooltip
                  contentStyle={{
                    backgroundColor: '#fff',
                    border: '1px solid #e2e8f0',
                    borderRadius: '8px',
                  }}
                  formatter={(value: number) => formatCurrency(value)}
                />
                <Bar dataKey="satis" fill="#0a66c2" radius={[4, 4, 0, 0]} name="Satış" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </Card>
      )}

      {activeTab === 'performans' && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Bayi Performans */}
          <Card>
            <CardHeader title="Bayi Performansı" subtitle="Bayilere göre satış dağılımı" />
            <div className="space-y-4">
              {bayiPerformans.length > 0 ? (
                bayiPerformans.map((bayi, idx) => (
                  <div key={bayi.bayiId} className="flex items-center gap-4">
                    <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center">
                      <Store className="w-4 h-4 text-primary-600" />
                    </div>
                    <div className="flex-1">
                      <div className="flex justify-between items-center mb-1">
                        <span className="font-medium text-secondary-900">{bayi.bayiAd}</span>
                        <span className="text-sm font-semibold text-primary-600">
                          {formatCurrency(bayi.toplamCiro)}
                        </span>
                      </div>
                      <div className="w-full bg-secondary-100 rounded-full h-2">
                        <div
                          className="bg-primary-500 h-2 rounded-full"
                          style={{
                            width: `${(bayi.toplamCiro / Math.max(...bayiPerformans.map(b => b.toplamCiro))) * 100}%`,
                          }}
                        />
                      </div>
                      <div className="flex gap-4 mt-1 text-xs text-secondary-500">
                        <span>{bayi.satisSayisi} satış</span>
                        <span>{bayi.teknikServisSayisi} servis</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-center text-secondary-500 py-8">Veri bulunamadı</p>
              )}
            </div>
          </Card>

          {/* Personel Performans */}
          <Card>
            <CardHeader title="Personel Performansı" subtitle="En iyi performans gösteren personeller" />
            <div className="space-y-4">
              {personelPerformans.length > 0 ? (
                personelPerformans.slice(0, 5).map((personel, idx) => (
                  <div key={personel.personelId} className="flex items-center gap-4">
                    <div className="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center text-green-600 font-semibold text-sm">
                      {idx + 1}
                    </div>
                    <div className="flex-1">
                      <div className="flex justify-between items-center mb-1">
                        <span className="font-medium text-secondary-900">{personel.personelAdSoyad}</span>
                        <span className="text-sm font-semibold text-success">
                          {formatCurrency(personel.toplamCiro)}
                        </span>
                      </div>
                      <div className="w-full bg-secondary-100 rounded-full h-2">
                        <div
                          className="bg-success h-2 rounded-full"
                          style={{
                            width: `${(personel.toplamCiro / Math.max(...personelPerformans.map(p => p.toplamCiro))) * 100}%`,
                          }}
                        />
                      </div>
                      <div className="flex gap-4 mt-1 text-xs text-secondary-500">
                        <span>{personel.satisSayisi} satış</span>
                        <span>{personel.teknikServisSayisi} servis</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-center text-secondary-500 py-8">Veri bulunamadı</p>
              )}
            </div>
          </Card>
        </div>
      )}
    </div>
  );
}
