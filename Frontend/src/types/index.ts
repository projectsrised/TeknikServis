// Auth Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface User {
  id: string;
  tenantId: string;
  branchId?: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone?: string;
  role: UserRole;
  roleName: string;
  tenantName?: string;
  branchName?: string;
  isActive: boolean;
}

export enum UserRole {
  TenantAdmin = 1,
  BranchAdmin = 2,
  BranchManager = 3,
  SalesStaff = 4,
  TechnicalStaff = 5
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface PagedRequest {
  page: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  sortDesc?: boolean;
}

// Category Types
export interface Category {
  id: string;
  parentId?: string;
  name: string;
  description?: string;
  type: CategoryType;
  typeName: string;
  sortOrder: number;
  isActive: boolean;
  parentName?: string;
  children: Category[];
  productCount: number;
}

export enum CategoryType {
  Product = 1,
  TechnicalService = 2,
  Accessory = 3,
  Phone = 4
}

// Product Types
export interface Product {
  id: string;
  categoryId: string;
  categoryName: string;
  name: string;
  barcode: string;
  sku?: string;
  description?: string;
  brand?: string;
  model?: string;
  purchasePrice: number;
  salePrice: number;
  discountedPrice?: number;
  vatRate: number;
  trackSerialNumber: boolean;
  minStockLevel: number;
  isActive: boolean;
  imageUrl?: string;
  totalStock: number;
  availableStock: number;
}

// Sale Types
export interface Sale {
  id: string;
  saleNumber: string;
  branchId: string;
  branchName: string;
  customerId?: string;
  customerName?: string;
  customerPhone?: string;
  userId: string;
  userName: string;
  saleDate: string;
  subTotal: number;
  vatTotal: number;
  discountTotal: number;
  grandTotal: number;
  paymentMethod: PaymentMethod;
  paymentMethodName: string;
  isPaid: boolean;
  notes?: string;
  items: SaleItem[];
}

export interface SaleItem {
  id: string;
  productId: string;
  barcode: string;
  serial: string;
  productName: string;
  unitPrice: number;
  vatRate: number;
  vatAmount: number;
  discountAmount: number;
  totalPrice: number;
}

export enum PaymentMethod {
  Cash = 1,
  CreditCard = 2,
  BankTransfer = 3,
  Installment = 4
}

// Technical Service Types
export interface TechnicalService {
  id: string;
  serviceNumber: string;
  branchId: string;
  branchName: string;
  customerId: string;
  customerName: string;
  customerPhone: string;
  userId: string;
  userName: string;
  deviceBrand: string;
  deviceModel: string;
  deviceIMEI?: string;
  deviceSerialNumber?: string;
  devicePassword?: string;
  faultDescription: string;
  diagnosisNotes?: string;
  status: TechnicalServiceStatus;
  statusName: string;
  receivedAt: string;
  approvedAt?: string;
  completedAt?: string;
  deliveredAt?: string;
  laborCost: number;
  partsCost: number;
  totalCost: number;
  customerApproved: boolean;
  customerApprovedAt?: string;
  paymentMethod?: PaymentMethod;
  paymentMethodName?: string;
  isPaid: boolean;
  paidAt?: string;
  notes?: string;
  parts: TechnicalServicePart[];
  statusHistory: TechnicalServiceStatusHistory[];
}

export interface TechnicalServicePart {
  id: string;
  productId: string;
  productName: string;
  serialNumberId?: string;
  serial?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  isUsed: boolean;
}

export interface TechnicalServiceStatusHistory {
  id: string;
  fromStatus: TechnicalServiceStatus;
  fromStatusName: string;
  toStatus: TechnicalServiceStatus;
  toStatusName: string;
  notes?: string;
  createdAt: string;
  createdByName: string;
}

export enum TechnicalServiceStatus {
  Pending = 1,
  Approved = 2,
  InProgress = 3,
  Completed = 4,
  PaymentReceived = 5,
  Cancelled = 6
}

// Transfer Types
export interface Transfer {
  id: string;
  transferNumber: string;
  fromWarehouseId: string;
  fromWarehouseName: string;
  fromBranchName?: string;
  toWarehouseId: string;
  toWarehouseName: string;
  toBranchName?: string;
  status: TransferStatus;
  statusName: string;
  transferDate: string;
  approvedAt?: string;
  approvedByName?: string;
  deliveredAt?: string;
  deliveredByName?: string;
  notes?: string;
  itemCount: number;
  items: TransferItem[];
}

export interface TransferItem {
  id: string;
  productId: string;
  barcode: string;
  serial: string;
  productName: string;
  brand?: string;
  model?: string;
}

export enum TransferStatus {
  Pending = 1,
  Approved = 2,
  InTransit = 3,
  Delivered = 4,
  Cancelled = 5
}

// Return Types
export interface Return {
  id: string;
  returnNumber: string;
  saleId: string;
  saleNumber: string;
  branchId: string;
  branchName: string;
  customerId?: string;
  customerName?: string;
  customerPhone?: string;
  userId: string;
  userName: string;
  returnDate: string;
  reason: ReturnReason;
  reasonName: string;
  reasonNotes?: string;
  refundAmount: number;
  refundMethod: PaymentMethod;
  refundMethodName: string;
  isRefunded: boolean;
  refundedAt?: string;
  notes?: string;
  items: ReturnItem[];
}

export interface ReturnItem {
  id: string;
  productId: string;
  serial: string;
  productName: string;
  refundPrice: number;
  condition: ReturnCondition;
  conditionName: string;
  conditionNotes?: string;
}

export enum ReturnReason {
  Defective = 1,
  WrongProduct = 2,
  CustomerRegret = 3,
  Other = 4
}

export enum ReturnCondition {
  Good = 1,
  Damaged = 2
}

// Inventory Count Types
export interface InventoryCount {
  id: string;
  countNumber: string;
  branchId: string;
  branchName: string;
  warehouseId: string;
  warehouseName: string;
  userId: string;
  userName: string;
  countDate: string;
  status: InventoryCountStatus;
  statusName: string;
  completedAt?: string;
  approvedAt?: string;
  approvedByName?: string;
  notes?: string;
  totalSystemStock: number;
  totalPhysicalStock: number;
  totalSoldItems: number;
  difference: number;
  items: InventoryCountItem[];
}

export interface InventoryCountItem {
  id: string;
  productId: string;
  serialNumberId?: string;
  serial?: string;
  barcode: string;
  productName: string;
  isInSystem: boolean;
  isPhysicallyPresent: boolean;
  isSold: boolean;
  status: string;
  notes?: string;
}

export enum InventoryCountStatus {
  InProgress = 1,
  Completed = 2,
  Approved = 3
}

// Employee Types
export interface Employee {
  id: string;
  userId: string;
  userName: string;
  email: string;
  phone?: string;
  branchId: string;
  branchName: string;
  tcNo: string;
  birthDate: string;
  hireDate: string;
  terminationDate?: string;
  position?: string;
  baseSalary: number;
  iban?: string;
  emergencyContact?: string;
  emergencyPhone?: string;
  annualLeaveBalance: number;
  isActive: boolean;
  role: UserRole;
  roleName: string;
}

// Dashboard Types
export interface DashboardSummary {
  todaySalesCount: number;
  todaySalesAmount: number;
  todayServiceCount: number;
  todayServiceAmount: number;
  monthSalesCount: number;
  monthSalesAmount: number;
  monthServiceCount: number;
  monthServiceAmount: number;
  pendingServices: number;
  pendingTransfers: number;
  lowStockCount: number;
  cashBalance: number;
}

export interface SaleSummary {
  date: string;
  totalSales: number;
  totalAmount: number;
  cashAmount: number;
  cardAmount: number;
}

export interface UserSaleSummary {
  userId: string;
  userName: string;
  totalSales: number;
  totalAmount: number;
}

// Scan Response
export interface ScanBarcodeResponse {
  found: boolean;
  isAvailable: boolean;
  message?: string;
  serialInfo?: SerialNumberInfo;
}

export interface SerialNumberInfo {
  serialNumberId: string;
  productId: string;
  serial: string;
  imei?: string;
  productName: string;
  barcode: string;
  brand?: string;
  model?: string;
  salePrice: number;
  discountedPrice?: number;
  vatRate: number;
  status: string;
  currentWarehouse?: string;
}

// Customer Types
export interface Customer {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phone: string;
  email?: string;
  address?: string;
  tcNo?: string;
  taxNumber?: string;
  taxOffice?: string;
  totalPurchases: number;
  totalServices: number;
}

// Branch Types
export interface Branch {
  id: string;
  name: string;
  code: string;
  phone?: string;
  email?: string;
  address?: string;
  city?: string;
  district?: string;
  isActive: boolean;
  isCentralBranch: boolean;
  userCount: number;
  stockCount: number;
}

// Warehouse Types
export interface Warehouse {
  id: string;
  branchId?: string;
  branchName?: string;
  name: string;
  code: string;
  isCentral: boolean;
  isActive: boolean;
  totalStock: number;
}
