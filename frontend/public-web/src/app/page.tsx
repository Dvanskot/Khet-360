import { Suspense } from "react";
import { Building2Icon, RocketIcon, ZapIcon } from "lucide-react";

import { SiteHeader } from "@/components/site-header";
import { SiteFooter } from "@/components/site-footer";
import { AnimatedGradientText } from "@/components/velora/animated-gradient-text";
import { AuroraBackground } from "@/components/velora/aurora-background";
import { AvatarCircles } from "@/components/velora/avatar-circles";
import { BlurFade } from "@/components/velora/blur-fade";
import { Marquee } from "@/components/velora/marquee";
import { NumberTicker } from "@/components/velora/number-ticker";
import { Particles } from "@/components/velora/particles";
import { ScrollProgress } from "@/components/velora/scroll-progress";
import { ShimmerButton } from "@/components/velora/shimmer-button";
import { SpotlightCard } from "@/components/velora/spotlight-card";
import { TextReveal } from "@/components/velora/text-reveal";
import { TiltCard } from "@/components/velora/tilt-card";
import { Typewriter } from "@/components/velora/typewriter";
import { FeatureGrid } from "@/components/template/feature-grid";
import { HeroSection } from "@/components/template/hero-section";
import { PricingSection } from "@/components/template/pricing-section";
import { siteConfig } from "@/lib/site-config";

const logos = [
  "Heritage Funerals",
  "Eternal Rest",
  "Peaceful Memories",
  "Legacy Memorials",
  "Guardian Services",
  "Compassionate Care",
];

const testimonials = [
  {
    quote:
      "Khet-360 transformed our funeral home operations. The case management and family portal saved us hours every week.",
    name: "Sarah Mitchell",
    role: "Owner, Heritage Funerals",
  },
  {
    quote: "The insurance-policy-to-finance bridge is a game changer. Claims automatically create inventory orders and journal entries.",
    name: "James Okafor",
    role: "Operations Director, Eternal Rest",
  },
  {
    quote: "SA statutory payroll (PAYE/UIF/SDL) and SARS reporting (EMP201/EMP501) built-in. Compliance is no longer a headache.",
    name: "Priya Naidoo",
    role: "HR Manager, Peaceful Memories",
  },
  {
    quote: "Multi-tenancy with complete data isolation gave us confidence to scale. Each branch gets its own database.",
    name: "Michael Chen",
    role: "CTO, Legacy Memorials",
  },
];

const faqs = [
  {
    q: "What is Khet-360?",
    a: "Khet-360 is a multi-tenant ERP platform built specifically for funeral homes, memorial services, crematoriums, and related businesses. It combines CRM, case management, insurance, payroll, inventory, accounting, and more in one integrated system.",
  },
  {
    q: "How does the multi-tenancy work?",
    a: "Each tenant gets their own isolated SQL Server database (named `KhetLinQ_<slug>`). The platform control plane manages tenants, subscriptions, and billing. Tenant data is completely isolated — no shared tables between tenants.",
  },
  {
    q: "What's included in the 30-day free trial?",
    a: "Full access to all modules for your selected plan tier. No credit card required. Your data is preserved if you convert to a paid subscription.",
  },
  {
    q: "Is South African statutory compliance included?",
    a: "Yes. The payroll engine handles PAYE, UIF, SDL calculations per SARS tax tables. EMP201 monthly returns and EMP501/IRP5 annual certificates are generated automatically.",
  },
  {
    q: "Can I migrate from my existing system?",
    a: "Yes. We provide migration tooling and APIs for importing customers, cases, inventory, and financial data. Dedicated migration support is available for Pro plans.",
  },
  {
    q: "What deployment options exist?",
    a: "SaaS (shared infrastructure, isolated databases) or Dedicated (your own SQL Server instance). Both include automated backups, monitoring, and SLA-backed uptime.",
  },
];

export default async function Home() {
  return (
    <main className="relative min-h-screen">
      <ScrollProgress />

      <SiteHeader />

      <Suspense fallback={<div className="h-96" />}>
        <HeroSection />
      </Suspense>

      {/* Logo marquee */}
      <section className="border-y border-border/40 py-12">
        <div className="mx-auto max-w-6xl px-4 lg:px-8">
          <p className="mb-8 text-center text-sm text-muted-foreground">
            Trusted by funeral homes across South Africa
          </p>
          <Marquee pauseOnHover className="[--duration:30s]">
            {logos.map((logo) => (
              <span
                key={logo}
                className="mx-8 text-xl font-semibold tracking-tight text-muted-foreground/60 transition-colors hover:text-foreground"
              >
                {logo}
              </span>
            ))}
          </Marquee>
        </div>
      </section>

      {/* Feature highlights */}
      <section id="features" className="relative py-24 lg:py-32">
        <div className="mx-auto max-w-6xl px-4 lg:px-8">
          <BlurFade>
            <h2 className="mx-auto max-w-2xl text-center text-3xl font-semibold tracking-tight text-balance lg:text-5xl">
              Everything you need to <span className="text-primary">run your business</span>
            </h2>
            <p className="mx-auto mt-4 max-w-xl text-center text-muted-foreground">
              Purpose-built modules that work together seamlessly — from first call to final invoice.
            </p>
          </BlurFade>

          <Suspense fallback={<div className="h-64" />}>
            <FeatureGrid />
          </Suspense>
        </div>
      </section>

      {/* Stats */}
      <section className="relative py-16 lg:py-24 bg-muted/30">
        <div className="mx-auto max-w-6xl px-4 lg:px-8">
          <div className="mx-auto grid max-w-4xl grid-cols-2 gap-8 lg:grid-cols-4">
            {[
              { value: 10, suffix: "+", prefix: "", label: "Core Modules" },
              { value: 99.9, suffix: "%", prefix: "", label: "Uptime SLA" },
              { value: 256, suffix: "", prefix: "R", label: "Starting /month" },
              { value: 30, suffix: " days", prefix: "", label: "Free Trial" },
            ].map((stat, i) => (
              <BlurFade key={stat.label} delay={i * 0.1}>
                <div className="flex flex-col items-center gap-1 text-center">
                  <span className="text-4xl font-semibold tracking-tight">
                    <NumberTicker
                      value={stat.value}
                      prefix={stat.prefix}
                      suffix={stat.suffix}
                    />
                  </span>
                  <span className="text-sm text-muted-foreground">{stat.label}</span>
                </div>
              </BlurFade>
            ))}
          </div>
        </div>
      </section>

      {/* Pricing from API */}
      <Suspense fallback={<div className="h-96" />}>
        <PricingSection />
      </Suspense>

      {/* Testimonials */}
      <section className="relative py-24 lg:py-32">
        <div className="mx-auto max-w-6xl px-4 lg:px-8">
          <BlurFade>
            <h2 className="mx-auto max-w-2xl text-center text-3xl font-semibold tracking-tight text-balance lg:text-5xl">
              Funeral homes <span className="text-primary">love Khet-360</span>
            </h2>
            <p className="mx-auto mt-4 max-w-xl text-center text-muted-foreground">
              See what our customers have to say about their experience.
            </p>
          </BlurFade>
          <div className="mt-16 columns-1 gap-6 md:columns-2 lg:columns-3 [&>*]:mb-6">
            {testimonials.map((t, i) => (
              <BlurFade key={t.name} delay={(i % 3) * 0.1} className="break-inside-avoid">
                <TiltCard>
                  <figure className="rounded-2xl border bg-card p-6 h-full">
                    <span className="flex gap-0.5 text-amber-400">
                      {Array.from({ length: 5 }).map((_, s) => (
                        <ZapIcon key={s} className="size-3.5 fill-current" />
                      ))}
                    </span>
                    <blockquote className="mt-4 text-sm text-card-foreground">
                      "{t.quote}"
                    </blockquote>
                    <figcaption className="mt-4 flex items-center gap-3">
                      <AvatarCircles people={[t.name]} className="[&>span]:size-8 [&>span]:text-[10px]" />
                      <div>
                        <p className="text-sm font-medium">{t.name}</p>
                        <p className="text-xs text-muted-foreground">{t.role}</p>
                      </div>
                    </figcaption>
                  </figure>
                </TiltCard>
              </BlurFade>
            ))}
          </div>
        </div>
      </section>

      {/* FAQ */}
      <section id="faq" className="py-24 lg:py-32">
        <div className="mx-auto max-w-3xl px-4 lg:px-8">
          <BlurFade>
            <h2 className="text-center text-3xl font-semibold tracking-tight lg:text-4xl">
              Frequently asked questions
            </h2>
          </BlurFade>
          <BlurFade delay={0.15}>
            <div className="mt-12 space-y-4">
              {faqs.map((faq) => (
                <details key={faq.q} className="group rounded-xl border bg-card p-6">
                  <summary className="flex cursor-pointer items-center justify-between list-none text-base font-medium">
                    {faq.q}
                    <ZapIcon className="size-5 text-muted-foreground transition-transform group-open:rotate-180" />
                  </summary>
                  <div className="mt-4 text-muted-foreground">{faq.a}</div>
                </details>
              ))}
            </div>
          </BlurFade>
        </div>
      </section>

      {/* Final CTA */}
      <section className="relative overflow-hidden py-24 lg:py-32">
        <AuroraBackground intensity="subtle" />
        <Particles quantity={50} />
        <div className="relative mx-auto max-w-4xl px-4 text-center lg:px-8">
          <BlurFade>
            <h2 className="text-4xl font-semibold tracking-tight text-balance lg:text-6xl">
              Ready to modernize your <span className="text-primary">funeral business</span>?
            </h2>
            <p className="mx-auto mt-6 max-w-xl text-lg text-muted-foreground">
              Join hundreds of funeral homes already using Khet-360. Start your free trial today.
            </p>
            <div className="mt-10">
              <ShimmerButton className="h-14 px-10 text-base">
                <RocketIcon className="size-5" />
                Start Free Trial
              </ShimmerButton>
            </div>
          </BlurFade>
        </div>
      </section>

      <SiteFooter />
    </main>
  );
}