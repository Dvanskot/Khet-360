import { z } from "zod";

const API_BASE = process.env.NEXT_PUBLIC_API_BASE ?? "http://localhost:8080";

export const SubscriptionPlanSchema = z.object({
  id: z.string().uuid(),
  name: z.string(),
  description: z.string(),
  category: z.string(),
  monthlyPrice: z.number(),
  annualPrice: z.number(),
  trialPeriodDays: z.number(),
  entitlements: z.array(
    z.object({
      code: z.string(),
      description: z.string(),
      limitValue: z.number().nullable(),
    })
  ),
});

export type SubscriptionPlan = z.infer<typeof SubscriptionPlanSchema>;

export const TrialSignupSchema = z.object({
  companyName: z.string().min(1).max(255),
  slug: z.string().min(1).max(100).regex(/^[a-z0-9-]+$/),
  subscriptionPlanId: z.string().uuid(),
  email: z.string().email().max(320),
});

export type TrialSignupInput = z.infer<typeof TrialSignupSchema>;

export const SubscribeSchema = z.object({
  companyName: z.string().min(1).max(255),
  slug: z.string().min(1).max(100).regex(/^[a-z0-9-]+$/),
  subscriptionPlanId: z.string().uuid(),
  email: z.string().email().max(320),
});

export type SubscribeInput = z.infer<typeof SubscribeSchema>;

export const TrialSignupResponseSchema = z.object({
  message: z.string(),
  tenantId: z.string().uuid(),
  trialEndDate: z.string().datetime(),
});

export const SubscribeResponseSchema = z.object({
  message: z.string(),
  paymentLink: z.string().url(),
  plan: z.string(),
  amount: z.number(),
});

export async function fetchPublicPlans(): Promise<SubscriptionPlan[]> {
  const res = await fetch(`${API_BASE}/platform/api/public/plans`, {
    next: { revalidate: 300 },
    headers: { "Content-Type": "application/json" },
  });

  if (!res.ok) {
    throw new Error("Failed to fetch subscription plans");
  }

  const data = await res.json();
  return z.array(SubscriptionPlanSchema).parse(data);
}

export async function startFreeTrial(input: TrialSignupInput) {
  const validated = TrialSignupSchema.parse(input);

  const res = await fetch(`${API_BASE}/platform/api/public/trial`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(validated),
  });

  const data = await res.json();

  if (!res.ok) {
    throw new Error(data.Message ?? data.message ?? "Failed to start free trial");
  }

  return TrialSignupResponseSchema.parse(data);
}

export async function initiateSubscription(input: SubscribeInput) {
  const validated = SubscribeSchema.parse(input);

  const res = await fetch(`${API_BASE}/platform/api/public/subscribe`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(validated),
  });

  const data = await res.json();

  if (!res.ok) {
    throw new Error(data.Message ?? data.message ?? "Failed to initiate subscription");
  }

  return SubscribeResponseSchema.parse(data);
}