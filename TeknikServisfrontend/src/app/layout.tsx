import type { Metadata } from 'next';
import { Toaster } from 'react-hot-toast';
import { AuthProvider } from '@/providers/AuthProvider';
import './globals.css';

export const metadata: Metadata = {
  title: 'TeknikServis - Yönetim Paneli',
  description: 'Telefon teknik servis ve satış yönetim sistemi',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="tr">
      <body>
        <AuthProvider>
          {children}
        </AuthProvider>
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 4000,
            style: {
              background: '#1e293b',
              color: '#fff',
              borderRadius: '8px',
              padding: '12px 16px',
            },
            success: {
              iconTheme: {
                primary: '#057642',
                secondary: '#fff',
              },
            },
            error: {
              iconTheme: {
                primary: '#cc1016',
                secondary: '#fff',
              },
            },
          }}
        />
      </body>
    </html>
  );
}
