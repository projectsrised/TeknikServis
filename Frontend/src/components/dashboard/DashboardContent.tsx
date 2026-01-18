'use client';

import { useAuthStore } from '@/store/authStore';
import {
  ShoppingCart,
  Wrench,
  TrendingUp,
  Package,
  Wallet,
  AlertTriangle,
  ArrowRightLeft,
  BarChart3,
} from 'lucide-react';
import Link from 'next/link';

interface StatCardProps {
  title: string;
  value: string | number;
  icon: any;
  color: string;
  trend?: string;
  link?: string;
}

function StatCard({ title, value, icon: Icon, color, trend, link }: StatCardProps) {
  const content = (
    <div className={`bg-white rounded-xl shadow-sm p-6 border-l-4 ${color} hover:shadow-md transition`}>
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm font-medium text-gray-500">{title}</p>
          <p className="mt-2 text-3xl font-bold text-gray-900">{value}</p>
          {trend && (
            <p className="mt-1 text-sm text-green-600 flex items-center">
              <TrendingUp className="h-4 w-4 mr-1" />
              {trend}
            </p>
          )}
        </div>
        <div className={`p-3 rounded-full bg-opacity-10 ${color.replace('border-', 'bg-')}`}>
          <Icon className={`h-8 w-8 ${color.replace('border-', 'text-')}`} />
        </div>
      </div>
    </div>
  );

  if (link) {
    return <Link href={link}>{content}</Link>;
  }

  return content;
}

export function DashboardContent() {
  const { user, isTenantAdmin, isBranchAdmin } = useAuthStore();

  // Mock data - gerçek uygulamada API'den gelecek
  const stats = {
    todaySales: 15,
    todaySalesAmount: 45250,
    todayServices: 8,
    pendingServices: 12,
    lowStockCount: 5,
    pendingTransfers: 3,
    cashBalance: 125000,
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Hoş Geldin, {user?.firstName}!
          </h1>
          <p className="mt-1 text-sm text-gray-500">
            İşte bugünkü özet bilgiler
          </p>
        </div>
        <div className="mt-4 sm:mt-0 flex gap-3">
          <Link
            href="/sales/new"
            className="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            <ShoppingCart className="h-5 w-5 mr-2" />
            Satış Yap
          </Link>
          <Link
            href="/services/new"
            className="inline-flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition"
          >
            <Wrench className="h-5 w-5 mr-2" />
            Servis Kaydı
          </Link>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard
          title="Bugünkü Satışlar"
          value={stats.todaySales}
          icon={ShoppingCart}
          color="border-blue-500"
          trend="+12% dünden"
          link="/sales"
        />
        <StatCard
          title="Satış Tutarı"
          value={`₺${stats.todaySalesAmount.toLocaleString('tr-TR')}`}
          icon={TrendingUp}
          color="border-green-500"
          link="/sales"
        />
        <StatCard
          title="Bekleyen Servisler"
          value={stats.pendingServices}
          icon={Wrench}
          color="border-orange-500"
          link="/services?status=pending"
        />
        {(isTenantAdmin() || isBranchAdmin()) && (
          <StatCard
            title="Kasa Bakiyesi"
            value={`₺${stats.cashBalance.toLocaleString('tr-TR')}`}
            icon={Wallet}
            color="border-purple-500"
            link="/cash"
          />
        )}
      </div>

      {/* Alerts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {stats.lowStockCount > 0 && (
          <div className="bg-yellow-50 border border-yellow-200 rounded-xl p-4">
            <div className="flex items-center">
              <AlertTriangle className="h-5 w-5 text-yellow-600 mr-3" />
              <div>
                <h3 className="text-sm font-medium text-yellow-800">
                  Düşük Stok Uyarısı
                </h3>
                <p className="text-sm text-yellow-700">
                  {stats.lowStockCount} üründe stok minimum seviyenin altında
                </p>
              </div>
              <Link
                href="/products?lowStock=true"
                className="ml-auto text-sm font-medium text-yellow-800 hover:text-yellow-900"
              >
                Görüntüle →
              </Link>
            </div>
          </div>
        )}

        {stats.pendingTransfers > 0 && (
          <div className="bg-blue-50 border border-blue-200 rounded-xl p-4">
            <div className="flex items-center">
              <ArrowRightLeft className="h-5 w-5 text-blue-600 mr-3" />
              <div>
                <h3 className="text-sm font-medium text-blue-800">
                  Bekleyen Transferler
                </h3>
                <p className="text-sm text-blue-700">
                  {stats.pendingTransfers} transfer onay bekliyor
                </p>
              </div>
              <Link
                href="/transfers?status=pending"
                className="ml-auto text-sm font-medium text-blue-800 hover:text-blue-900"
              >
                Görüntüle →
              </Link>
            </div>
          </div>
        )}
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <Link
          href="/sales/scan"
          className="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition text-center"
        >
          <div className="mx-auto w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center mb-3">
            <ShoppingCart className="h-6 w-6 text-blue-600" />
          </div>
          <p className="font-medium text-gray-900">Barkod ile Satış</p>
        </Link>

        <Link
          href="/transfers/new"
          className="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition text-center"
        >
          <div className="mx-auto w-12 h-12 bg-green-100 rounded-full flex items-center justify-center mb-3">
            <ArrowRightLeft className="h-6 w-6 text-green-600" />
          </div>
          <p className="font-medium text-gray-900">Transfer Oluştur</p>
        </Link>

        <Link
          href="/returns/new"
          className="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition text-center"
        >
          <div className="mx-auto w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center mb-3">
            <Package className="h-6 w-6 text-orange-600" />
          </div>
          <p className="font-medium text-gray-900">İade İşlemi</p>
        </Link>

        <Link
          href="/inventory/new"
          className="bg-white p-6 rounded-xl shadow-sm hover:shadow-md transition text-center"
        >
          <div className="mx-auto w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center mb-3">
            <BarChart3 className="h-6 w-6 text-purple-600" />
          </div>
          <p className="font-medium text-gray-900">Sayım Başlat</p>
        </Link>
      </div>

      {/* Charts section placeholder */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-xl shadow-sm p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Haftalık Satış Grafiği
          </h3>
          <div className="h-64 flex items-center justify-center text-gray-400">
            Grafik burada görünecek
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">
            Personel Satış Performansı
          </h3>
          <div className="h-64 flex items-center justify-center text-gray-400">
            Grafik burada görünecek
          </div>
        </div>
      </div>
    </div>
  );
}
