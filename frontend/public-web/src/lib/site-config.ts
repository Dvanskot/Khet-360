/**
 * Single source of truth for external URLs. Override at build time with
 * NEXT_PUBLIC_SITE_URL / NEXT_PUBLIC_GITHUB_URL when the real domain and
 * repo are wired up.
 */
export const siteConfig = {
  name: "Khet-360",
  url: (
    process.env.NEXT_PUBLIC_SITE_URL ?? "https://khet360.com"
  ).replace(/\/$/, ""),
  github:
    process.env.NEXT_PUBLIC_GITHUB_URL ??
    "https://github.com/Khet-360/khet360",
  tagline: "Multi-tenant ERP for Funeral & Memorial Services",
  description:
    "Khet-360 is a comprehensive multi-tenant ERP platform designed for funeral homes, memorial services, and related businesses. Built on .NET 10 with clean architecture, featuring CRM, case management, insurance, payroll, inventory, and more.",
} as const;