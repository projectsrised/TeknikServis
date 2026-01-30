"use client";
import { ReactNode, useState } from "react";
import { clsx } from "clsx";

// Card Component
interface CardProps {
  children: ReactNode;
  className?: string;
  padding?: boolean;
  hover?: boolean;
}

export function Card({
  children,
  className,
  padding = true,
  hover = false,
}: CardProps) {
  return (
    <div
      className={clsx(
        "bg-white rounded-xl shadow-sm border border-secondary-200",
        padding && "p-6",
        hover && "transition-shadow hover:shadow-md cursor-pointer",
        className,
      )}
    >
      {children}
    </div>
  );
}

// CardHeader Component
interface CardHeaderProps {
  title: string;
  subtitle?: string;
  action?: ReactNode;
}

export function CardHeader({ title, subtitle, action }: CardHeaderProps) {
  return (
    <div className="flex items-center justify-between mb-4">
      <div>
        <h3 className="text-lg font-semibold text-secondary-900">{title}</h3>
        {subtitle && <p className="text-sm text-secondary-500">{subtitle}</p>}
      </div>
      {action}
    </div>
  );
}

// StatCard Component
interface StatCardProps {
  title: string;
  value: string | number;
  change?: { value: string; type: "up" | "down" };
  icon?: ReactNode;
  color?: "primary" | "success" | "warning" | "danger";
}

export function StatCard({
  title,
  value,
  change,
  icon,
  color = "primary",
}: StatCardProps) {
  const colors = {
    primary: "bg-primary-50 text-primary-600",
    success: "bg-green-50 text-green-600",
    warning: "bg-yellow-50 text-yellow-600",
    danger: "bg-red-50 text-red-600",
  };

  return (
    <Card>
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-secondary-500">{title}</p>
          <p className="text-2xl font-bold text-secondary-900 mt-1">{value}</p>
          {change && (
            <p
              className={clsx(
                "text-sm mt-1",
                change.type === "up" ? "text-green-600" : "text-red-600",
              )}
            >
              {change.type === "up" ? "↑" : "↓"} {change.value}
            </p>
          )}
        </div>
        {icon && (
          <div className={clsx("p-3 rounded-lg", colors[color])}>{icon}</div>
        )}
      </div>
    </Card>
  );
}

// PageLoading Component
export function PageLoading() {
  return (
    <div className="flex items-center justify-center min-h-[400px]">
      <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
    </div>
  );
}

// Badge Component
interface BadgeProps {
  children: ReactNode;
  variant?: "primary" | "success" | "warning" | "danger" | "info" | "secondary";
  className?: string;
}

export function Badge({ children, variant = "primary", className }: BadgeProps) {
  const variants = {
    primary: "bg-primary-100 text-primary-700",
    success: "bg-green-100 text-green-700",
    warning: "bg-yellow-100 text-yellow-700",
    danger: "bg-red-100 text-red-700",
    info: "bg-blue-100 text-blue-700",
    secondary: "bg-gray-100 text-gray-700",
  };

  return (
    <span
      className={clsx(
        "px-2 py-1 rounded-full text-xs font-medium",
        variants[variant],
        className,
      )}
    >
      {children}
    </span>
  );
}

// Modal Component
interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  size?: "sm" | "md" | "lg" | "xl";
  footer?: ReactNode;
}

export function Modal({
  isOpen,
  onClose,
  title,
  children,
  size = "md",
  footer,
}: ModalProps) {
  if (!isOpen) return null;

  const sizes = {
    sm: "max-w-sm",
    md: "max-w-md",
    lg: "max-w-2xl",
    xl: "max-w-4xl",
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex min-h-screen items-center justify-center p-4">
        <div className="fixed inset-0 bg-black/50" onClick={onClose} />
        <div
          className={clsx(
            "relative bg-white rounded-xl shadow-xl w-full p-6",
            sizes[size],
          )}
        >
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-secondary-900">
              {title}
            </h3>
            <button
              onClick={onClose}
              className="text-secondary-400 hover:text-secondary-600 text-2xl leading-none"
            >
              ×
            </button>
          </div>
          {children}
          {footer && (
            <div className="flex justify-end gap-3 mt-6 pt-4 border-t border-secondary-200">
              {footer}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

// Tabs Component
interface Tab {
  id: string;
  label: string;
  count?: number;
}

interface TabsProps {
  tabs: Tab[];
  activeTab: string;
  onChange: (tabId: string) => void;
}

export function Tabs({ tabs, activeTab, onChange }: TabsProps) {
  return (
    <div className="border-b border-secondary-200">
      <nav className="flex gap-4">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            onClick={() => onChange(tab.id)}
            className={clsx(
              "py-2 px-1 border-b-2 font-medium text-sm transition-colors flex items-center gap-2",
              activeTab === tab.id
                ? "border-primary-500 text-primary-600"
                : "border-transparent text-secondary-500 hover:text-secondary-700",
            )}
          >
            {tab.label}
            {tab.count !== undefined && (
              <span
                className={clsx(
                  "px-2 py-0.5 rounded-full text-xs",
                  activeTab === tab.id
                    ? "bg-primary-100 text-primary-700"
                    : "bg-secondary-100 text-secondary-600",
                )}
              >
                {tab.count}
              </span>
            )}
          </button>
        ))}
      </nav>
    </div>
  );
}

// ConfirmDialog Component
interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: "danger" | "warning" | "primary";
}

export function ConfirmDialog({
  isOpen,
  onClose,
  onConfirm,
  title,
  message,
  confirmText = "Onayla",
  cancelText = "İptal",
  variant = "danger",
}: ConfirmDialogProps) {
  if (!isOpen) return null;

  const variants = {
    danger: "bg-red-600 hover:bg-red-700",
    warning: "bg-yellow-600 hover:bg-yellow-700",
    primary: "bg-primary-600 hover:bg-primary-700",
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title} size="sm">
      <p className="text-secondary-600 mb-6">{message}</p>
      <div className="flex justify-end gap-3">
        <button
          onClick={onClose}
          className="px-4 py-2 border border-secondary-300 rounded-lg hover:bg-secondary-50"
        >
          {cancelText}
        </button>
        <button
          onClick={() => {
            onConfirm();
            onClose();
          }}
          className={clsx("px-4 py-2 text-white rounded-lg", variants[variant])}
        >
          {confirmText}
        </button>
      </div>
    </Modal>
  );
}

// DataTable Component
interface Column<T> {
  key: string;
  label: string;
  sortable?: boolean;
  render?: (row: T) => ReactNode;
}

interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
  loading?: boolean;
  searchable?: boolean;
  searchPlaceholder?: string;
  emptyMessage?: string;
}

export function DataTable<T extends Record<string, any>>({
  data,
  columns,
  loading = false,
  searchable = false,
  searchPlaceholder = "Ara...",
  emptyMessage = "Kayıt bulunamadı",
}: DataTableProps<T>) {
  const [search, setSearch] = useState("");
  const [sortKey, setSortKey] = useState<string | null>(null);
  const [sortDir, setSortDir] = useState<"asc" | "desc">("asc");

  const filteredData = data.filter((row) => {
    if (!search) return true;
    return Object.values(row).some((val) =>
      String(val).toLowerCase().includes(search.toLowerCase()),
    );
  });

  const sortedData = sortKey
    ? [...filteredData].sort((a, b) => {
        const aVal = a[sortKey];
        const bVal = b[sortKey];
        if (aVal < bVal) return sortDir === "asc" ? -1 : 1;
        if (aVal > bVal) return sortDir === "asc" ? 1 : -1;
        return 0;
      })
    : filteredData;

  const handleSort = (key: string) => {
    if (sortKey === key) {
      setSortDir(sortDir === "asc" ? "desc" : "asc");
    } else {
      setSortKey(key);
      setSortDir("asc");
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div>
      {searchable && (
        <div className="mb-4">
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder={searchPlaceholder}
            className="w-full max-w-xs px-3 py-2 border border-secondary-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
          />
        </div>
      )}

      <div className="overflow-x-auto">
        <table className="w-full">
          <thead>
            <tr className="border-b border-secondary-200">
              {columns.map((col) => (
                <th
                  key={col.key}
                  className={clsx(
                    "text-left py-3 px-4 text-sm font-semibold text-secondary-700",
                    col.sortable && "cursor-pointer hover:bg-secondary-50",
                  )}
                  onClick={() => col.sortable && handleSort(col.key)}
                >
                  <span className="flex items-center gap-1">
                    {col.label}
                    {col.sortable && sortKey === col.key && (
                      <span>{sortDir === "asc" ? "↑" : "↓"}</span>
                    )}
                  </span>
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {sortedData.length === 0 ? (
              <tr>
                <td
                  colSpan={columns.length}
                  className="py-8 text-center text-secondary-500"
                >
                  {emptyMessage}
                </td>
              </tr>
            ) : (
              sortedData.map((row, idx) => (
                <tr
                  key={row.id || idx}
                  className="border-b border-secondary-100 hover:bg-secondary-50"
                >
                  {columns.map((col) => (
                    <td
                      key={col.key}
                      className="py-3 px-4 text-sm text-secondary-900"
                    >
                      {col.render ? col.render(row) : row[col.key]}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// Loading Spinner
export function LoadingSpinner({ size = "md" }: { size?: "sm" | "md" | "lg" }) {
  const sizes = {
    sm: "h-4 w-4",
    md: "h-8 w-8",
    lg: "h-12 w-12",
  };

  return (
    <div
      className={clsx(
        "animate-spin rounded-full border-b-2 border-primary-600",
        sizes[size],
      )}
    />
  );
}

// Empty State
interface EmptyStateProps {
  title: string;
  description?: string;
  action?: ReactNode;
}

export function EmptyState({ title, description, action }: EmptyStateProps) {
  return (
    <div className="text-center py-12">
      <h3 className="text-lg font-medium text-secondary-900 mb-2">{title}</h3>
      {description && <p className="text-secondary-500 mb-4">{description}</p>}
      {action}
    </div>
  );
}

// Re-export BrandModelSelect components
export { BrandSelect, ModelSelect, BrandModelSelect } from './BrandModelSelect';
