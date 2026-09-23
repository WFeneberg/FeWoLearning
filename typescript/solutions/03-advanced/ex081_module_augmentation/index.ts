// Reference solution — exercise 081.
import { makeRequest } from "./library";
import type { Request } from "./library";

// Reopens ./library's declarations and merges into them. This file has
// top-level imports, so it is a module, which is what makes this an
// augmentation rather than an ambient declaration.
//
// Both members are optional because a required one would break
// ./library's own makeRequest, which does not set them.
declare module "./library" {
  interface Request {
    traceId?: string;
    startedAt?: number;
  }
}

export function tracedRequest(url: string, traceId: string, now: number): Request {
  // makeRequest knows nothing about the new members, so they are set
  // here — the augmentation declared them, it did not provide them.
  const request = makeRequest(url);
  request.startedAt = now;
  request.traceId = traceId;
  return request;
}

export function traceOf(request: Request): string {
  return request.traceId ?? "untraced";
}
