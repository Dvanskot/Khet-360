"use client";

import { RocketIcon, SparklesIcon, ZapIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ShimmerButton } from "@/components/velora/shimmer-button";
import { AnimatedGradientText } from "@/components/velora/animated-gradient-text";
import { AuroraBackground } from "@/components/velora/aurora-background";
import { BlurFade } from "@/components/velora/blur-fade";
import { GridPattern } from "@/components/velora/grid-pattern";
import { HeroMockup } from "@/components/demo/hero-mockup";
import { NumberTicker } from "@/components/velora/number-ticker";
import { TextReveal } from "@/components/velora/text-reveal";
import { Typewriter } from "@/components/velora/typewriter";

export function HeroSection() {
  const stats = [
    { value: 10, suffix: "+", prefix: "", label: "Core Modules" },
    { value: 99.9, suffix: "%", prefix: "", label: "Uptime SLA" },
    { value: 256, suffix: "", prefix: "R", label: "Starting /month" },
    { value: 30, suffix: " days", prefix: "", label: "Free Trial" },
  ];

  return (
    <section className="relative overflow-hidden pt-40 pb-24 lg:pt-48 lg:pb-28">
      <AuroraBackground intensity="subtle" />
      <GridPattern
        width={48}
        height={48}
        className="fill-transparent stroke-border/60 [mask-image:radial-gradient(ellipse_60%_50%_at_50%_0%,black,transparent)]"
      />
      <div className="relative mx-auto max-w-6xl px-4 text-center lg:px-8">
        <BlurFade delay={0}>
          <span className="inline-flex items-center gap-2 rounded-full border border-border/60 bg-card/60 px-4 py-1.5 text-sm backdrop-blur">
            <SparklesIcon className="size-3.5 text-primary" />
            <span className="font-medium">
              Khet-360 — Multi-tenant ERP for Funeral Services
            </span>
          </span>
        </BlurFade>

        <h1 className="mx-auto mt-8 max-w-4xl text-5xl font-semibold tracking-tight text-balance lg:text-7xl">
          <TextReveal text="The complete platform for" />{" "}
          <AnimatedGradientText>
            <Typewriter words={["funeral homes.", "memorial services.", "crematoriums."]} />
          </AnimatedGradientText>
        </h1>

        <BlurFade delay={0.35}>
          <p className="mx-auto mt-6 max-w-2xl text-lg text-muted-foreground text-pretty">
            CRM, case management, insurance, payroll, inventory, accounting, and compliance —
            all in one integrated system. Built for South African funeral businesses.
          </p>
        </BlurFade>

        <BlurFade delay={0.5}>
          <div className="mt-10 flex flex-wrap items-center justify-center gap-4">
            <ShimmerButton onClick={() => document.getElementById("signup")?.scrollIntoView({ behavior: "smooth" })}>
              <RocketIcon className="size-4" />
              Start Free Trial
            </ShimmerButton>
            <Button variant="ghost" size="lg" asChild>
              <a href="#features">Explore Modules</a>
            </Button>
          </div>
        </BlurFade>

        <BlurFade delay={0.6}>
          <div className="mt-8 flex flex-col items-center justify-center gap-3 sm:flex-row">
            <div className="flex flex-col items-center gap-0.5 sm:items-start">
              <span className="flex gap-0.5 text-amber-400">
                {Array.from({ length: 5 }).map((_, i) => (
                  <ZapIcon key={i} className="size-4 fill-current" />
                ))}
              </span>
              <span className="text-sm text-muted-foreground">
                Trusted by 200+ funeral homes across SA
              </span>
            </div>
          </div>
        </BlurFade>

        {/* Product mockup */}
        <BlurFade delay={0.75} offset={32}>
          <HeroMockup className="mt-20" />
        </BlurFade>

        {/* Stats */}
        <div className="mx-auto mt-20 grid max-w-4xl grid-cols-2 gap-8 lg:grid-cols-4">
          {stats.map((stat, i) => (
            <BlurFade key={stat.label} delay={i * 0.1}>
              <div className="flex flex-col items-center gap-1">
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
  );
}