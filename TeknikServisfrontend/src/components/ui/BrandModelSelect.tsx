'use client';

import { useEffect, useState, useCallback } from 'react';
import { markaApi, modelApi } from '@/lib/api';
import { Marka, Model, MarkaWithModels } from '@/types';
import { Search, X, ChevronDown, Check, Loader2 } from 'lucide-react';
import { clsx } from 'clsx';

interface BrandSelectProps {
  value?: string;
  onChange: (markaId: string | undefined, markaAd?: string) => void;
  placeholder?: string;
  disabled?: boolean;
  error?: boolean;
  className?: string;
}

export function BrandSelect({
  value,
  onChange,
  placeholder = 'Marka seçin',
  disabled = false,
  error = false,
  className = '',
}: BrandSelectProps) {
  const [markalar, setMarkalar] = useState<Marka[]>([]);
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState('');

  useEffect(() => {
    loadMarkalar();
  }, []);

  const loadMarkalar = async () => {
    setLoading(true);
    try {
      const response = await markaApi.getAll();
      if (response.data.basarili) {
        setMarkalar(response.data.data);
      }
    } catch (error) {
      console.error('Markalar yüklenemedi', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredMarkalar = markalar.filter((m) =>
    m.ad.toLowerCase().includes(search.toLowerCase())
  );

  const selectedMarka = markalar.find((m) => m.id === value);

  const handleSelect = (marka: Marka) => {
    onChange(marka.id, marka.ad);
    setIsOpen(false);
    setSearch('');
  };

  const handleClear = (e: React.MouseEvent) => {
    e.stopPropagation();
    onChange(undefined, undefined);
    setSearch('');
  };

  return (
    <div className={clsx('relative', className)}>
      <div
        onClick={() => !disabled && setIsOpen(!isOpen)}
        className={clsx(
          'input flex items-center justify-between cursor-pointer',
          disabled && 'bg-secondary-100 cursor-not-allowed',
          error && 'border-danger focus:ring-danger',
          isOpen && 'ring-2 ring-primary-500 border-primary-500'
        )}
      >
        <span className={selectedMarka ? 'text-secondary-900' : 'text-secondary-400'}>
          {selectedMarka?.ad || placeholder}
        </span>
        <div className="flex items-center gap-1">
          {selectedMarka && !disabled && (
            <button
              onClick={handleClear}
              className="p-1 hover:bg-secondary-100 rounded"
            >
              <X className="w-4 h-4 text-secondary-400" />
            </button>
          )}
          {loading ? (
            <Loader2 className="w-4 h-4 text-secondary-400 animate-spin" />
          ) : (
            <ChevronDown className={clsx('w-4 h-4 text-secondary-400 transition-transform', isOpen && 'rotate-180')} />
          )}
        </div>
      </div>

      {isOpen && (
        <div className="absolute z-50 w-full mt-1 bg-white border border-secondary-200 rounded-lg shadow-lg">
          <div className="p-2 border-b border-secondary-200">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-secondary-400" />
              <input
                type="text"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Marka ara..."
                className="input pl-9 py-2 text-sm"
                autoFocus
              />
            </div>
          </div>
          <div className="max-h-60 overflow-y-auto">
            {filteredMarkalar.length === 0 ? (
              <div className="p-3 text-sm text-secondary-500 text-center">
                Marka bulunamadı
              </div>
            ) : (
              filteredMarkalar.map((marka) => (
                <div
                  key={marka.id}
                  onClick={() => handleSelect(marka)}
                  className={clsx(
                    'px-3 py-2 cursor-pointer flex items-center justify-between hover:bg-secondary-50',
                    value === marka.id && 'bg-primary-50'
                  )}
                >
                  <div>
                    <p className="text-sm font-medium text-secondary-900">{marka.ad}</p>
                    <p className="text-xs text-secondary-500">{marka.modelSayisi} model</p>
                  </div>
                  {value === marka.id && <Check className="w-4 h-4 text-primary-600" />}
                </div>
              ))
            )}
          </div>
        </div>
      )}

      {isOpen && (
        <div
          className="fixed inset-0 z-40"
          onClick={() => setIsOpen(false)}
        />
      )}
    </div>
  );
}

interface ModelSelectProps {
  markaId?: string;
  value?: string;
  onChange: (modelId: string | undefined, modelAd?: string) => void;
  placeholder?: string;
  disabled?: boolean;
  error?: boolean;
  className?: string;
}

export function ModelSelect({
  markaId,
  value,
  onChange,
  placeholder = 'Model seçin',
  disabled = false,
  error = false,
  className = '',
}: ModelSelectProps) {
  const [modeller, setModeller] = useState<Model[]>([]);
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState('');

  useEffect(() => {
    if (markaId) {
      loadModeller(markaId);
    } else {
      setModeller([]);
    }
  }, [markaId]);

  const loadModeller = async (markaId: string) => {
    setLoading(true);
    try {
      const response = await modelApi.getByMarkaId(markaId);
      if (response.data.basarili) {
        setModeller(response.data.data);
      }
    } catch (error) {
      console.error('Modeller yüklenemedi', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredModeller = modeller.filter((m) =>
    m.ad.toLowerCase().includes(search.toLowerCase())
  );

  const selectedModel = modeller.find((m) => m.id === value);

  const handleSelect = (model: Model) => {
    onChange(model.id, model.ad);
    setIsOpen(false);
    setSearch('');
  };

  const handleClear = (e: React.MouseEvent) => {
    e.stopPropagation();
    onChange(undefined, undefined);
    setSearch('');
  };

  const isDisabled = disabled || !markaId;

  return (
    <div className={clsx('relative', className)}>
      <div
        onClick={() => !isDisabled && setIsOpen(!isOpen)}
        className={clsx(
          'input flex items-center justify-between cursor-pointer',
          isDisabled && 'bg-secondary-100 cursor-not-allowed',
          error && 'border-danger focus:ring-danger',
          isOpen && 'ring-2 ring-primary-500 border-primary-500'
        )}
      >
        <span className={selectedModel ? 'text-secondary-900' : 'text-secondary-400'}>
          {selectedModel?.ad || (markaId ? placeholder : 'Önce marka seçin')}
        </span>
        <div className="flex items-center gap-1">
          {selectedModel && !isDisabled && (
            <button
              onClick={handleClear}
              className="p-1 hover:bg-secondary-100 rounded"
            >
              <X className="w-4 h-4 text-secondary-400" />
            </button>
          )}
          {loading ? (
            <Loader2 className="w-4 h-4 text-secondary-400 animate-spin" />
          ) : (
            <ChevronDown className={clsx('w-4 h-4 text-secondary-400 transition-transform', isOpen && 'rotate-180')} />
          )}
        </div>
      </div>

      {isOpen && (
        <div className="absolute z-50 w-full mt-1 bg-white border border-secondary-200 rounded-lg shadow-lg">
          <div className="p-2 border-b border-secondary-200">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-secondary-400" />
              <input
                type="text"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Model ara..."
                className="input pl-9 py-2 text-sm"
                autoFocus
              />
            </div>
          </div>
          <div className="max-h-60 overflow-y-auto">
            {filteredModeller.length === 0 ? (
              <div className="p-3 text-sm text-secondary-500 text-center">
                {modeller.length === 0 ? 'Bu markaya ait model yok' : 'Model bulunamadı'}
              </div>
            ) : (
              filteredModeller.map((model) => (
                <div
                  key={model.id}
                  onClick={() => handleSelect(model)}
                  className={clsx(
                    'px-3 py-2 cursor-pointer flex items-center justify-between hover:bg-secondary-50',
                    value === model.id && 'bg-primary-50'
                  )}
                >
                  <div>
                    <p className="text-sm font-medium text-secondary-900">{model.ad}</p>
                    {model.kod && <p className="text-xs text-secondary-500">{model.kod}</p>}
                  </div>
                  {value === model.id && <Check className="w-4 h-4 text-primary-600" />}
                </div>
              ))
            )}
          </div>
        </div>
      )}

      {isOpen && (
        <div
          className="fixed inset-0 z-40"
          onClick={() => setIsOpen(false)}
        />
      )}
    </div>
  );
}

interface BrandModelSelectProps {
  markaId?: string;
  modelId?: string;
  onMarkaChange: (markaId: string | undefined, markaAd?: string) => void;
  onModelChange: (modelId: string | undefined, modelAd?: string) => void;
  markaLabel?: string;
  modelLabel?: string;
  markaPlaceholder?: string;
  modelPlaceholder?: string;
  disabled?: boolean;
  markaError?: boolean;
  modelError?: boolean;
  className?: string;
}

export function BrandModelSelect({
  markaId,
  modelId,
  onMarkaChange,
  onModelChange,
  markaLabel = 'Marka',
  modelLabel = 'Model',
  markaPlaceholder = 'Marka seçin',
  modelPlaceholder = 'Model seçin',
  disabled = false,
  markaError = false,
  modelError = false,
  className = '',
}: BrandModelSelectProps) {
  const handleMarkaChange = (newMarkaId: string | undefined, markaAd?: string) => {
    onMarkaChange(newMarkaId, markaAd);
    // Marka değişince model'i temizle
    if (newMarkaId !== markaId) {
      onModelChange(undefined, undefined);
    }
  };

  return (
    <div className={clsx('grid grid-cols-2 gap-4', className)}>
      <div>
        <label className="label">{markaLabel}</label>
        <BrandSelect
          value={markaId}
          onChange={handleMarkaChange}
          placeholder={markaPlaceholder}
          disabled={disabled}
          error={markaError}
        />
      </div>
      <div>
        <label className="label">{modelLabel}</label>
        <ModelSelect
          markaId={markaId}
          value={modelId}
          onChange={onModelChange}
          placeholder={modelPlaceholder}
          disabled={disabled}
          error={modelError}
        />
      </div>
    </div>
  );
}
