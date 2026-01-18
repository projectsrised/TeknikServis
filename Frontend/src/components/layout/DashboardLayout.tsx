'use client';

import { useState } from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import {
  LayoutDashboard,
  ShoppingCart,
  Wrench,
  Package,
  FolderTree,
  ArrowRightLeft,
  RotateCcw,
  ClipboardList,
  Users,
  Settings,
  LogOut,
  Menu,
  X,
  ChevronDown,
  Building2,
  Wallet,
} from 'lucide-react';
import { UserRole } from '@/types';

interface NavItem {
  name: string;
  href: string;
  icon: any;
  roles?: UserRole[];
  children?: { name: string; href: string }[];
}

const navigation: NavItem[] = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard },
  { name: 'Satışlar', href: '/sales', icon: ShoppingCart },
  { name: 'Teknik Servis', href: '/services', icon: Wrench, roles: [UserRole.TenantAdmin, UserRole.BranchAdmin, UserRole.BranchManager, UserRole.TechnicalStaff] },
  { name: 'Ürünler', href: '/products', icon: Package },
  { name: 'Kategoriler', href: '/categories', icon: FolderTree, roles: [UserRole.TenantAdmin, UserRole.BranchAdmin] },
  { name: 'Transferler', href: '/transfers', icon: ArrowRightLeft, roles: [UserRole.TenantAdmin, UserRole.BranchAdmin, UserRole.BranchManager] },
  { name: 'İadeler', href: '/returns', icon: RotateCcw },
  { name: 'Sayım', href: '/inventory', icon: ClipboardList, roles: [UserRole.TenantAdmin, UserRole.BranchAdmin, UserRole.BranchManager] },
  { name: 'Kasa', href: '/cash', icon: Wallet, roles: [UserRole.TenantAdmin, UserRole.BranchAdmin, UserRole.BranchManager] },
  { name: 'Personel', href: '/employees', icon: Users, roles: [UserRole.TenantAdmin, UserRole.BranchAdmin, UserRole.BranchManager] },
  { name: 'Bayiler', href: '/branches', icon: Building2, roles: [UserRole.TenantAdmin] },
  { name: 'Ayarlar', href: '/settings', icon: Settings },
];

export function DashboardLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const { user, logout } = useAuthStore();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const filteredNavigation = navigation.filter((item) => {
    if (!item.roles) return true;
    return user && item.roles.includes(user.role);
  });

  const handleLogout = async () => {
    await logout();
    window.location.href = '/login';
  };

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Mobile sidebar */}
      <div className={`fixed inset-0 z-50 lg:hidden ${sidebarOpen ? '' : 'hidden'}`}>
        <div className="fixed inset-0 bg-gray-600 bg-opacity-75" onClick={() => setSidebarOpen(false)} />
        <div className="fixed inset-y-0 left-0 flex w-64 flex-col bg-white">
          <div className="flex h-16 items-center justify-between px-4 border-b">
            <span className="text-xl font-bold text-blue-600">Teknik Servis</span>
            <button onClick={() => setSidebarOpen(false)}>
              <X className="h-6 w-6" />
            </button>
          </div>
          <nav className="flex-1 overflow-y-auto py-4">
            {filteredNavigation.map((item) => (
              <Link
                key={item.name}
                href={item.href}
                className={`flex items-center px-4 py-3 text-sm font-medium ${
                  pathname === item.href
                    ? 'bg-blue-50 text-blue-600 border-r-4 border-blue-600'
                    : 'text-gray-600 hover:bg-gray-50'
                }`}
                onClick={() => setSidebarOpen(false)}
              >
                <item.icon className="mr-3 h-5 w-5" />
                {item.name}
              </Link>
            ))}
          </nav>
        </div>
      </div>

      {/* Desktop sidebar */}
      <div className="hidden lg:fixed lg:inset-y-0 lg:flex lg:w-64 lg:flex-col">
        <div className="flex min-h-0 flex-1 flex-col bg-white border-r">
          <div className="flex h-16 items-center px-4 border-b">
            <span className="text-xl font-bold text-blue-600">Teknik Servis</span>
          </div>
          <nav className="flex-1 overflow-y-auto py-4">
            {filteredNavigation.map((item) => (
              <Link
                key={item.name}
                href={item.href}
                className={`flex items-center px-4 py-3 text-sm font-medium transition ${
                  pathname === item.href
                    ? 'bg-blue-50 text-blue-600 border-r-4 border-blue-600'
                    : 'text-gray-600 hover:bg-gray-50'
                }`}
              >
                <item.icon className="mr-3 h-5 w-5" />
                {item.name}
              </Link>
            ))}
          </nav>
        </div>
      </div>

      {/* Main content */}
      <div className="lg:pl-64">
        {/* Top header */}
        <header className="sticky top-0 z-40 flex h-16 items-center gap-x-4 border-b bg-white px-4 shadow-sm sm:px-6 lg:px-8">
          <button
            type="button"
            className="lg:hidden -m-2.5 p-2.5 text-gray-700"
            onClick={() => setSidebarOpen(true)}
          >
            <Menu className="h-6 w-6" />
          </button>

          <div className="flex flex-1 items-center justify-end gap-x-4">
            {/* Branch info */}
            {user?.branchName && (
              <div className="hidden sm:flex items-center text-sm text-gray-500">
                <Building2 className="h-4 w-4 mr-1" />
                {user.branchName}
              </div>
            )}

            {/* User menu */}
            <div className="relative group">
              <button className="flex items-center gap-x-2 rounded-lg px-3 py-2 hover:bg-gray-100 transition">
                <div className="h-8 w-8 rounded-full bg-blue-600 flex items-center justify-center text-white font-medium">
                  {user?.firstName?.[0]}{user?.lastName?.[0]}
                </div>
                <div className="hidden sm:block text-left">
                  <p className="text-sm font-medium text-gray-700">{user?.fullName}</p>
                  <p className="text-xs text-gray-500">{user?.roleName}</p>
                </div>
                <ChevronDown className="h-4 w-4 text-gray-400" />
              </button>

              <div className="absolute right-0 mt-2 w-48 origin-top-right rounded-lg bg-white shadow-lg ring-1 ring-black ring-opacity-5 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all">
                <div className="py-1">
                  <Link
                    href="/settings"
                    className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  >
                    Ayarlar
                  </Link>
                  <button
                    onClick={handleLogout}
                    className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-gray-100 flex items-center"
                  >
                    <LogOut className="h-4 w-4 mr-2" />
                    Çıkış Yap
                  </button>
                </div>
              </div>
            </div>
          </div>
        </header>

        {/* Page content */}
        <main className="p-4 sm:p-6 lg:p-8">
          {children}
        </main>
      </div>
    </div>
  );
}
