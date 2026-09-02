export type StockStatus = 'InStock' | 'LowStock' | 'OutOfStock';
export type TransferStatus = 'Draft' | 'Requested' | 'Approved' | 'Picked' | 'InTransit' | 'Received' | 'Reconciled';

export interface InventoryItem {
  id: string;
  sku: string;
  name: string;
  category: string;
  unit: string;
  quantityOnHand: number;
  reservedQuantity: number;
  availableQuantity: number;
  minLevel: number;
  maxLevel: number;
  unitCost: number;
  unitPrice: number;
  status: StockStatus;
}

export interface StockTransfer {
  id: string;
  transferNumber: string;
  sourceBranchId: string;
  sourceBranchName: string;
  destinationBranchId: string;
  destinationBranchName: string;
  status: TransferStatus;
  items: TransferItem[];
  requestedDate: string;
  receivedDate?: string;
}

export interface TransferItem {
  itemId: string;
  itemName: string;
  quantity: number;
  unit: string;
}

export interface PurchaseOrder {
  id: string;
  poNumber: string;
  supplierId: string;
  supplierName: string;
  status: 'Draft' | 'Submitted' | 'Approved' | 'PartiallyReceived' | 'Received' | 'Closed';
  totalAmount: number;
  orderDate: string;
  expectedDeliveryDate: string;
}
