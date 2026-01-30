'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  LayoutDashboard,
  Building2,
  Users,
  Package,
  Tags,
  ShoppingCart,
  Wrench,
  ArrowLeftRight,
  RotateCcw,
  FileText,
  ClipboardList,
  Boxes,
  Wallet,
  UserCog,
  BarChart3,
  Settings,
  LogOut,
  ChevronDown,
  Store,
  Tag,
  ArrowDownToLine,
  Scan,
} from 'lucide-react';
import { useAuthStore } from '@/store/authStore';
import { useState } from 'react';
import { clsx } from 'clsx';

const menuItems = [
  {
    title: 'Ana Sayfa',
    href: '/dashboard',
    icon: LayoutDashboard,
  },
  {
    title: 'Yönetim',
    icon: Building2,
    children: [
      { title: 'Bayiler', href: '/dashboard/bayiler', icon: Store },
      { title: 'Depolar', href: '/dashboard/depolar', icon: Building2 },
      { title: 'Personel', href: '/dashboard/personel', icon: UserCog },
    ],
  },
  {
    title: 'Müşteriler',
    href: '/dashboard/musteriler',
    icon: Users,
  },
  {
    title: 'Ürün Yönetimi',
    icon: Package,
    children: [
      { title: 'Markalar', href: '/dashboard/markalar', icon: Tag },
      { title: 'Kategoriler', href: '/dashboard/kategoriler', icon: Tags },
      { title: 'Ürünler', href: '/dashboard/urunler', icon: Package },
    ],
  },
  {
    title: 'Stok Yönetimi',
    icon: Boxes,
    children: [
      { title: 'Stok Durumu', href: '/dashboard/stok', icon: Boxes },
      { title: 'Stok Girişi', href: '/dashboard/stok/giris', icon: ArrowDownToLine },
      { title: 'Stok Kalemleri', href: '/dashboard/stok/kalemler', icon: Package },
      { title: 'Karekod Okut', href: '/dashboard/stok/karekod-okut', icon: Scan },
    ],
  },
  {
    title: 'Satış',
    href: '/dashboard/satislar',
    icon: ShoppingCart,
  },
  {
    title: 'Teknik Servis',
    href: '/dashboard/teknik-servis',
    icon: Wrench,
  },
  {
    title: 'İşlemler',
    icon: ArrowLeftRight,
    children: [
      { title: 'Transferler', href: '/dashboard/transferler', icon: ArrowLeftRight },
      { title: 'İadeler', href: '/dashboard/iadeler', icon: RotateCcw },
      { title: 'Faturalar', href: '/dashboard/faturalar', icon: FileText },
      { title: 'Sayımlar', href: '/dashboard/sayimlar', icon: ClipboardList },
    ],
  },
  {
    title: 'Kasa',
    href: '/dashboard/kasa',
    icon: Wallet,
  },
  {
    title: 'Raporlar',
    href: '/dashboard/raporlar',
    icon: BarChart3,
  },
];

export default function Sidebar() {
  const pathname = usePathname();
  const { user, logout } = useAuthStore();
  const [openMenus, setOpenMenus] = useState<string[]>(['Yönetim', 'Ürün Yönetimi', 'İşlemler']);
  const [isExpanded, setIsExpanded] = useState(false);

  const toggleMenu = (title: string) => {
    setOpenMenus((prev) =>
      prev.includes(title) ? prev.filter((t) => t !== title) : [...prev, title]
    );
  };

  const isActive = (href: string) => pathname === href;
  const isParentActive = (children: any[]) =>
    children?.some((child: any) => pathname === child.href);

  return (
    <aside 
      className="group w-16 hover:w-64 bg-white border-r border-secondary-200 h-screen fixed left-0 top-0 flex flex-col transition-all duration-300 ease-in-out z-50 shadow-lg"
      onMouseEnter={() => setIsExpanded(true)}
      onMouseLeave={() => setIsExpanded(false)}
    >
      {/* Logo */}
      <div className="p-4 border-b border-secondary-200">
        <Link href="/dashboard" className="flex items-center gap-2">
          <div className="w-10 h-10 bg-primary-500 rounded-lg flex items-center justify-center flex-shrink-0">
            <Wrench className="w-6 h-6 text-white" />
          </div>
          <div className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden">
            <h1 className="font-bold text-lg text-secondary-900">TeknikServis</h1>
            <p className="text-xs text-secondary-500">Yönetim Paneli</p>
          </div>
        </Link>
      </div>

      {/* User Info */}
      <div className="p-4 border-b border-secondary-200">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center flex-shrink-0">
            <span className="text-primary-600 font-semibold text-sm">
              {user?.ad?.charAt(0)}{user?.soyad?.charAt(0)}
            </span>
          </div>
          <div className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden flex-1 min-w-0">
            <p className="text-sm font-medium text-secondary-900 truncate">
              {user?.adSoyad}
            </p>
            <p className="text-xs text-secondary-500 truncate">{user?.rolAd}</p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto p-3 overflow-x-hidden">
        <ul className="space-y-1">
          {menuItems.map((item) => (
            <li key={item.title}>
              {item.children ? (
                <div>
                  <button
                    onClick={() => toggleMenu(item.title)}
                    className={clsx(
                      'w-full flex items-center gap-3 px-3 py-2.5 text-sm font-medium text-secondary-600 rounded-lg transition-all duration-200 hover:bg-secondary-100 hover:text-secondary-900 justify-center group-hover:justify-between',
                      isParentActive(item.children) && 'text-primary-600 bg-primary-50'
                    )}
                    title={item.title}
                  >
                    <item.icon className="w-5 h-5 flex-shrink-0" />
                    <span className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden">
                      {item.title}
                    </span>
                    <ChevronDown
                      className={clsx(
                        'w-4 h-4 transition-all duration-300 flex-shrink-0 opacity-0 group-hover:opacity-100',
                        openMenus.includes(item.title) && 'rotate-180'
                      )}
                    />
                  </button>
                  {isExpanded && openMenus.includes(item.title) && (
                    <ul className="mt-1 ml-0 group-hover:ml-4 space-y-1 transition-all duration-300">
                      {item.children.map((child) => (
                        <li key={child.href}>
                          <Link
                            href={child.href}
                            className={clsx(
                              'flex items-center gap-3 px-3 py-2.5 text-sm font-medium text-secondary-600 rounded-lg transition-all duration-200 hover:bg-secondary-100 hover:text-secondary-900 justify-center group-hover:justify-start group-hover:pl-6',
                              isActive(child.href) && 'bg-primary-50 text-primary-600 border-l-4 border-primary-500 -ml-1 pl-5'
                            )}
                            title={child.title}
                          >
                            <child.icon className="w-4 h-4 flex-shrink-0" />
                            <span className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden">
                              {child.title}
                            </span>
                          </Link>
                        </li>
                      ))}
                    </ul>
                  )}
                </div>
              ) : (
                <Link
                  href={item.href!}
                  className={clsx(
                    'flex items-center gap-3 px-3 py-2.5 text-sm font-medium text-secondary-600 rounded-lg transition-all duration-200 hover:bg-secondary-100 hover:text-secondary-900 justify-center group-hover:justify-start',
                    isActive(item.href!) && 'bg-primary-50 text-primary-600 border-l-4 border-primary-500 -ml-1 pl-5'
                  )}
                  title={item.title}
                >
                  <item.icon className="w-5 h-5 flex-shrink-0" />
                  <span className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden">
                    {item.title}
                  </span>
                </Link>
              )}
            </li>
          ))}
        </ul>
      </nav>

      {/* Footer */}
      <div className="p-3 border-t border-secondary-200 space-y-1">
        <Link 
          href="/dashboard/ayarlar" 
          className="flex items-center gap-3 px-3 py-2.5 text-sm font-medium text-secondary-600 rounded-lg transition-all duration-200 hover:bg-secondary-100 hover:text-secondary-900 justify-center group-hover:justify-start"
          title="Ayarlar"
        >
          <Settings className="w-5 h-5 flex-shrink-0" />
          <span className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden">
            Ayarlar
          </span>
        </Link>
        <button 
          onClick={logout} 
          className="flex items-center gap-3 px-3 py-2.5 text-sm font-medium text-danger rounded-lg transition-all duration-200 hover:bg-red-50 w-full justify-center group-hover:justify-start"
          title="Çıkış Yap"
        >
          <LogOut className="w-5 h-5 flex-shrink-0" />
          <span className="opacity-0 group-hover:opacity-100 transition-opacity duration-300 whitespace-nowrap overflow-hidden">
            Çıkış Yap
          </span>
        </button>
      </div>
    </aside>
  );
}
