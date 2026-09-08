<template>
  <div class="page-container">
    <div class="page-header">
      <h1 class="title">Product Catalog</h1>
      <div class="header-actions">
        <ConnectionStatus />
        <KButton @click="showCreateDialog = true" variant="primary">
          Add Product
        </KButton>
      </div>
    </div>

    <div class="filters-section">
      <div class="filter-group">
        <label for="search" class="form-label">Search Products</label>
        <KInput
          v-model="searchTerm"
          placeholder="Search by name, SKU, or category..."
          class="w-full"
        />
      </div>
      
      <div class="filter-group">
        <label for="category" class="form-label">Category</label>
        <KSelect
          v-model="selectedCategory"
          :options="categories"
          placeholder="All Categories"
          class="w-full"
        />
      </div>
      
      <div class="filter-group">
        <label for="status" class="form-label">Status</label>
        <KSelect
          v-model="selectedStatus"
          :options="statusOptions"
          placeholder="All Statuses"
          class="w-full"
        />
      </div>
    </div>

    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>Loading products...</p>
    </div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else class="products-table">
      <table class="products-table">
        <thead>
          <tr>
            <th>Product Name</th>
            <th>SKU</th>
            <th>Category</th>
            <th>Price</th>
            <th>Cost</th>
            <th>Stock</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in filteredProducts" :key="product.id" class="table-row">
            <td>{{ product.name }}</td>
            <td>{{ product.sku }}</td>
            <td>{{ product.category }}</td>
            <td>{{ product.price }}</td>
            <td>{{ product.cost }}</td>
            <td>{{ product.stockQuantity }}</td>
            <td>
              <span 
                class="status-badge" 
                :class="[product.status.toLowerCase().replace(' ', '-')]"
              >
                {{ product.status }}
              </span>
            </td>
            <td class="actions-cell">
              <KButton 
                variant="secondary" 
                size="sm" 
                @click="editProduct(product)"
                class="mr-2"
              >
                Edit
              </KButton>
              <KButton 
                variant="success" 
                size="sm" 
                @click="orderProduct(product.id)"
              >
                Order
              </KButton>
              <KButton 
                variant="danger" 
                size="sm" 
                @confirm="deleteProduct(product.id)"
                @confirmed="confirmDelete(product.id)"
                class="mr-2"
              >
                Delete
              </KButton>
              <KButton 
                variant="outline" 
                size="sm" 
                @click="toggleStockDetails(product)"
              >
                Stock
              </KButton>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="no-results">
      <p>No products found matching your criteria.</p>
    </div>
  </div>

  <!-- Create/Edit Product Dialog -->
  <KDialog v-model:show="showCreateDialog" title="Create New Product" width="500px">
    <div class="dialog-content">
      <div class="form-group">
        <label for="name" class="form-label">Product Name *</label>
        <KInput
          v-model="form.name"
          id="name"
          placeholder="Enter product name"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="sku" class="form-label">SKU *</label>
        <KInput
          v-model="form.sku"
          id="sku"
          placeholder="Enter SKU"
          required
        />
      </div
      
      <div class="form-group">
        <label for="category" class="form-label">Category *</label>
        <KSelect
          v-model="form.category"
          id="category"
          :options="categories"
          placeholder="Select category"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="price" class="form-label">Price (R) *</label>
        <KInput
          type="number"
          v-model.number="form.price"
          id="price"
          placeholder="0.00"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="cost" class="form-label">Cost (R) *</label>
        <KInput
          type="number"
          v-model.number="form.cost"
          id="cost"
          placeholder="0.00"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="stockQuantity" class="form-label">Initial Stock Quantity *</label>
        <KInput
          type="number"
          v-model.number="form.stockQuantity"
          id="stockQuantity"
          placeholder="0"
          min="0"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="reorderLevel" class="form-label">Reorder Level *</label>
        <KInput
          type="number"
          v-model.number="form.reorderLevel"
          id="reorderLevel"
          placeholder="0"
          min="0"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="maxStockLevel" class="form-label">Maximum Stock Level *</label>
        <KInput
          type="number"
          v-model.number="form.maxStockLevel"
          id="maxStockLevel"
          placeholder="0"
          min="0"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="status" class="form-label">Status</label>
        <KSelect
          v-model="form.status"
          id="status"
          :options="statusOptions"
          placeholder="Select status"
        />
      </div>
    </div>
    
    <template #footer>
      <KButton variant="secondary" @click="showCreateDialog = false">
        Cancel
      </KButton>
      <KButton 
        variant="primary" 
        @click="saveProduct"
        :loading="saving"
      >
        {{ form.id ? 'Update' : 'Create' }} Product
      </KButton>
    </template>
  </KDialog>

  <!-- Stock Details Dialog -->
  <KDialog v-model:show="showStockDetails" title="Stock Details" width="400px">
    <div class="dialog-content">
      <p><strong>Product:</strong> {{ selectedProduct.name }}</p>
      <p><strong>SKU:</strong> {{ selectedProduct.sku }}</p>
      <p><strong>Current Stock:</strong> {{ selectedProduct.stockQuantity }}</p>
      <p><strong>Reorder Level:</strong> {{ selectedProduct.reorderLevel }}</p>
      <p><strong>Maximum Stock:</strong> {{ selectedProduct.maxStockLevel }}</p>
      
      <div v-if="selectedProduct.stockQuantity <= selectedProduct.reorderLevel" class="alert alert-warning">
        ⚠️ Stock is at or below reorder level!
      </div>
    </div>
    
    <template #footer>
      <KButton variant="secondary" @click="showStockDetails = false">
        Close
      </KButton>
    </template>
  </KDialog>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton, KInput, KSelect, KDialog } from '@khet360/ui-shared';
import { VendorHubService, Product } from '@/services/vendorHubService';
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { notificationService } from '@/services/notificationService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

const products = ref<Product[]>([]);
const filteredProducts = ref<Product[]>([]);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const searchTerm = ref('');
const selectedCategory = ref<string | null>(null);
const selectedStatus = ref<string | null>(null);
const showCreateDialog = ref(false);
const showStockDetails = ref(false);
const selectedProduct = ref<Product | null>(null);
const saving = ref(false);
const confirmDelete = ref(false);

const form = ref({
  id: '',
  name: '',
  sku: '',
  category: '',
  price: 0,
  cost: 0,
  stockQuantity: 0,
  reorderLevel: 0,
  maxStockLevel: 0,
  status: 'Active'
});

const categories = [
  'Caskets',
  'Urns',
  'Flowers',
  'Memorial Items',
  'Stationery',
  'Transportation',
  'Attire',
  'Other'
];

const statusOptions = ['Active', 'Inactive', 'Discontinued'];

// Computed property for filtered products
computed(() => {
  return products.value.filter(product => {
    const matchesSearch = product.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                         product.sku.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                         product.category.toLowerCase().includes(searchTerm.value.toLowerCase());
    
    const matchesCategory = !selectedCategory.value || product.category === selectedCategory.value;
    const matchesStatus = !selectedStatus.value || product.status === selectedStatus.value;
    
    return matchesSearch && matchesCategory && matchesStatus;
  });
});

// Real-time update handlers
const handleProductUpdated = (updatedProduct: Product) => {
  // Find and update the specific product
  const index = products.value.findIndex(p => p.id === updatedProduct.id);
  if (index !== -1) {
    products.value[index] = { ...products.value[index], ...updatedProduct };
    
    // Show notification for stock updates
    if (updatedProduct.stockQuantity !== undefined) {
      notificationService.addNotification({
        title: 'Stock Update',
        message: `Product "${updatedProduct.name}" stock updated to ${updatedProduct.stockQuantity} units`,
        type: updatedProduct.stockQuantity <= 5 ? 'warning' : 'info'
      });
    }
  }
};

const handleProductCreated = (newProduct: Product) => {
  // Add new product to the list
  products.value = [...products.value, newProduct];
  
  notificationService.addNotification({
    title: 'New Product Added',
    message: `Product "${newProduct.name}" has been added to the catalog`,
    type: 'success'
  });
};

const handleProductDeleted = (productId: string) => {
  // Remove product from the list
  products.value = products.value.filter(p => p.id !== productId);
  
  notificationService.addNotification({
    title: 'Product Removed',
    message: `Product has been removed from the catalog`,
    type: 'warning'
  });
};

const fetchProducts = async () => {
  try {
    loading.value = true;
    error.value = null;
    products.value = await VendorHubService.getProducts();
  } catch (err) {
    error.value = 'Failed to load products. Please try again later.';
    console.error('Error fetching products:', err);
    // Fallback to mock data in case of API failure
    products.value = [
      { id: '1', name: 'Premium Mahogany Casket', sku: 'CAK-001', category: 'Caskets', price: 12500, cost: 8500, stockQuantity: 3, status: 'Active' },
      { id: '2', name: 'Standard Steel Casket', sku: 'CAK-002', category: 'Caskets', price: 8500, cost: 5500, stockQuantity: 8, status: 'Active' },
      { id: '3', name: 'Cremation Urn - Bronze', sku: 'URN-001', category: 'Urns', price: 2500, cost: 1200, stockQuantity: 15, status: 'Active' },
      { id: '4', name: 'Funeral Flowers - Lilly Arrangement', sku: 'FLR-001', category: 'Flowers', price: 1200, cost: 600, stockQuantity: 0, status: 'Active' },
      { id: '5', name: 'Memorial Plaque - Granite', sku: 'MEM-001', category: 'Memorial Items', price: 1800, cost: 900, stockQuantity: 12, status: 'Active' },
    ];
  } finally {
    loading.value = false;
  }
};

const saveProduct = async () => {
  try {
    saving.value = true;
    if (form.value.id) {
      // Update existing product
      await VendorHubService.updateProduct(form.value.id, form.value);
      // Notify via SignalR that we updated a product
      signalRService.send('ProductUpdated', form.value.id, form.value);
    } else {
      // Create new product
      const newProduct = await VendorHubService.createProduct(form.value);
      // Notify via SignalR that we created a product
      signalRService.send('ProductCreated', newProduct);
    }
    
    // Close dialog and refresh
    showCreateDialog.value = false;
    await fetchProducts();
  } catch (err) {
    error.value = 'Failed to save product. Please try again later.';
    console.error('Error saving product:', err);
  } finally {
    saving.value = false;
  }
};

const editProduct = (product: Product) => {
  form.value = { ...product };
  showCreateDialog.value = true;
};

const deleteProduct = (productId: string) => {
  confirmDelete.value = true;
};

const confirmDelete = async (productId: string) => {
  try {
    await VendorHubService.deleteProduct(productId);
    // Notify via SignalR that we deleted a product
    signalRService.send('ProductDeleted', productId);
    
    await fetchProducts();
  } catch (err) {
    error.value = 'Failed to delete product. Please try again later.';
    console.error('Error deleting product:', err);
  }
};

const orderProduct = async (productId: string) => {
  try {
    // In a real implementation, this would create an order
    // For now, we'll just show a confirmation
    const result = await VendorHubService.getProductById(productId);
    alert(`Placing order for ${result.name}...`);
    
    // Notify via SignalR that we ordered a product (decreased stock)
    signalRService.send('ProductOrdered', {
      productId,
      quantity: 1
    });
  } catch (err) {
    error.value = 'Failed to order product. Please try again later.';
    console.error('Error ordering product:', err);
  }
};

const toggleStockDetails = (product: Product) => {
  selectedProduct.value = product;
  showStockDetails.value = true;
};

onMounted(async () => {
  await fetchProducts();
  
  // Set up SignalR listeners for real-time updates
  const productCleanup = VendorHubService.subscribeToProductUpdates(
    handleProductCreated,
    handleProductUpdated,
    handleProductDeleted
  );
  
  // Start SignalR connection
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    notificationService.addNotification({
      title: 'Connection Issue',
      message: 'Unable to connect to real-time updates. Some features may not be live.',
      type: 'warning'
    });
  });
  
  // Cleanup on unmount
  onBeforeUnmount(() => {
    if (productCleanup) productCleanup();
    signalRService.stop();
  });
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.page-container {
  padding: 2rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.filters-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 600;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: var(--khet-text-muted);
}

.spinner {
  width: 32px;
  height: 32px;
  border: 3px solid var(--khet-border);
  border-top-color: var(--khet-primary);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-message {
  padding: 1rem 1.5rem;
  background-color: #f8d7da;
  color: #721c24;
  border-radius: 8px;
  border: 1px solid #f5c6cb;
}

.products-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.products-table th,
.products-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #f3f4f6;
}

.products-table th {
  background-color: #f8fafc;
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.table-row {
  transition: background-color 0.2s;
}

.table-row:hover {
  background-color: #f8fafc;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-active {
  background-color: #dcfce7;
  color: #16a34a;
}

.status-inactive {
  background-color: #fed7d7;
  color: #dc2626;
}

.status-discontinued {
  background-color: #fef3c7;
  color: #d97706;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
}

.alert {
  padding: 1rem;
  border-radius: 8px;
  margin: 1rem 0;
}

.alert-warning {
  background-color: #fffbeb;
  border: 1px solid #fef3c7;
  color: #92400e;
}

.no-results {
  text-align: center;
  padding: 3rem;
  color: var(--khet-text-muted);
  font-style: italic;
}

/* Dialog styles */
.dialog-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>