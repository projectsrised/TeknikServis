'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/providers/AuthProvider';
import Sidebar from '@/components/layout/Sidebar';
import Header from '@/components/layout/Header';
import { Loader2 } from 'lucide-react';

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const { isHydrated, isAuthenticated, isLoading } = useAuth();

  useEffect(() => {
    if (isHydrated && !isAuthenticated) {
      router.replace('/');
    }
  }, [isHydrated, isAuthenticated, router]);

  // Show loading while hydrating
  if (isLoading || !isHydrated) {
    return (
      <div className="min-h-screen bg-secondary-100 flex items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="w-8 h-8 animate-spin text-primary-500" />
          <p className="text-secondary-500">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  // After hydration, if not authenticated, show nothing (redirect will happen)
  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="min-h-screen bg-secondary-100">
      <Sidebar />
      <Header />
      <main className="ml-16 pt-16 p-6">
        {children}
      </main>
    </div>
  );
}
