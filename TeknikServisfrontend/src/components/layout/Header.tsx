'use client';

import { Bell, Search, ChevronDown, User, Settings, LogOut } from 'lucide-react';
import { useAuthStore } from '@/store/authStore';
import { useState, useRef, useEffect } from 'react';
import Link from 'next/link';

export default function Header() {
  const { user, logout } = useAuthStore();
  const [showDropdown, setShowDropdown] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setShowDropdown(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <header className="h-16 bg-white border-b border-secondary-200 fixed top-0 right-0 left-16 group-hover:left-64 z-40 transition-all duration-300">
      <div className="h-full px-6 flex items-center justify-between">
        {/* Search */}
        <div className="flex-1 max-w-xl">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-secondary-400" />
            <input
              type="text"
              placeholder="Ara... (Müşteri, Ürün, Servis No)"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full pl-10 pr-4 py-2 bg-secondary-100 border-0 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:bg-white transition-all"
            />
          </div>
        </div>

        {/* Right Side */}
        <div className="flex items-center gap-2">
          {/* Notifications */}
          <button className="relative p-2 text-secondary-600 hover:bg-secondary-100 rounded-full transition-colors">
            <Bell className="w-5 h-5" />
            <span className="absolute top-1 right-1 w-2 h-2 bg-danger rounded-full"></span>
          </button>

          {/* User Dropdown */}
          <div className="relative" ref={dropdownRef}>
            <button
              onClick={() => setShowDropdown(!showDropdown)}
              className="flex items-center gap-2 p-1.5 hover:bg-secondary-100 rounded-lg transition-colors"
            >
              <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center">
                <span className="text-primary-600 font-medium text-sm">
                  {user?.ad?.charAt(0)}{user?.soyad?.charAt(0)}
                </span>
              </div>
              <div className="text-left hidden md:block">
                <p className="text-sm font-medium text-secondary-900">{user?.adSoyad}</p>
                <p className="text-xs text-secondary-500">{user?.bayiAd || user?.tenantAd}</p>
              </div>
              <ChevronDown className="w-4 h-4 text-secondary-400" />
            </button>

            {showDropdown && (
              <div className="dropdown animate-fadeIn">
                <div className="px-4 py-3 border-b border-secondary-100">
                  <p className="text-sm font-medium text-secondary-900">{user?.adSoyad}</p>
                  <p className="text-xs text-secondary-500">{user?.email}</p>
                </div>
                <div className="py-1">
                  <Link href="/dashboard/profil" className="dropdown-item">
                    <User className="w-4 h-4" />
                    Profilim
                  </Link>
                  <Link href="/dashboard/ayarlar" className="dropdown-item">
                    <Settings className="w-4 h-4" />
                    Ayarlar
                  </Link>
                </div>
                <div className="border-t border-secondary-100 py-1">
                  <button
                    onClick={logout}
                    className="dropdown-item w-full text-danger hover:bg-red-50"
                  >
                    <LogOut className="w-4 h-4" />
                    Çıkış Yap
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}
