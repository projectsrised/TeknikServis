'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { Wrench, Eye, EyeOff, Loader2 } from 'lucide-react';
import { authApi } from '@/lib/api';
import { useAuthStore } from '@/store/authStore';
import { useAuth } from '@/providers/AuthProvider';
import toast from 'react-hot-toast';

interface LoginForm {
  email: string;
  sifre: string;
}

export default function LoginPage() {
  const router = useRouter();
  const setAuth = useAuthStore((state) => state.setAuth);
  const { isHydrated, isAuthenticated } = useAuth();
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);

  // Redirect to dashboard if already authenticated
  useEffect(() => {
    if (isHydrated && isAuthenticated) {
      router.replace('/dashboard');
    }
  }, [isHydrated, isAuthenticated, router]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({
    defaultValues: {
      email: 'admin@demo.com',
      sifre: 'Admin123!',
    },
  });

  const onSubmit = async (data: LoginForm) => {
    setLoading(true);
    try {
      const response = await authApi.login(data.email, data.sifre);
      const result = response.data;

      console.log('[Login] API Response:', result);

      if (result.basarili && result.data) {
        const { accessToken, refreshToken, kullanici } = result.data;

        console.log('[Login] Calling setAuth with:', {
          accessToken: accessToken?.substring(0, 30) + '...',
          refreshToken: refreshToken?.substring(0, 30) + '...',
          kullanici: kullanici?.email
        });

        // Call setAuth
        setAuth(accessToken, refreshToken, kullanici);

        // Debug: Check localStorage immediately and after delay
        console.log('[Login] localStorage immediately:', localStorage.getItem('auth-storage'));

        setTimeout(() => {
          const stored = localStorage.getItem('auth-storage');
          console.log('[Login] localStorage after 500ms:', stored);

          if (stored) {
            const parsed = JSON.parse(stored);
            console.log('[Login] Parsed state:', {
              isAuthenticated: parsed.state?.isAuthenticated,
              hasToken: !!parsed.state?.token
            });
          }
        }, 500);

        toast.success('Giriş başarılı!');

        // Small delay before navigation to ensure state is saved
        setTimeout(() => {
          router.push('/dashboard');
        }, 100);
      } else {
        toast.error(result.mesaj || 'Giriş başarısız');
      }
    } catch (error: any) {
      console.error('[Login] Error:', error);
      toast.error(error.response?.data?.mesaj || 'Bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary-100 to-secondary-200 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-primary-500 rounded-xl shadow-lg mb-4">
            <Wrench className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-2xl font-bold text-secondary-900">TeknikServis</h1>
          <p className="text-secondary-500 mt-1">Yönetim Paneline Hoş Geldiniz</p>
        </div>

        {/* Login Card */}
        <div className="card p-8">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            {/* Email */}
            <div>
              <label htmlFor="email" className="label">
                E-posta Adresi
              </label>
              <input
                id="email"
                type="email"
                {...register('email', {
                  required: 'E-posta gerekli',
                  pattern: {
                    value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                    message: 'Geçerli bir e-posta adresi girin',
                  },
                })}
                className={errors.email ? 'input-error' : 'input'}
                placeholder="ornek@sirket.com"
              />
              {errors.email && (
                <p className="mt-1 text-sm text-danger">{errors.email.message}</p>
              )}
            </div>

            {/* Password */}
            <div>
              <label htmlFor="sifre" className="label">
                Şifre
              </label>
              <div className="relative">
                <input
                  id="sifre"
                  type={showPassword ? 'text' : 'password'}
                  {...register('sifre', {
                    required: 'Şifre gerekli',
                    minLength: {
                      value: 6,
                      message: 'Şifre en az 6 karakter olmalı',
                    },
                  })}
                  className={errors.sifre ? 'input-error pr-10' : 'input pr-10'}
                  placeholder="••••••••"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-secondary-400 hover:text-secondary-600"
                >
                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
              {errors.sifre && (
                <p className="mt-1 text-sm text-danger">{errors.sifre.message}</p>
              )}
            </div>

            {/* Remember & Forgot */}
            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  className="w-4 h-4 rounded border-secondary-300 text-primary-500 focus:ring-primary-500"
                />
                <span className="text-secondary-600">Beni hatırla</span>
              </label>
              <a href="#" className="text-primary-500 hover:text-primary-600 font-medium">
                Şifremi unuttum
              </a>
            </div>

            {/* Submit */}
            <button
              type="submit"
              disabled={loading}
              className="btn-primary w-full py-3 text-base"
            >
              {loading ? (
                <>
                  <Loader2 className="w-5 h-5 animate-spin mr-2" />
                  Giriş yapılıyor...
                </>
              ) : (
                'Giriş Yap'
              )}
            </button>
          </form>

          {/* Demo Info */}
          <div className="mt-6 p-4 bg-primary-50 rounded-lg">
            <p className="text-sm font-medium text-primary-700 mb-2">Demo Bilgileri:</p>
            <p className="text-xs text-primary-600">
              E-posta: <span className="font-mono">admin@demo.com</span>
            </p>
            <p className="text-xs text-primary-600">
              Şifre: <span className="font-mono">Admin123!</span>
            </p>
          </div>
        </div>

        {/* Footer */}
        <p className="text-center text-sm text-secondary-500 mt-6">
          © 2024 TeknikServis. Tüm hakları saklıdır.
        </p>
      </div>
    </div>
  );
}
