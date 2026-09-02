<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Repatriation</h1>
      <router-link to="/ops" class="back-link">← Hub</router-link>
    </header>

    <div class="trip-list">
      <div v-if="trips.length === 0" class="empty-state">
        <span class="emoji">🗺️</span>
        <p>No active repatriation trips.</p>
      </div>

      <div
        v-for="trip in trips"
        :key="trip.id"
        class="trip-card"
      >
        <div class="trip-status-bar" :class="trip.status.toLowerCase()">
          {{ trip.status }}
        </div>

        <div class="trip-header">
          <span class="case-id">{{ trip.caseId }}</span>
          <span class="vehicle">{{ trip.vehicleId }}</span>
        </div>

        <div class="route">
          <div class="route-step">
            <span class="dot"></span>
            <span class="label">Origin:</span>
            <span class="value">{{ trip.origin }}</span>
          </div>
          <div class="route-connector"></div>
          <div class="route-step">
            <span class="dot"></span>
            <span class="label">Dest:</span>
            <span class="value">{{ trip.destination }}</span>
          </div>
        </div>

        <div class="trip-actions">
          <button
            v-if="trip.status === 'Scheduled'"
            @click="updateTripStatus(trip, 'Dispatched')"
            class="btn-action"
          >
            Confirm Dispatch
          </button>
          <button
            v-if="trip.status === 'Dispatched'"
            @click="updateTripStatus(trip, 'InTransit')"
            class="btn-action"
          >
            Confirm Collection
          </button>
          <button
            v-if="trip.status === 'InTransit'"
            @click="updateTripStatus(trip, 'Completed')"
            class="btn-action btn-complete"
          >
            Confirm Arrival
          </button>
          <span v-if="trip.status === 'Completed'" class="status-badge">
            Delivered
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { db } from '@/db/schema';
import { syncEngine } from '@/sync/sync-engine';
import { LocalTrip } from '@/db/schema';

const trips = ref<LocalTrip[]>([]);

async function loadTrips() {
  trips.value = await db.trips.where('status').anyOf(['Scheduled', 'Dispatched', 'InTransit']).toArray();
}

async function updateTripStatus(trip: LocalTrip, newStatus: LocalTrip['status']) {
  await syncEngine.executeCommand({
    entityType: 'Trip',
    entityId: trip.id,
    action: 'UPDATE',
    payload: { status: newStatus },
  });
  await loadTrips();
}

onMounted(async () => {
  await loadTrips();
  setInterval(loadTrips, 5000);
});
</script>

<style scoped>
.page {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.title {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.back-link {
  font-size: 0.85rem;
  color: var(--khet-primary);
  text-decoration: none;
  font-weight: 600;
}

.trip-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.trip-card {
  background: var(--khet-surface);
  border-radius: 20px;
  overflow: hidden;
  border: 1px solid var(--khet-border);
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
}

.trip-status-bar {
  padding: 6px 12px;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  text-align: center;
  color: white;
}

.trip-status-bar.scheduled { background: #9ca3af; }
.trip-status-bar.dispatched { background: #3b82f6; }
.trip-status-bar.intransit { background: #f59e0b; }
.trip-status-bar.completed { background: #10b981; }

.trip-header {
  padding: 1rem 1.25rem 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.case-id {
  font-weight: 700;
  color: var(--khet-text-main);
}

.vehicle {
  font-size: 0.75rem;
  color: var(--khet-text-muted);
  background: rgba(0,0,0,0.05);
  padding: 2px 6px;
  border-radius: 4px;
}

.route {
  padding: 1rem 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.route-step {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.9rem;
}

.dot {
  width: 8px;
  height: 8px;
  background: var(--khet-primary);
  border-radius: 50%;
}

.label {
  font-weight: 600;
  color: var(--khet-text-muted);
  width: 60px;
}

.value {
  font-weight: 500;
}

.route-connector {
  width: 2px;
  height: 1rem;
  background: var(--khet-border);
  margin-left: 3px;
}

.trip-actions {
  padding: 1rem 1.25rem 1.25rem;
  border-top: 1px solid var(--khet-border);
  display: flex;
  justify-content: center;
}

.btn-action {
  width: 100%;
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 12px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
}

.btn-complete {
  background: #10b981;
}

.status-badge {
  font-size: 0.85rem;
  font-weight: 600;
  color: #10b981;
}

.empty-state {
  text-align: center;
  padding: 3rem 0;
  color: var(--khet-text-muted);
}

.empty-state .emoji {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
}
</style>
