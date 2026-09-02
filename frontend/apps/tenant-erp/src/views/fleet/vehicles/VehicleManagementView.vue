<template>
  <div class="view-container">
    <div class="view-header">
      <div class="header-left">
        <h1>Vehicle Register</h1>
        <p>Manage your fleet of hearses, limousines and support vehicles.</p>
      </div>
      <div class="header-actions">
        <KButton variant="primary" @click="openAddVehicleModal">➕ Add Vehicle</KButton>
      </div>
    </div>

    <div class="filters-bar">
      <div class="filter-item">
        <span class="filter-label">Type:</span>
        <select v-model="filters.type">
          <option value="All">All Types</option>
          <option value="Hearse">Hearse</option>
          <option value="Limousine">Limousine</option>
          <option value="Support Vehicle">Support Vehicle</option>
        </select>
      </div>
      <div class="filter-item search">
        <KInput v-model="filters.search" placeholder="Search by registration..." />
      </div>
    </div>

    <div class="vehicle-grid">
      <KCard v-for="vehicle in filteredVehicles" :key="vehicle.id" elevation="sm" class="vehicle-card">
        <div class="vehicle-card-content">
          <div class="vehicle-header">
            <div class="reg-number">{{ vehicle.registration }}</div>
            <span class="status-pill" :class="vehicle.status">{{ vehicle.status }}</span>
          </div>
          <div class="vehicle-body">
            <div class="detail-row">
              <span class="label">Type:</span>
              <span class="value">{{ vehicle.type }}</span>
            </div>
            <div class="detail-row">
              <span class="label">Mileage:</span>
              <span class="value">{{ vehicle.mileage }} km</span>
            </div>
            <div class="detail-row">
              <span class="label">Last Service:</span>
              <span class="value">{{ vehicle.lastInspectionDate }}</span>
            </div>
          </div>
          <div class="vehicle-footer">
            <KButton variant="secondary" size="sm" @click="editVehicle(vehicle.id)">Edit</KButton>
            <KButton variant="primary" size="sm" @click="scheduleService(vehicle.id)">Service</KButton>
          </div>
        </div>
      </KCard>
    </div>
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton, KInput, KCard } from '@khet360/ui-shared';
  import { Vehicle } from '@/components/fleet/types';

  const filters = ref({
    type: 'All',
    search: '',
  });

  const vehicles = ref<Vehicle[]>([
    { id: 'V1', registration: 'CA 123-456', type: 'Hearse', status: 'Available', branchId: 'B1', lastInspectionDate: '2026-07-15', mileage: 12500 },
    { id: 'V2', registration: 'CA 987-654', type: 'Hearse', status: 'InTransit', branchId: 'B1', lastInspectionDate: '2026-08-01', mileage: 45000 },
    { id: 'V3', registration: 'CP 111-222', type: 'Limousine', status: 'Available', branchId: 'B2', lastInspectionDate: '2026-06-10', mileage: 8200 },
    { id: 'V4', registration: 'CP 333-444', type: 'Support Vehicle', status: 'Maintenance', branchId: 'B1', lastInspectionDate: '2026-09-01', mileage: 110000 },
  ]);

  const filteredVehicles = computed(() => {
    return vehicles.value.filter(v => {
      const typeMatch = filters.value.type === 'All' || v.type === filters.value.type;
      const searchMatch = !filters.value.search || v.registration.toLowerCase().includes(filters.value.search.toLowerCase());
      return typeMatch && searchMatch;
    });
  });

  const openAddVehicleModal = () => alert('Opening Add Vehicle form...');
  const editVehicle = (id: string) => alert(`Editing vehicle ${id}...`);
  const scheduleService = (id: string) => alert(`Scheduling service for vehicle ${id}...`);
  </script>

  <style scoped>
  .view-container {
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

  .filters-bar {
    display: flex;
    gap: 1.5rem;
    align-items: center;
    background-color: white;
    padding: 1rem;
    border-radius: var(--khet-radius-md);
    border: 1px solid var(--khet-border);
  }

  .filter-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.9rem;
  }

  .filter-label {
    color: var(--khet-text-muted);
    font-weight: 500;
  }

  .filter-item select {
    padding: 0.4rem;
    border: 1px solid var(--khet-border);
    border-radius: 4px;
    font-family: inherit;
  }

  .filter-item.search {
    margin-left: auto;
    flex: 1;
    max-width: 300px;
  }

  .vehicle-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 1.5rem;
  }

  .vehicle-card {
    padding: 1rem;
  }

  .vehicle-card-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .vehicle-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .reg-number {
    font-weight: 800;
    font-size: 1.1rem;
    color: var(--khet-text-main);
  }

  .status-pill {
    font-size: 0.7rem;
    padding: 2px 8px;
    border-radius: 12px;
    font-weight: 600;
    text-transform: uppercase;
  }

  .status-pill.Available { background-color: #d4edda; color: #155724; }
  .status-pill.InTransit { background-color: #d1ecf1; color: #0c5460; }
  .status-pill.Maintenance { background-color: #f8d7da; color: #721c24; }

  .vehicle-body {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    padding: 1rem 0;
    border-top: 1px solid var(--khet-border);
    border-bottom: 1px solid var(--khet-border);
  }

  .detail-row {
    display: flex;
    justify-content: space-between;
    font-size: 0.9rem;
  }

  .label {
    color: var(--khet-text-muted);
  }

  .value {
    font-weight: 500;
    color: var(--khet-text-main);
  }

  .vehicle-footer {
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
    margin-top: 0.5rem;
  }
  </style>
</template>
