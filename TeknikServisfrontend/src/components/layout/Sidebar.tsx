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
      { title: 'Stok', href: '/dashboard/stok', icon: Boxes },
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

  const toggleMenu = (title: string) => {
    setOpenMenus((prev) =>
      prev.includes(title) ? prev.filter((t) => t !== title) : [...prev, title]
    );
  };

  const isActive = (href: string) => pathname === href;
  const isParentActive = (children: any[]) =>
    children?.some((child: any) => pathname === child.href);

  return (
    <aside className="w-64 bg-white border-r border-secondary-200 h-screen fixed left-0 top-0 flex flex-col">
      {/* Logo */}
      <div className="p-4 border-b border-secondary-200">
        <Link href="/dashboard" className="flex items-center gap-2">
          <div className="w-10 h-10 bg-primary-500 rounded-lg flex items-center justify-center">
            <Wrench className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="font-bold text-lg text-secondary-900">TeknikServis</h1>
            <p className="text-xs text-secondary-500">Yönetim Paneli</p>
          </div>
        </Link>
      </div>

      {/* User Info */}
      <div className="p-4 border-b border-secondary-200">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center">
            <span className="text-primary-600 font-semibold">
              {user?.ad?.charAt(0)}{user?.soyad?.charAt(0)}
            </span>
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium text-secondary-900 truncate">
              {user?.adSoyad}
            </p>
            <p className="text-xs text-secondary-500 truncate">{user?.rolAd}</p>
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto p-3">
        <ul className="space-y-1">
          {menuItems.map((item) => (
            <li key={item.title}>
              {item.children ? (
                <div>
                  <button
                    onClick={() => toggleMenu(item.title)}
                    className={clsx(
                      'w-full sidebar-item justify-between',
                      isParentActive(item.children) && 'text-primary-600 bg-primary-50'
                    )}
                  >
                    <span className="flex items-center gap-3">
                      <item.icon className="w-5 h-5" />
                      {item.title}
                    </span>
                    <ChevronDown
                      className={clsx(
                        'w-4 h-4 transition-transform',
                        openMenus.includes(item.title) && 'rotate-180'
                      )}
                    />
                  </button>
                  {openMenus.includes(item.title) && (
                    <ul className="mt-1 ml-4 space-y-1">
                      {item.children.map((child) => (
                        <li key={child.href}>
                          <Link
                            href={child.href}
                            className={clsx(
                              'sidebar-item pl-6',
                              isActive(child.href) && 'sidebar-item-active'
                            )}
                          >
                            <child.icon className="w-4 h-4" />
                            {child.title}
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
                    'sidebar-item',
                    isActive(item.href!) && 'sidebar-item-active'
                  )}
                >
                  <item.icon className="w-5 h-5" />
                  {item.title}
                </Link>
              )}
            </li>
          ))}
        </ul>
      </nav>

      {/* Footer */}
      <div className="p-3 border-t border-secondary-200">
        <Link href="/dashboard/ayarlar" className="sidebar-item">
          <Settings className="w-5 h-5" />
          Ayarlar
        </Link>
        <button onClick={logout} className="sidebar-item w-full text-danger hover:bg-red-50">
          <LogOut className="w-5 h-5" />
          Çıkış Yap
        </button>
      </div>
    </aside>
  );
}
