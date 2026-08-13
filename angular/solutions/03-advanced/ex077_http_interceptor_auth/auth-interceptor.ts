import { Injectable, Signal, inject, signal } from "@angular/core";
import { HttpInterceptorFn } from "@angular/common/http";

// Exercise 077 — a functional HTTP interceptor that attaches an auth header (reference solution).

@Injectable({ providedIn: "root" })
export class TokenStore {
  private readonly token = signal<string | null>(null);
  readonly current: Signal<string | null> = this.token.asReadonly();

  set(value: string | null): void {
    this.token.set(value);
  }
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(TokenStore).current();
  if (!token) {
    return next(req);
  }

  // clone(), not mutation — the same HttpRequest instance may be replayed elsewhere.
  return next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
