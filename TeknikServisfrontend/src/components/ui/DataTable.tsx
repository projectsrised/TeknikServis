'use client';

import { ReactNode } from 'react';
import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-react';
import { clsx } from 'clsx';

export interface Column<T> {
  key: string;
  header: string;
  render?: (item: T) => ReactNode;
  className?: string;
  sortable?: boolean;
}

interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  loading?: boolean;
  emptyMessage?: string;
  onRowClick?: (item: T) => void;
  keyExtractor: (item: T) => string;
  pagination?: {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    onPageChange: (page: number) => void;
  };
}

export function DataTable<T>({
  columns,
  data,
  loading,
  emptyMessage = 'Veri bulunamadı',
  onRowClick,
  keyExtractor,
  pagination,
}: DataTableProps<T>) {
  if (loading) {
    return (
      <div className="animate-pulse">
        <div className="h-12 bg-secondary-100 rounded mb-2"></div>
        {[...Array(5)].map((_, i) => (
          <div key={i} className="h-14 bg-secondary-50 rounded mb-2"></div>
        ))}
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <div className="text-center py-12 text-secondary-500">
        <p>{emptyMessage}</p>
      </div>
    );
  }

  return (
    <div>
      <div className="overflow-x-auto">
        <table className="table">
          <thead>
            <tr>
              {columns.map((col) => (
                <th key={col.key} className={col.className}>
                  {col.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.map((item) => (
              <tr
                key={keyExtractor(item)}
                onClick={() => onRowClick?.(item)}
                className={clsx(onRowClick && 'cursor-pointer')}
              >
                {columns.map((col) => (
                  <td key={col.key} className={col.className}>
                    {col.render
                      ? col.render(item)
                      : (item as any)[col.key]}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {pagination && pagination.totalPages > 1 && (
        <div className="flex items-center justify-between px-4 py-3 border-t border-secondary-200">
          <div className="text-sm text-secondary-500">
            Toplam {pagination.totalCount} kayıttan{' '}
            {(pagination.page - 1) * pagination.pageSize + 1}-
            {Math.min(pagination.page * pagination.pageSize, pagination.totalCount)} arası
            gösteriliyor
          </div>
          <div className="flex items-center gap-1">
            <button
              onClick={() => pagination.onPageChange(1)}
              disabled={pagination.page === 1}
              className="p-1.5 rounded hover:bg-secondary-100 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ChevronsLeft className="w-4 h-4" />
            </button>
            <button
              onClick={() => pagination.onPageChange(pagination.page - 1)}
              disabled={pagination.page === 1}
              className="p-1.5 rounded hover:bg-secondary-100 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ChevronLeft className="w-4 h-4" />
            </button>

            {/* Page Numbers */}
            <div className="flex items-center gap-1 mx-2">
              {getPageNumbers(pagination.page, pagination.totalPages).map((pageNum, idx) =>
                pageNum === '...' ? (
                  <span key={`ellipsis-${idx}`} className="px-2 text-secondary-400">
                    ...
                  </span>
                ) : (
                  <button
                    key={pageNum}
                    onClick={() => pagination.onPageChange(pageNum as number)}
                    className={clsx(
                      'w-8 h-8 rounded text-sm font-medium transition-colors',
                      pagination.page === pageNum
                        ? 'bg-primary-500 text-white'
                        : 'hover:bg-secondary-100 text-secondary-600'
                    )}
                  >
                    {pageNum}
                  </button>
                )
              )}
            </div>

            <button
              onClick={() => pagination.onPageChange(pagination.page + 1)}
              disabled={pagination.page === pagination.totalPages}
              className="p-1.5 rounded hover:bg-secondary-100 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ChevronRight className="w-4 h-4" />
            </button>
            <button
              onClick={() => pagination.onPageChange(pagination.totalPages)}
              disabled={pagination.page === pagination.totalPages}
              className="p-1.5 rounded hover:bg-secondary-100 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <ChevronsRight className="w-4 h-4" />
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

function getPageNumbers(currentPage: number, totalPages: number): (number | string)[] {
  const pages: (number | string)[] = [];
  const showPages = 5;
  const halfShow = Math.floor(showPages / 2);

  if (totalPages <= showPages) {
    for (let i = 1; i <= totalPages; i++) {
      pages.push(i);
    }
  } else {
    if (currentPage <= halfShow + 1) {
      for (let i = 1; i <= showPages - 1; i++) {
        pages.push(i);
      }
      pages.push('...');
      pages.push(totalPages);
    } else if (currentPage >= totalPages - halfShow) {
      pages.push(1);
      pages.push('...');
      for (let i = totalPages - showPages + 2; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      pages.push(1);
      pages.push('...');
      for (let i = currentPage - 1; i <= currentPage + 1; i++) {
        pages.push(i);
      }
      pages.push('...');
      pages.push(totalPages);
    }
  }

  return pages;
}
