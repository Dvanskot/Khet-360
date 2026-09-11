"use client";

import { useState, useEffect, Suspense } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { fetchPublicPlans, startFreeTrial, type SubscriptionPlan } from "@/lib/api";
import { CheckIcon, RocketIcon } from "lucide-react";
import { BlurFade } from "@/components/velora/blur-fade";
import { BorderBeam } from "@/components/velora/border-beam";

export function PlanSelector() {
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [selectedPlanId, setSelectedPlanId] = useState<string>("");

  useEffect(() => {
    fetchPublicPlans()
      .then(setPlans)
      .catch(console.error);
  }, []);

  return (
    <select
      id="subscriptionPlanId"
      name="subscriptionPlanId"
      value={selectedPlanId}
      onChange={(e) => setSelectedPlanId(e.target.value)}
      className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
      required
    >
      <option value="">Select a plan...</option>
      {plans.map((plan) => (
        <option key={plan.id} value={plan.id}>
          {plan.name} — {plan.monthlyPrice === 0 ? "Free" : `R${plan.monthlyPrice}/month`}
        </option>
      ))}
    </select>
  );
}

export function TrialSignupForm() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);
    setSuccess(null);

    const formData = new FormData(e.currentTarget);
    const data = {
      companyName: formData.get("companyName") as string,
      slug: formData.get("slug") as string,
      subscriptionPlanId: formData.get("subscriptionPlanId") as string,
      email: formData.get("email") as string,
    };

    try {
      const result = await startFreeTrial(data);
      setSuccess(`Trial started! Your tenant ID: ${result.tenantId}. Check your email for login details.`);
      e.currentTarget.reset();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Signup failed. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="mt-12 space-y-6 rounded-2xl border bg-card p-8 shadow-lg">
      {error && (
        <div className="rounded-md bg-destructive/10 p-4 text-sm text-destructive" role="alert">
          {error}
        </div>
      )}
      {success && (
        <div className="rounded-md bg-green-500/10 p-4 text-sm text-green-600" role="status">
          {success}
        </div>
      )}
      <div className="grid gap-4 md:grid-cols-2">
        <div className="space-y-2">
          <Label htmlFor="companyName">Company Name</Label>
          <Input
            id="companyName"
            name="companyName"
            placeholder="Heritage Funerals"
            required
            disabled={isSubmitting}
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="slug">Subdomain (slug)</Label>
          <div className="relative">
            <Input
              id="slug"
              name="slug"
              placeholder="heritage-funerals"
              pattern="^[a-z0-9-]+$"
              required
              disabled={isSubmitting}
            />
            <span className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground text-sm">
              .khet360.com
            </span>
          </div>
          <p className="text-xs text-muted-foreground">
            Lowercase letters, numbers, and hyphens only. This becomes your subdomain.
          </p>
        </div>
        <div className="space-y-2 md:col-span-2">
          <Label htmlFor="email">Email Address</Label>
          <Input
            id="email"
            name="email"
            type="email"
            placeholder="owner@heritagefunerals.co.za"
            required
            disabled={isSubmitting}
          />
        </div>
        <div className="space-y-2 md:col-span-2">
          <Label htmlFor="subscriptionPlanId">Select Plan</Label>
          <PlanSelector />
          <p className="text-xs text-muted-foreground">
            Plan selection is saved locally. You can change it before submitting.
          </p>
        </div>
      </div>
      <Button type="submit" className="w-full rounded-full" size="lg" disabled={isSubmitting}>
        {isSubmitting ? (
          <>
            <svg className="mr-2 size-4 animate-spin" viewBox="0 0 24 24">
              <circle
                className="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                strokeWidth="4"
                fill="none"
              />
              <path
                className="opacity-75"
                d="M12 2a10 10 0 0 1 10 10"
                stroke="currentColor"
                strokeWidth="4"
                fill="none"
                strokeLinecap="round"
              />
            </svg>
            Creating your trial...
          </>
        ) : (
          <>
            <RocketIcon className="size-4 mr-2" />
            Start Free Trial
          </>
        )}
      </Button>
      <p className="text-center text-xs text-muted-foreground">
        By signing up, you agree to our Terms of Service and Privacy Policy.
      </p>
    </form>
  );
}

function PlansListClient() {
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchPublicPlans()
      .then((data) => {
        setPlans(data);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) {
    return (
      <BlurFade delay={0.15}>
        <div className="mx-auto mt-16 grid max-w-5xl gap-6 md:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3].map((i) => (
            <BlurFade key={i} delay={i * 0.1}>
              <Card className="flex h-full flex-col animate-pulse">
                <CardHeader>
                  <div className="h-6 bg-muted w-3/4 rounded" />
                  <div className="h-4 bg-muted w-1/2 rounded mt-2" />
                </CardHeader>
                <CardContent className="flex-1 flex flex-col">
                  <div className="mb-6">
                    <div className="h-12 bg-muted w-1/4 rounded" />
                  </div>
                  <ul className="mb-6 flex-1 space-y-3">
                    {[1, 2, 3, 4].map((j) => (
                      <li key={j} className="h-4 bg-muted w-3/4 rounded" />
                    ))}
                  </ul>
                  <Button className="w-full rounded-full" disabled>
                    <div className="h-10 w-1/3 bg-muted rounded-full mx-auto" />
                  </Button>
                </CardContent>
              </Card>
            </BlurFade>
          ))}
        </div>
      </BlurFade>
    );
  }

  if (error) {
    return (
      <div className="text-center py-12 text-muted-foreground">
        <p>Unable to load pricing plans. Please try again later.</p>
        <button
          onClick={() => window.location.reload()}
          className="mt-4 text-primary underline"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <BlurFade delay={0.15}>
      <div className="mx-auto mt-16 grid max-w-5xl gap-6 md:grid-cols-2 lg:grid-cols-3">
        {plans.map((plan, i) => (
          <BlurFade key={plan.id} delay={i * 0.1}>
            <Card className="flex h-full flex-col relative overflow-hidden">
              <BorderBeam size={80} duration={8 + i * 2} className="absolute top-0 left-0 right-0" />
              <CardHeader>
                <CardTitle className="text-lg">{plan.name}</CardTitle>
                <CardDescription>{plan.description}</CardDescription>
              </CardHeader>
              <CardContent className="flex-1 flex flex-col">
                <div className="mb-6">
                  <p className="text-5xl font-semibold tracking-tight">
                    {plan.monthlyPrice === 0 ? (
                      "Free"
                    ) : (
                      <>
                        R{plan.monthlyPrice.toLocaleString()}
                        <span className="text-base font-normal text-muted-foreground"> /month</span>
                      </>
                    )}
                  </p>
                  {plan.annualPrice > 0 && (
                    <p className="text-sm text-muted-foreground">
                      R{plan.annualPrice.toLocaleString()} /year (save {Math.round((1 - plan.annualPrice / (plan.monthlyPrice * 12)) * 100)}%)
                    </p>
                  )}
                </div>
                <ul className="mb-6 flex-1 space-y-3 text-sm">
                  {plan.entitlements.slice(0, 8).map((ent) => (
                    <li key={ent.code} className="flex items-center gap-3">
                      <span className="flex size-5 items-center justify-center rounded-full bg-primary/15 text-primary">
                        <CheckIcon className="size-3" />
                      </span>
                      <span>{ent.description}</span>
                    </li>
                  ))}
                  {plan.entitlements.length > 8 && (
                    <li className="flex items-center gap-3 text-muted-foreground">
                      <span className="flex size-5 items-center justify-center rounded-full bg-muted">
                        <span className="size-3">+</span>
                      </span>
                      <span>+{plan.entitlements.length - 8} more features</span>
                    </li>
                  )}
                </ul>
                <Button
                  variant={plan.monthlyPrice === 0 ? "default" : "outline"}
                  size="lg"
                  className="w-full rounded-full"
                  onClick={() => {
                    document.getElementById("signup")?.scrollIntoView({ behavior: "smooth" });
                    localStorage.setItem("selectedPlanId", plan.id);
                    localStorage.setItem("selectedPlanName", plan.name);
                  }}
                >
                  {plan.monthlyPrice === 0 ? "Start Free" : "Start Trial"}
                </Button>
              </CardContent>
            </Card>
          </BlurFade>
        ))}
      </div>
    </BlurFade>
  );
}

export function PricingSection() {
  return (
    <section id="pricing" className="relative py-24 lg:py-32">
      <div className="mx-auto max-w-6xl px-4 lg:px-8">
        <BlurFade>
          <h2 className="mx-auto max-w-2xl text-center text-3xl font-semibold tracking-tight text-balance lg:text-5xl">
            Simple, transparent <span className="text-primary">pricing</span>
          </h2>
          <p className="mx-auto mt-4 max-w-xl text-center text-muted-foreground">
            Choose the plan that fits your business. All plans include a 30-day free trial.
          </p>
        </BlurFade>

        <PlansListClient />

        {/* Signup Form */}
        <BlurFade delay={0.3}>
          <div id="signup" className="mx-auto mt-24 max-w-3xl px-4 lg:px-8">
            <h3 className="mx-auto max-w-2xl text-center text-2xl font-semibold tracking-tight text-balance">
              Start your <span className="text-primary">30-day free trial</span>
            </h3>
            <p className="mx-auto mt-4 max-w-xl text-center text-muted-foreground">
              No credit card required. Full access to all features. Cancel anytime.
            </p>
            <TrialSignupForm />
          </div>
        </BlurFade>
      </div>
    </section>
  );
}