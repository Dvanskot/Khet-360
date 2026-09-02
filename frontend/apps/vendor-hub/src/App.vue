<template>
  <div class="vendor-portal">
    <nav class="vendor-nav">
      <div class="nav-logo">
        <img src="/images/khet360_logo.png" alt="Khet-360 Logo" />
        <span class="hub-name">Vendor Collaboration Hub</span>
      </div>
      <div class="vendor-profile">
        <span class="vendor-name">Oak Caskets Ltd</span>
        <KButton variant="secondary" size="sm">Logout</KButton>
      </div>
    </nav>

    <main class="vendor-main">
      <div class="dashboard-grid">
        <!-- Active Orders -->
        <div class="orders-section">
          <div class="section-header">
            <h2>Active Purchase Orders</h2>
            <span class="count-badge">3 Pending</span>
          </div>
          <div class="orders-list">
            <KCard v-for="order in orders" :key="order.id" elevation="sm" class="order-card">
              <div class="order-header">
                <span class="po-number">{{ order.poNumber }}</span>
                <span class="order-status" :class="order.status">{{ order.status }}</span>
              </div>
              <div class="order-body">
                <div class="item-summary">
                  <span class="item-name">{{ order.mainItem }}</span>
                  <span class="item-qty">Qty: {{ order.quantity }}</span>
                </div>
                <div class="delivery-info">
                  <span class="label">Delivery To:</span>
                  <span class="value">{{ order.deliveryLocation }}</span>
                </div>
                <div class="delivery-date">
                  <span class="label">Due Date:</span>
                  <span class="value">{{ order.dueDate }}</span>
                </div>
              </div>
              <div class="order-footer">
                <KButton variant="primary" size="sm" @click="confirmDelivery(order.id)">Confirm Delivery</KButton>
              </div>
            </KCard>
          </div>
        </div>

        <!-- Invoicing Section -->
        <div class="invoicing-section">
          <div class="section-header">
            <h2>Billing & Invoices</h2>
          </div>
          <div class="invoice-card">
            <div class="invoice-header">
              <h3>Submit Invoice</h3>
              <span class="hint">Link to a valid Purchase Order</span>
            </div>
            <div class="invoice-form">
              <div class="form-group">
                <label>Select PO Number</label>
                <select v-model="selectedPo">
                  <option v-for="order in orders" :key="order.id" :value="order.poNumber">
                    {{ order.poNumber }} - {{ order.mainItem }}
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label>Total Amount (ZAR)</label>
                <KInput v-model="invoiceAmount" type="number" placeholder="0.00" />
              </div>
              <div class="form-group">
                <label>Invoice PDF</label>
                <div class="file-dropzone">
                  <span class="drop-icon">📤</span>
                  <span class="drop-text">Click or drag invoice PDF here</span>
                </div>
              </div>
              <KButton variant="primary" @click="submitInvoice">Submit Invoice for Approval</KButton>
            </div>
          </div>
        </div>
      </div>
    </main>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { KButton, KCard, KInput } from '@khet360/ui-shared';

  const selectedPo = ref('');
  const invoiceAmount = ref('');

  const orders = ref([
    { id: 'O1', poNumber: 'PO-2026-881', mainItem: 'Premium Mahogany Casket', quantity: 2, deliveryLocation: 'Cape Town Central', dueDate: '2026-09-05', status: 'Approved' },
    { id: 'O2', poNumber: 'PO-2026-885', mainItem: 'Standard Oak Casket', quantity: 5, deliveryLocation: 'Stellenbosch', dueDate: '2026-09-08', status: 'Submitted' },
    { id: 'O3', poNumber: 'PO-2026-890', mainItem: 'Ceramic Urns (White)', quantity: 10, deliveryLocation: 'Paarl', dueDate: '2026-09-10', status: 'Approved' },
  ]);

  const confirmDelivery = (id: string) => {
    alert(`Delivery for order ${id} confirmed. Notifying Tenant ERP...`);
  };

  const submitInvoice = () => {
    alert(`Invoice for ${selectedPo.value} submitted for ${invoiceAmount.value} ZAR.`);
  };
  </script>

  <style scoped>
  .vendor-portal {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
    background-color: #f9fafb;
  }

  .vendor-nav {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 2rem;
    background-color: white;
    border-bottom: 1px solid var(--khet-border);
  }

  .nav-logo {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .nav-logo img {
    height: 32px;
  }

  .hub-name {
    font-weight: 800;
    font-size: 1.1rem;
    color: var(--khet-text-main);
  }

  .vendor-profile {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .vendor-name {
    font-weight: 600;
    font-size: 0.9rem;
    color: var(--khet-text-muted);
  }

  .vendor-main {
    flex: 1;
    padding: 2rem;
    max-width: 1200px;
    margin: 0 auto;
    width: 100%;
  }

  .dashboard-grid {
    display: grid;
    grid-template-columns: 1fr 400px;
    gap: 2rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
  }

  .section-header h2 {
    font-size: 1.4rem;
    margin: 0;
    color: var(--khet-text-main);
  }

  .count-badge {
    background-color: var(--khet-primary-light);
    color: var(--khet-primary);
    padding: 4px 12px;
    border-radius: 12px;
    font-size: 0.8rem;
    font-weight: 700;
  }

  .orders-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .order-card {
    padding: 1.5rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .order-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .po-number {
    font-weight: 800;
    font-size: 1rem;
    color: var(--khet-text-main);
  }

  .order-status {
    font-size: 0.7rem;
    padding: 2px 8px;
    border-radius: 12px;
    font-weight: 700;
    text-transform: uppercase;
  }

  .order-status.Approved { background-color: #d4edda; color: #155724; }
  .order-status.Submitted { background-color: #fff3cd; color: #856404; }

  .item-summary {
    display: flex;
    justify-content: space-between;
    font-weight: 600;
    font-size: 1.1rem;
  }

  .order-body {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    font-size: 0.9rem;
    color: var(--khet-text-muted);
  }

  .delivery-info, .delivery-date {
    display: flex;
    justify-content: space-between;
  }

  .label {
    font-weight: 500;
  }

  .order-footer {
    display: flex;
    justify-content: flex-end;
    margin-top: 0.5rem;
  }

  .invoice-card {
    background-color: white;
    border-radius: var(--khet-radius-md);
    padding: 2rem;
    border: 1px solid var(--khet-border);
    box-shadow: 0 4px 12px rgba(0,0,0,0.05);
  }

  .invoice-header {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    margin-bottom: 1.5rem;
  }

  .invoice-header h3 {
    font-size: 1.3rem;
    margin: 0;
  }

  .invoice-header .hint {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .invoice-form {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .form-group label {
    font-size: 0.9rem;
    font-weight: 600;
  }

  .file-dropzone {
    border: 2px dashed var(--khet-border);
    border-radius: var(--khet-radius-md);
    padding: 2rem;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    cursor: pointer;
    transition: all 0.2s;
  }

  .file-dropzone:hover {
    border-color: var(--khet-primary);
    background-color: var(--khet-primary-light);
  }

  .drop-icon {
    font-size: 2rem;
  }

  .drop-text {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }
  </style>
</template>
