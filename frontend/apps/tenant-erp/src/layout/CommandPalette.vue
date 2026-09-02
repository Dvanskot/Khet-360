<template>
  <div class="palette-overlay" @click.self="emit('close')">
    <div class="palette-container">
      <div class="palette-header">
        <div class="search-input-wrapper">
          <span class="search-icon">🔍</span>
          <input
            ref="inputRef"
            v-model="query"
            placeholder="Type a command or search for a record..."
            @keydown.esc="emit('close')"
            autofocus
          />
        </div>
      </div>

      <div class="palette-results">
        <div v-if="filteredItems.length === 0" class="empty-state">
          No results found for "{{ query }}"
        </div>
        <div v-else class="results-list">
          <div v-for="item in filteredItems" :key="item.id" class="result-item" @click="executeItem(item)">
            <div class="item-left">
              <span class="item-icon">{{ item.icon }}</span>
              <div class="item-info">
                <span class="item-name">{{ item.name }}</span>
                <span class="item-desc">{{ item.description }}</span>
              </div>
            </div>
            <span class="item-shortcut">{{ item.shortcut }}</span>
          </div>
        </div>
      </div>

      <div class="palette-footer">
        <div class="footer-item">
          <span class="footer-key">↵</span>
          <span class="footer-text">Enter to select</span>
        </div>
        <div class="footer-item">
          <span class="footer-key">esc</span>
          <span class="footer-text">Escape to close</span>
        </div>
      </div>
    </template>

    <script setup lang="ts">
    import { ref, computed, nextTick, onMounted } from 'vue';
    import { useRouter } from 'vue-router';

    const emit = defineEmits(['close']);
    const router = useRouter();
    const query = ref('');
    const inputRef = ref<HTMLInputElement | null>(null);

    const allItems = [
      { id: 'my-work', name: 'My Work', description: 'View your assigned tasks', icon: '📅', shortcut: 'G W', action: () => router.push('/') },
      { id: 'team-queue', name: 'Team Queue', description: 'Manage unassigned work', icon: '👥', shortcut: 'G Q', action: () => router.push('/team-queue') },
      { id: 'exceptions', name: 'Exceptions', description: 'Review SLA breaches', icon: '⚠️', shortcut: 'G E', action: () => router.push('/exceptions') },
      { id: 'crm', name: 'CRM Dashboard', description: 'Customer relationship management', icon: '🤝', shortcut: 'G C', action: () => router.push('/crm') },
      { id: 'ops', name: 'Funeral Operations', description: 'Manage cases and services', icon: '⚰️', shortcut: 'G O', action: () => router.push('/operations') },
      { id: 'finance', name: 'Finance', description: 'Ledger and accounting', icon: '💰', shortcut: 'G F', action: () => router.push('/finance') },
      { id: 'hr', name: 'HR & Payroll', description: 'Employees and payroll', icon: '👥', shortcut: 'G H', action: () => router.push('/hr') },
      { id: 'create-case', name: 'Create Case', description: 'Open a new funeral case', icon: '➕', shortcut: 'C C', action: () => alert('Opening Create Case Wizard...') },
      { id: 'create-lead', name: 'Create Lead', description: 'Add a new potential customer', icon: '👤', shortcut: 'C L', action: () => alert('Opening Create Lead Form...') },
    ];

    const filteredItems = computed(() => {
      if (!query.value) return allItems;
      const q = query.value.toLowerCase();
      return allItems.filter(i =>
        i.name.toLowerCase().includes(q) ||
        i.description.toLowerCase().includes(q)
      );
    });

    const executeItem = (item: typeof allItems[0]) => {
      item.action();
      emit('close');
    };

    onMounted(async () => {
      await nextTick();
      inputRef.value?.focus();
    });
    </script>

    <style scoped>
    .palette-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: flex-start;
      justify-content: center;
      padding-top: 10vh;
      z-index: 1000;
      backdrop-filter: blur(4px);
    }

    .palette-container {
      width: 600px;
      background-color: var(--khet-surface);
      border-radius: 12px;
      box-shadow: 0 20px 50px rgba(0, 0, 0, 0.3);
      overflow: hidden;
      border: 1px solid var(--khet-border);
    }

    .palette-header {
      padding: 1rem;
      border-bottom: 1px solid var(--khet-border);
    }

    .search-input-wrapper {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.5rem 0;
    }

    .search-icon {
      font-size: 1.2rem;
      color: var(--khet-text-muted);
    }

    input {
      width: 100%;
      border: none;
      outline: none;
      font-size: 1.1rem;
      font-family: inherit;
      color: var(--khet-text-main);
    }

    .palette-results {
      max-height: 400px;
      overflow-y: auto;
      padding: 0.5rem;
    }

    .empty-state {
      padding: 2rem;
      text-align: center;
      color: var(--khet-text-muted);
    }

    .results-list {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .result-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.75rem 1rem;
      border-radius: 8px;
      cursor: pointer;
      transition: background 0.1s;
    }

    .result-item:hover {
      background-color: var(--khet-primary-light);
    }

    .item-left {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .item-icon {
      font-size: 1.2rem;
      width: 24px;
      text-align: center;
    }

    .item-info {
      display: flex;
      flex-direction: column;
    }

    .item-name {
      font-size: 0.9rem;
      font-weight: 600;
      color: var(--khet-text-main);
    }

    .item-desc {
      font-size: 0.75rem;
      color: var(--khet-text-muted);
    }

    .item-shortcut {
      font-size: 0.7rem;
      background-color: var(--khet-surface-alt);
      padding: 2px 6px;
      border-radius: 4px;
      color: var(--khet-text-muted);
      border: 1px solid var(--khet-border);
    }

    .palette-footer {
      padding: 0.75rem 1rem;
      background-color: var(--khet-surface-alt);
      border-top: 1px solid var(--khet-border);
      display: flex;
      gap: 1.5rem;
    }

    .footer-item {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.75rem;
      color: var(--khet-text-muted);
    }

    .footer-key {
      background-color: white;
      border: 1px solid var(--khet-border);
      padding: 1px 4px;
      border-radius: 3px;
      font-family: monospace;
      font-weight: 700;
    }
    </style>
    </script>
