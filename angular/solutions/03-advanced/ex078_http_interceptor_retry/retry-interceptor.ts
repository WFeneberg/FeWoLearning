import { InjectionToken, inject } from "@angular/core";
import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { retry, throwError, timer } from "rxjs";

// Exercise 078 — an HTTP interceptor that retries transient failures with backoff (reference solution).

export interface RetryConfig {
  readonly maxAttempts: number;
  readonly baseDelayMs: number;
}

export const RETRY_CONFIG = new InjectionToken<RetryConfig>("RETRY_CONFIG", {
  factory: (): RetryConfig => ({ maxAttempts: 3, baseDelayMs: 200 }),
});

export function computeBackoffDelayMs(attempt: number, baseDelayMs: number): number {
  return baseDelayMs * 2 ** (attempt - 1);
}

export const retryInterceptor: HttpInterceptorFn = (req, next) => {
  const config = inject(RETRY_CONFIG);

  return next(req).pipe(
    retry({
      count: Math.max(0, config.maxAttempts - 1),
      // Growing delay for a transient 5xx; immediate give-up (via throwError) for anything else,
      // since retrying an identical request that failed for a non-transient reason cannot help.
      delay: (error: unknown, retryCount: number) => {
        if (!(error instanceof HttpErrorResponse) || error.status < 500) {
          return throwError(() => error);
        }
        return timer(computeBackoffDelayMs(retryCount, config.baseDelayMs));
      },
    }),
  );
};
