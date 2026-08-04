import { inject, Injectable, InjectionToken } from "@angular/core";

// Exercise 019 — InjectionToken for configuration (reference solution).

export interface ApiConfig {
  readonly baseUrl: string;
  readonly timeoutMs: number;
}

// The description is not decoration: it is the name that shows up in a
// NullInjectorError, and it is all the caller has to go on.
export const API_CONFIG = new InjectionToken<ApiConfig>("API_CONFIG");

// A token that carries its own default. Consumers work with no provider at all, and an
// application overrides it only when it wants something else.
export const PAGE_SIZE = new InjectionToken<number>("PAGE_SIZE", {
  providedIn: "root",
  factory: () => 25,
});

@Injectable({ providedIn: "root" })
export class ApiClient {
  readonly config = inject(API_CONFIG);
  readonly pageSize = inject(PAGE_SIZE);

  url(path: string): string {
    // Trim both sides and rejoin: neither end has to agree about slashes.
    const base = this.config.baseUrl.replace(/\/+$/, "");
    const suffix = path.replace(/^\/+/, "");
    return `${base}/${suffix}`;
  }

  describe(path: string): string {
    return `${this.url(path)} (timeout ${this.config.timeoutMs}ms, page ${this.pageSize})`;
  }
}
