import { db } from './db/schema';

export async function seedMobileDB() {
  const count = await db.tasks.count();
  if (count > 0) return;

  console.log('Seeding local mobile DB...');

  await db.tasks.bulkAdd([
    {
      id: 't1',
      caseId: 'CASE-1001',
      title: 'Collection of Deceased',
      description: 'Pick up from hospital morgue',
      status: 'Pending',
      priority: 'Critical',
      dueAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    {
      id: 't2',
      caseId: 'CASE-1002',
      title: 'Venue Setup',
      description: 'Arrange chairs and sound system at church',
      status: 'InProgress',
      priority: 'High',
      dueAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    {
      id: 't3',
      caseId: 'CASE-1003',
      title: 'Casket Delivery',
      description: 'Deliver mahogany casket to family home',
      status: 'Pending',
      priority: 'Medium',
      dueAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
  ]);

  await db.trips.bulkAdd([
    {
      id: 'tr1',
      caseId: 'CASE-1001',
      origin: 'Windhoek',
      destination: 'Johannesburg',
      status: 'Dispatched',
      driverId: 'd1',
      vehicleId: 'V-101',
      updatedAt: new Date().toISOString(),
    },
  ]);

  await db.custody.bulkAdd([
    {
      id: 'c1',
      caseId: 'CASE-1002',
      location: 'Main Cold Room',
      slotId: 'A-12',
      status: 'Admitted',
      updatedAt: new Date().toISOString(),
    },
  ]);

  await db.production.bulkAdd([
    {
      id: 'p1',
      memorialId: 'MEM-501',
      stage: 'Cutting',
      status: 'Active',
      updatedAt: new Date().toISOString(),
    },
  ]);

  console.log('Seeding complete.');
}
