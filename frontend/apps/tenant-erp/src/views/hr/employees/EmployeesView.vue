<template>
  <div class="employees-view">
    <div class="view-header">
      <div class="header-left">
        <h1>Employee Directory</h1>
        <p>Manage staff profiles, reporting lines and contracts.</p>
      </div>
      <div class="header-actions">
        <KButton variant="primary" @click="openCreateEmpModal">➕ Add Employee</KButton>
      </div>
    </div>

    <div class="filters-bar">
      <div class="filter-item">
        <span class="filter-label">Department:</span>
        <select v-model="filters.dept">
          <option value="All">All Departments</option>
          <option value="Operations">Operations</option>
          <option value="Finance">Finance</option>
          <option value="HR">HR</option>
        </select>
      </div>
      <div class="filter-item search">
        <KInput v-model="filters.search" placeholder="Search employees..." />
      </div>
    </div>

    <div class="employees-grid">
      <KCard v-for="emp in filteredEmployees" :key="emp.id" elevation="sm" class="emp-card">
        <div class="emp-card-content">
          <div class="emp-avatar">{{ emp.firstName[0] }}{{ emp.lastName[0] }}</div>
          <div class="emp-details">
            <h4 class="emp-name">{{ emp.firstName }} {{ emp.lastName }}</h4>
            <span class="emp-position">{{ emp.position }}</span>
            <span class="emp-status" :class="emp.status">{{ emp.status }}</span>
          </div>
          <div class="emp-actions">
            <KButton variant="secondary" size="sm" @click="viewProfile(emp.id)">Profile</KButton>
          </div>
        </div>
      </KCard>
    </div>
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton, KInput, KCard } from '@khet360/ui-shared';

  const filters = ref({
    dept: 'All',
    search: '',
  });

  const employees = ref([
    { id: 'E1', firstName: 'Sarah', lastName: 'Jenkins', position: 'Branch Manager', dept: 'Operations', status: 'Full-Time', email: 'sarah@company.co.za' },
    { id: 'E2', firstName: 'Mike', lastName: 'Ross', position: 'Accountant', dept: 'Finance', status: 'Full-Time', email: 'mike@company.co.za' },
    { id: 'E3', firstName: 'Lindiwe', lastName: 'Khoza', position: 'Fleet Supervisor', dept: 'Operations', status: 'Full-Time', email: 'lindiwe@company.co.za' },
    { id: 'E4', firstName: 'Peter', lastName: 'Pan', position: 'Driver', dept: 'Operations', status: 'Contract', email: 'peter@company.co.za' },
  ]);

  const filteredEmployees = computed(() => {
    return employees.value.filter(e => {
      const deptMatch = filters.value.dept === 'All' || e.dept === filters.value.dept;
      const searchMatch = !filters.value.search ||
        `${e.firstName} ${e.lastName}`.toLowerCase().includes(filters.value.search.toLowerCase()) ||
        e.position.toLowerCase().includes(filters.value.search.toLowerCase());
      return deptMatch && searchMatch;
    });
  });

  const openCreateEmpModal = () => alert('Opening Add Employee form...');
  const viewProfile = (id: string) => alert(`Opening profile for Employee ${id}...`);
  </script>

  <style scoped>
  .employees-view {
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

  .employees-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 1.5rem;
  }

  .emp-card {
    padding: 1rem;
  }

  .emp-card-content {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .emp-avatar {
    width: 48px;
    height: 48px;
    background-color: var(--khet-primary);
    color: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    font-size: 1.1rem;
  }

  .emp-details {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .emp-name {
    font-weight: 600;
    font-size: 0.95rem;
    margin: 0;
  }

  .emp-position {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .emp-status {
    font-size: 0.7rem;
    font-weight: 700;
    text-transform: uppercase;
  }

  .emp-status.Full-Time { color: #2ecc71; }
  .emp-status.Contract { color: #e67e22; }

  .emp-actions {
    display: flex;
    align-items: center;
  }
  </style>
</template>
