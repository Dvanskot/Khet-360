<template>
  <div class="inventory-dashboard">
    <div class="view-header">
      <div class="header-left">
        <h1>Inventory & Procurement</h1>
        <p>Manage stock levels, branch transfers and supplier orders.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="$router.push('/inventory/transfers')">Manage Transfers</KButton>
        <KButton variant="primary" @click="$router.push('/inventory/procurement')">New Purchase Order</KButton>
      </div>
    </div>

    <div class="alerts-bar">
      <div class="alert-item critical">
        <span class="alert-icon">⚠️</span>
        <span class="alert-text"><strong>Low Stock Alert:</strong> Mahogany Caskets are below minimum levels in Cape Town Central.</span>
        <KButton variant="secondary" size="sm" @click="$router.push('/inventory/procurement')">Order Now</KButton>
      </div>
    </div>

    <div class="inventory-grid">
      <!-- Stock Overview -->
      <div class="stock-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Current Stock Levels</strong>
              <KButton variant="secondary" size="sm" @click="$router.push('/inventory/stock')">Full Inventory</KButton>
            </div>
          </template>
          <div class="stock-list">
            <div v-for="item in stockItems" :key="item.id" class="stock-row">
              <div class="item-info">
                <span class="item-name">{{ item.name }}</span>
                <span class="item-sku">{{ item.sku }}</span>
              </div>
              <div class="stock-status-cell">
                <span class="status-pill" :class="item.status">{{ item.status }}</span>
              </div>
              <div class="stock-qty">
                <span class="available">{{ item.availableQuantity }}</span>
                <span class="total">/ {{ item.quantityOnHand }}</span>
              </div>
              <div class="stock-action">
                <KButton variant="secondary" size="sm">Adjust</KButton>
              </div>
            </div>
          </div>
        </KCard>
      </div>

      <!-- Recent Transfers -->
      <div class="transfers-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Active Stock Transfers</strong>
              <KButton variant="secondary" size="sm" @click="$router.push('/inventory/transfers')">View All</KButton>
            </div>
          </template>
          <div class="transfer-list">
            <div v-for="tx in activeTransfers" :key="tx.id" class="transfer-row">
              <div class="tx-info">
                <span class="tx-number">{{ tx.transferNumber }}</span>
                <span class="tx-route">{{ tx.sourceBranchName }} → {{ tx.destinationBranchName }}</span>
              </div>
              <div class="tx-status">
                <span class="pill" :class="tx.status">{{ tx.status }}</span>
              </div>
              <div class="tx-action">
                <KButton variant="secondary" size="sm">Track</KButton>
              </div>
            </div>
          </div>
        </KCard>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { KButton, KCard } from '@khet360/ui-shared';
  import { InventoryItem, StockTransfer } from '@/components/inventory/types';

  const stockItems = ref<InventoryItem[]>([
    { id: 'I1', sku: 'CASK-OAK-STD', name: 'Oak Standard Casket', category: 'Caskets', unit: 'pcs', quantityOnHand: 12, reservedQuantity: 3, availableQuantity: 9, minLevel: 5, maxLevel: 20, unitCost: 2000, unitPrice: 5000, status: 'InStock' },
    { id: 'I2', sku: 'CASK-MAH-PRE', name: 'Mahogany Premium Casket', category: 'Caskets', unit: 'pcs', quantityOnHand: 3, reservedQuantity: 2, availableQuantity: 1, minLevel: 5, maxLevel: 10, unitCost: 5000, unitPrice: 12000, status: 'LowStock' },
    { id: 'I3', sku: 'URN-CER-WHT', name: 'Ceramic Urn White', category: 'Urns', unit: 'pcs', quantityOnHand: 0, reservedQuantity: 0, availableQuantity: 0, minLevel: 5, maxLevel: 20, unitCost: 500, unitPrice: 1500, status: 'OutOfStock' },
    { id: 'I4', sku: 'TENT-XL-WHT', name: 'White Event Tent XL', category: 'Equipment', unit: 'pcs', quantityOnHand: 5, reservedQuantity: 1, availableQuantity: 4, minLevel: 2, maxLevel: 5, unitCost: 15000, unitPrice: 2000, status: 'InStock' },
  ]);

  const activeTransfers = ref<StockTransfer[]>([
    { id: 'T1', transferNumber: 'TR-8812', sourceBranchName: 'Cape Town Central', destinationBranchName: 'Stellenbosch', status: 'InTransit', items: [], requestedDate: '2026-09-01', receivedDate: undefined },
    { id: 'T2', transferNumber: 'TR-8815', sourceBranchName: 'Paarl', destinationBranchName: 'Cape Town Central', status: 'Requested', items: [], requestedDate: '2026-09-02', receivedDate: undefined },
  ]);
  </script>

  <style scoped>
  .inventory-dashboard {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .view-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .header-left h1 {
    font-size: 2rem;
    margin: 0 0 0.5rem 0;
  }

  .header-left p {
    color: var(--khet-text-muted);
    font-size: 1.1rem;
  }

  .alerts-bar {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .alert-item {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    border-radius: var(--khet-radius-md);
    border-left: 5px solid;
  }

  .alert-item.critical {
    background-color: #fdf2f2;
    border-color: #c0392b;
    color: #c0392b;
  }

  .alert-icon {
    font-size: 1.2rem;
  }

  .alert-text {
    flex: 1;
    font-size: 0.9rem;
  }

  .inventory-grid {
    display: grid;
    grid-template-columns: 1.2fr 0.8fr;
    gap: 2rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .stock-list, .transfer-list {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding: 1rem 0;
  }

  .stock-row, .transfer-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
  }

  .item-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    flex: 1;
  }

  .item-name {
    font-weight: 600;
  }

  .item-sku {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
    font-family: monospace;
  }

  .stock-status-cell {
    margin: 0 1.5rem;
  }

  .status-pill {
    font-size: 0.7rem;
    padding: 2px 8px;
    border-radius: 12px;
    font-weight: 600;
    text-transform: uppercase;
  }

  .status-pill.InStock { background-color: #d4edda; color: #155724; }
  .status-pill.LowStock { background-color: #fff3cd; color: #856404; }
  .status-pill.OutOfStock { background-color: #f8d7da; color: #721c24; }

  .stock-qty {
    display: flex;
    align-items: baseline;
    gap: 0.25rem;
    font-weight: 700;
    margin-right: 1.5rem;
  }

  .available {
    font-size: 1rem;
    color: var(--khet-text-main);
  }

  .total {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .tx-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    flex: 1;
  }

  .tx-number {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .tx-route {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .tx-status {
    margin: 0 1.5rem;
  }

  .tx-status .pill {
    font-size: 0.7rem;
    padding: 2px 8px;
    border-radius: 12px;
    font-weight: 600;
    background-color: var(--khet-surface-alt);
  }

  .tx-status .pill.InTransit { background-color: #d1ecf1; color: #0c5460; }
  .tx-status .pill.Requested { background-color: #fff3cd; color: #856404; }
  </style>
</template>
