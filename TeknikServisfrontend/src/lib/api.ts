import axios, { AxiosError, InternalAxiosRequestConfig } from 'axios';
import { useAuthStore } from '@/store/authStore';

// Environment'a göre API URL belirleme
// Development: Next.js rewrite kullan (CORS sorunu olmaz)
// Production: NEXT_PUBLIC_API_URL environment variable'dan alınır
const getApiUrl = (): string => {
  // Client-side kontrol
  if (typeof window === 'undefined') {
    // Server-side: Development'ta boş, production'da environment variable
    return process.env.NEXT_PUBLIC_API_URL || '';
  }
  
  // Client-side: Development'ta her zaman rewrite kullan
  // Production'da environment variable kullan
  const isDevelopment = process.env.NODE_ENV === 'development' || 
                       window.location.hostname === 'localhost' ||
                       window.location.hostname === '127.0.0.1';
  
  if (isDevelopment) {
    // Development'ta Next.js rewrite kullan (relative path)
    // next.config.js'de /api/* -> http://localhost:5000/api/* rewrite'ı var
    // Bu sayede CORS sorunu olmaz çünkü aynı origin'den istek atılır
    return '';
  }
  
  // Production'da environment variable kullan
  if (process.env.NEXT_PUBLIC_API_URL) {
    return process.env.NEXT_PUBLIC_API_URL;
  }
  
  // Fallback: Production'da environment variable yoksa window.location kullan
  const protocol = window.location.protocol;
  const host = window.location.host;
  return `${protocol}//${host}`;
};

const API_URL = getApiUrl();

// Base URL oluştur
// Development'ta boş string ise sadece /api kullan (Next.js rewrite)
// Production'da tam URL kullan
const baseURL = API_URL ? `${API_URL}/api` : '/api';

// Debug log (sadece development'ta)
if (typeof window !== 'undefined' && process.env.NODE_ENV === 'development') {
  console.log('[API] Base URL:', baseURL, '| API_URL:', API_URL, '| NODE_ENV:', process.env.NODE_ENV);
}

export const api = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - Token ekleme
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor - Error handling
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    if (error.response?.status === 401) {
      // Token expired veya invalid
      useAuthStore.getState().logout();
      if (typeof window !== 'undefined') {
        window.location.href = '/';
      }
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authApi = {
  login: (email: string, sifre: string) =>
    api.post('/auth/login', { email, sifre }),
  
  register: (data: any) =>
    api.post('/auth/register', data),
  
  me: () =>
    api.get('/auth/me'),
  
  changePassword: (data: { mevcutSifre: string; yeniSifre: string }) =>
    api.post('/auth/change-password', data),
  
  refreshToken: (refreshToken: string) =>
    api.post('/auth/refresh-token', { refreshToken }),
};

// Bayi API
export const bayiApi = {
  getAll: () => api.get('/bayi'),
  getById: (id: string) => api.get(`/bayi/${id}`),
  create: (data: any) => api.post('/bayi', data),
  update: (id: string, data: any) => api.put(`/bayi/${id}`, data),
  delete: (id: string) => api.delete(`/bayi/${id}`),
  getDepolar: (id: string) => api.get(`/bayi/${id}/depolar`),
  getKasa: (id: string) => api.get(`/bayi/${id}/kasa`),
};

// Depo API
export const depoApi = {
  getById: (id: string) => api.get(`/depo/${id}`),
  getByBayiId: (bayiId: string) => api.get(`/depo/bayi/${bayiId}`),
  create: (data: any) => api.post('/depo', data),
  update: (id: string, data: any) => api.put(`/depo/${id}`, data),
  delete: (id: string) => api.delete(`/depo/${id}`),
};

// Musteri API
export const musteriApi = {
  getList: (page = 1, pageSize = 20, arama?: string) =>
    api.get('/musteri', { params: { page, pageSize, arama } }),
  getById: (id: string) => api.get(`/musteri/${id}`),
  getByTelefon: (telefon: string) => api.get(`/musteri/telefon/${telefon}`),
  create: (data: any) => api.post('/musteri', data),
  update: (id: string, data: any) => api.put(`/musteri/${id}`, data),
  delete: (id: string) => api.delete(`/musteri/${id}`),
  getSatislar: (id: string) => api.get(`/musteri/${id}/satislar`),
  getTeknikServisler: (id: string) => api.get(`/musteri/${id}/teknik-servisler`),
};

// Marka API
export const markaApi = {
  getAll: () => api.get('/marka'),
  getWithModels: () => api.get('/marka/with-models'),
  getById: (id: string) => api.get(`/marka/${id}`),
  create: (data: any) => api.post('/marka', data),
  update: (id: string, data: any) => api.put(`/marka/${id}`, data),
  delete: (id: string) => api.delete(`/marka/${id}`),
};

// Model API
export const modelApi = {
  getAll: () => api.get('/model'),
  getByMarkaId: (markaId: string) => api.get(`/model/marka/${markaId}`),
  getById: (id: string) => api.get(`/model/${id}`),
  create: (data: any) => api.post('/model', data),
  update: (id: string, data: any) => api.put(`/model/${id}`, data),
  delete: (id: string) => api.delete(`/model/${id}`),
};

// Kategori API
export const kategoriApi = {
  getAll: () => api.get('/kategori'),
  getTree: () => api.get('/kategori/tree'),
  getById: (id: string) => api.get(`/kategori/${id}`),
  create: (data: any) => api.post('/kategori', data),
  update: (id: string, data: any) => api.put(`/kategori/${id}`, data),
  delete: (id: string) => api.delete(`/kategori/${id}`),
};

// Urun API
export const urunApi = {
  getList: (page = 1, pageSize = 20, kategoriId?: string, arama?: string) =>
    api.get('/urun', { params: { page, pageSize, kategoriId, arama } }),
  getById: (id: string) => api.get(`/urun/${id}`),
  getByBarkod: (barkod: string) => api.get(`/urun/barkod/${barkod}`),
  create: (data: any) => api.post('/urun', data),
  update: (id: string, data: any) => api.put(`/urun/${id}`, data),
  delete: (id: string) => api.delete(`/urun/${id}`),
  getSeriNumaralari: (id: string, depoId?: string) =>
    api.get(`/urun/${id}/seri-numaralari`, { params: { depoId } }),
  getStok: (id: string, depoId?: string) =>
    api.get(`/urun/${id}/stok`, { params: { depoId } }),
};

// Satis API
export const satisApi = {
  getList: (page = 1, pageSize = 20, baslangic?: string, bitis?: string) =>
    api.get('/satis', { params: { page, pageSize, baslangic, bitis } }),
  getById: (id: string) => api.get(`/satis/${id}`),
  getBySatisNo: (satisNo: string) => api.get(`/satis/no/${satisNo}`),
  create: (data: any) => api.post('/satis', data),
  iptal: (id: string, neden: string) => api.post(`/satis/${id}/iptal`, { neden }),
  validateSeri: (seriNo: string) => api.post('/satis/validate-seri', { seriNo }),
};

// Teknik Servis API
export const teknikServisApi = {
  getList: (page = 1, pageSize = 20, durum?: number) =>
    api.get('/teknik-servis', { params: { page, pageSize, durum } }),
  getById: (id: string) => api.get(`/teknik-servis/${id}`),
  getByServisNo: (servisNo: string) => api.get(`/teknik-servis/no/${servisNo}`),
  getByTelefon: (telefon: string) => api.get(`/teknik-servis/telefon/${telefon}`),
  create: (data: any) => api.post('/teknik-servis', data),
  updateDurum: (id: string, data: any) => api.put(`/teknik-servis/${id}/durum`, data),
  parcaEkle: (id: string, data: any) => api.post(`/teknik-servis/${id}/parca`, data),
  teslim: (id: string) => api.post(`/teknik-servis/${id}/teslim`),
  odemeAl: (id: string, data: any) => api.post(`/teknik-servis/${id}/odeme`, data),
  iptal: (id: string, neden: string) => api.post(`/teknik-servis/${id}/iptal`, { neden }),
};

// Transfer API
export const transferApi = {
  getList: (page = 1, pageSize = 20) =>
    api.get('/transfer', { params: { page, pageSize } }),
  getById: (id: string) => api.get(`/transfer/${id}`),
  create: (data: any) => api.post('/transfer', data),
  onayla: (id: string) => api.post(`/transfer/${id}/onayla`),
  tamamla: (id: string) => api.post(`/transfer/${id}/tamamla`),
  iptal: (id: string, neden: string) => api.post(`/transfer/${id}/iptal`, { neden }),
  kalemTeslimAl: (transferId: string, kalemId: string) =>
    api.post(`/transfer/${transferId}/kalem/${kalemId}/teslim-al`),
};

// Iade API
export const iadeApi = {
  getList: (page = 1, pageSize = 20) =>
    api.get('/iade', { params: { page, pageSize } }),
  getById: (id: string) => api.get(`/iade/${id}`),
  create: (data: any) => api.post('/iade', data),
  onayla: (id: string) => api.post(`/iade/${id}/onayla`),
  reddet: (id: string, neden: string) => api.post(`/iade/${id}/reddet`, { neden }),
  tamamla: (id: string) => api.post(`/iade/${id}/tamamla`),
};

// Stok API
export const stokApi = {
  getByUrun: (urunId: string) => api.get(`/stok/urun/${urunId}`),
  getDurum: (bayiId?: string) => api.get('/stok/durum', { params: { bayiId } }),
  getKritik: () => api.get('/stok/kritik'),
  getHareketler: (urunId?: string, depoId?: string, page = 1, pageSize = 20) =>
    api.get('/stok/hareketler', { params: { urunId, depoId, page, pageSize } }),
  getBySeriNo: (seriNo: string) => api.get(`/stok/seri/${seriNo}`),
  giris: (data: any) => api.post('/stok/giris', data),
  cikis: (data: any) => api.post('/stok/cikis', data),
  getTedarikciler: () => api.get('/stok/tedarikciler'),
  girisWithFatura: (data: any) => api.post('/stok/giris-fatura', data),
  getKalemler: (page = 1, pageSize = 20, urunId?: string, depoId?: string, satildi?: boolean) =>
    api.get('/stok/kalemler', { params: { page, pageSize, urunId, depoId, satildi } }),
  updateKalem: (id: string, data: any) => api.put(`/stok/kalem/${id}`, data),
  deleteKalem: (id: string) => api.delete(`/stok/kalem/${id}`),
  deleteUrunStoklari: (urunId: string, depoId?: string) => 
    api.delete(`/stok/urun/${urunId}`, { params: { depoId } }),
  deleteHareket: (id: string) => api.delete(`/stok/hareket/${id}`),
};

// Kasa API
export const kasaApi = {
  getById: (id: string) => api.get(`/kasa/${id}`),
  getByBayiId: (bayiId: string) => api.get(`/kasa/bayi/${bayiId}`),
  create: (data: any) => api.post('/kasa', data),
  hareketEkle: (data: any) => api.post('/kasa/hareket', data),
  getHareketler: (kasaId: string, page = 1, pageSize = 20) =>
    api.get(`/kasa/${kasaId}/hareketler`, { params: { page, pageSize } }),
  getGunlukCiro: (bayiId: string, tarih?: string) =>
    api.get(`/kasa/bayi/${bayiId}/gunluk-ciro`, { params: { tarih } }),
};

// Personel API
export const personelApi = {
  getList: (bayiId?: string) => api.get('/personel', { params: { bayiId } }),
  getById: (id: string) => api.get(`/personel/${id}`),
  create: (data: any) => api.post('/personel', data),
  update: (id: string, data: any) => api.put(`/personel/${id}`, data),
  delete: (id: string) => api.delete(`/personel/${id}`),
  maasHesapla: (id: string, yil: number, ay: number) =>
    api.get(`/personel/${id}/maas-hesapla`, { params: { yil, ay } }),
  maasOde: (id: string, yil: number, ay: number) =>
    api.post(`/personel/${id}/maas-ode`, { yil, ay }),
  getMaasOdemeleri: (id: string) => api.get(`/personel/${id}/maas-odemeleri`),
  avansTalep: (id: string, tutar: number) =>
    api.post(`/personel/${id}/avans-talep`, { tutar }),
  avansOnayla: (avansId: string) => api.post(`/personel/avans/${avansId}/onayla`),
  getAvanslar: (id: string) => api.get(`/personel/${id}/avanslar`),
  izinTalep: (data: any) => api.post('/personel/izin-talep', data),
  izinOnayla: (izinId: string) => api.post(`/personel/izin/${izinId}/onayla`),
  getIzinler: (id: string) => api.get(`/personel/${id}/izinler`),
  getKalanIzin: (id: string) => api.get(`/personel/${id}/kalan-izin`),
  mesaiEkle: (data: any) => api.post('/personel/mesai', data),
  mesaiOnayla: (mesaiId: string) => api.post(`/personel/mesai/${mesaiId}/onayla`),
  getMesailer: (id: string, yil?: number, ay?: number) =>
    api.get(`/personel/${id}/mesailer`, { params: { yil, ay } }),
};

// Rapor API
export const raporApi = {
  getOzet: (bayiId?: string) => api.get('/rapor/ozet', { params: { bayiId } }),
  getSatis: (baslangic: string, bitis: string, bayiId?: string) =>
    api.get('/rapor/satis', { params: { baslangic, bitis, bayiId } }),
  getBayiPerformans: (baslangic: string, bitis: string) =>
    api.get('/rapor/bayi-performans', { params: { baslangic, bitis } }),
  getPersonelPerformans: (baslangic: string, bitis: string, bayiId?: string) =>
    api.get('/rapor/personel-performans', { params: { baslangic, bitis, bayiId } }),
  getStok: (bayiId?: string) => api.get('/rapor/stok', { params: { bayiId } }),
};
// Fatura API
export const faturaApi = {
  getList: (page = 1, pageSize = 20, tip?: string) =>
    api.get('/fatura', { params: { page, pageSize, tip } }),
  getById: (id: string) => api.get(`/fatura/${id}`),
  create: (data: any) => api.post('/fatura', data),
  onayla: (id: string) => api.post(`/fatura/${id}/onayla`),
  iptal: (id: string, neden?: string) => api.post(`/fatura/${id}/iptal`, { neden }),
};

// Sayim API
export const sayimApi = {
  getList: (page = 1, pageSize = 20, depoId?: string) =>
    api.get('/sayim', { params: { page, pageSize, depoId } }),
  getById: (id: string) => api.get(`/sayim/${id}`),
  baslat: (data: any) => api.post('/sayim/baslat', data),
  seriTara: (id: string, seriNo: string) => api.post(`/sayim/${id}/seri-tara`, { seriNo }),
  tamamla: (id: string) => api.post(`/sayim/${id}/tamamla`),
  iptal: (id: string) => api.post(`/sayim/${id}/iptal`),
  getRapor: (id: string) => api.get(`/sayim/${id}/rapor`),
};


export default api;
