import Link from "next/link";
import { Building2Icon } from "lucide-react";

import { siteConfig } from "@/lib/site-config";

const groups = [
  {
    title: "Platform",
    links: [
      { text: "Modules", href: "#features" },
      { text: "Pricing", href: "#pricing" },
      { text: "FAQ", href: "#faq" },
      { text: "Documentation", href: "https://docs.khet360.com" },
    ],
  },
  {
    title: "Solutions",
    links: [
      { text: "Funeral Homes", href: "#" },
      { text: "Crematoriums", href: "#" },
      { text: "Memorial Services", href: "#" },
      { text: "Repatriation", href: "#" },
    ],
  },
  {
    title: "Company",
    links: [
      { text: "About Us", href: "#" },
      { text: "Careers", href: "#" },
      { text: "Blog", href: "#" },
      { text: "Contact", href: "#" },
    ],
  },
  {
    title: "Legal",
    links: [
      { text: "Privacy Policy", href: "#" },
      { text: "Terms of Service", href: "#" },
      { text: "Cookie Policy", href: "#" },
      { text: "DPA", href: "#" },
    ],
  },
];

export function SiteFooter() {
  return (
    <footer className="border-t border-border/40 py-14">
      <div className="mx-auto grid max-w-6xl gap-10 px-4 md:grid-cols-[1.4fr_1fr_1fr_1fr] lg:px-8">
        <div>
          <Link href="/" className="flex items-center gap-2 font-semibold">
            <Building2Icon className="size-5 text-primary" />
            Khet-360
          </Link>
          <p className="mt-3 max-w-xs text-sm text-muted-foreground">
            Multi-tenant ERP platform for funeral homes, memorial services,
            and crematoriums. Built on .NET 10 with clean architecture.
          </p>
          <p className="mt-4 text-xs text-muted-foreground">
            SA statutory compliant • ISO 27001 ready • SOC 2 Type II
          </p>
        </div>
        {groups.map((group) => (
          <nav key={group.title} aria-label={group.title}>
            <h3 className="text-sm font-semibold">{group.title}</h3>
            <ul className="mt-4 space-y-2.5 text-sm text-muted-foreground">
              {group.links.map((link) => (
                <li key={link.text}>
                  {link.href.startsWith("http") ? (
                    <a
                      href={link.href}
                      rel="noopener"
                      target="_blank"
                      className="transition-colors hover:text-foreground"
                    >
                      {link.text}
                    </a>
                  ) : (
                    <Link
                      href={link.href}
                      className="transition-colors hover:text-foreground"
                    >
                      {link.text}
                    </Link>
                  )}
                </li>
              ))}
            </ul>
          </nav>
        ))}
      </div>
      <div className="mx-auto mt-12 flex max-w-6xl flex-col items-center justify-between gap-2 border-t border-border/40 px-4 pt-6 text-xs text-muted-foreground md:flex-row lg:px-8">
        <span>Khet-360 — Built for the funeral industry.</span>
        <span>Data sovereignty • Multi-tenant isolation • Enterprise SLA</span>
      </div>
    </footer>
  );
}