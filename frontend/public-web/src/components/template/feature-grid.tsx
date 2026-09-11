"use client";

import {
  Building2Icon,
  CalendarIcon,
  CreditCardIcon,
  DatabaseIcon,
  FileTextIcon,
  LayersIcon,
  LockIcon,
  PackageIcon,
  ShieldIcon,
  TruckIcon,
  UsersIcon,
} from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { BlurFade } from "@/components/velora/blur-fade";

const features = [
  {
    icon: UsersIcon,
    title: "Customer & Family 360°",
    description: "Complete view of every family, contact history, relationships, and preferences. Polymorphic customer hierarchy (Individual/Organisation).",
    color: "text-blue-500",
    bg: "bg-blue-500/10",
  },
  {
    icon: FileTextIcon,
    title: "Funeral Case Management",
    description: "End-to-end case workflow: intake → arrangement → service → closure. Milestone tracking, SLA monitoring, and automated escalation.",
    color: "text-green-500",
    bg: "bg-green-500/10",
  },
  {
    icon: CalendarIcon,
    title: "Service Arrangements & Catering",
    description: "Drag-and-drop arrangement builder with catering, venue, vehicles, and memorial items. Guest management and seating charts.",
    color: "text-purple-500",
    bg: "bg-purple-500/10",
  },
  {
    icon: ShieldIcon,
    title: "Insurance Policies & Claims",
    description: "Age-banded benefit plans, automated claims processing, and instant inventory/finance bridge — claims create stock orders & journal entries.",
    color: "text-orange-500",
    bg: "bg-orange-500/10",
  },
  {
    icon: PackageIcon,
    title: "Inventory, POS & Vendors",
    description: "Multi-location stock with low-stock alerts, barcode scanning, purchase orders, vendor portal, and automated reorder points.",
    color: "text-teal-500",
    bg: "bg-teal-500/10",
  },
  {
    icon: CreditCardIcon,
    title: "Accounting & SARS Reporting",
    description: "Double-entry ledger, chart of accounts, automated journal entries from operations, EMP201 monthly & EMP501/IRP5 annual certificates.",
    color: "text-indigo-500",
    bg: "bg-indigo-500/10",
  },
  {
    icon: UsersIcon,
    title: "HR, Leave & SA Payroll",
    description: "Employee profiles, contracts, leave accrual/approval, statutory PAYE/UIF/SDL calculations, payslips, and tax year management.",
    color: "text-pink-500",
    bg: "bg-pink-500/10",
  },
  {
    icon: TruckIcon,
    title: "Fleet, Mortuary & Repatriation",
    description: "Vehicle telematics, maintenance schedules, mortuary slot management, custody records, and international repatriation workflows.",
    color: "text-cyan-500",
    bg: "bg-cyan-500/10",
  },
  {
    icon: Building2Icon,
    title: "Memorial Manufacturing",
    description: "Production orders, quality checklists, artisan assignments, installation scheduling, and client sign-off with photo evidence.",
    color: "text-amber-500",
    bg: "bg-amber-500/10",
  },
  {
    icon: LockIcon,
    title: "Multi-Tenant Isolation",
    description: "Each tenant gets a dedicated SQL Server database (KhetLinQ_<slug>). Row-level security, branch scoping, and complete data isolation.",
    color: "text-red-500",
    bg: "bg-red-500/10",
  },
  {
    icon: DatabaseIcon,
    title: "Backup, Restore & Migration",
    description: "Automated per-tenant backups to MinIO/S3, point-in-time restore, and tier migration (Shared → Dedicated infrastructure).",
    color: "text-gray-500",
    bg: "bg-gray-500/10",
  },
  {
    icon: LayersIcon,
    title: "Extensible Platform",
    description: "Plugin architecture for payment gateways (Netcash, Stripe, PayFast, etc.), webhook handlers, feature flags, and tenant-specific overrides.",
    color: "text-violet-500",
    bg: "bg-violet-500/10",
  },
];

export function FeatureGrid() {
  return (
    <BlurFade delay={0.15}>
      <div className="mx-auto mt-16 grid max-w-6xl gap-6 md:grid-cols-2 lg:grid-cols-3">
        {features.map((feature, i) => (
          <BlurFade key={feature.title} delay={i * 0.05}>
            <Card className="h-full transition-shadow hover:shadow-lg">
              <CardHeader>
                <div className={`mb-3 flex h-12 w-12 items-center justify-center rounded-xl ${feature.bg}`}>
                  <feature.icon className={`size-6 ${feature.color}`} />
                </div>
                <CardTitle className="text-lg">{feature.title}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-sm text-muted-foreground">{feature.description}</p>
              </CardContent>
            </Card>
          </BlurFade>
        ))}
      </div>
    </BlurFade>
  );
}