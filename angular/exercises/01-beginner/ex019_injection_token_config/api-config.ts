import { Injectable, InjectionToken } from "@angular/core";

// Exercise 019 — InjectionToken for configuration (beginner).
// Goal:   inject something that is not a class.
// Drills: new InjectionToken<T>(), providing one with useValue, a token that supplies its
//         own default through `factory`, and why an interface cannot be a DI key.
// Passes: when `npx jest exercises/01-beginner/ex019_injection_token_config` is green.
//
// DI looks dependencies up by a runtime *value*. A class works as its own key because the
// class object survives compilation; an `interface` does not exist at runtime at all, so
// `inject(ApiConfig)` cannot compile and a plain string key would collide with anyone
// else's "config". An InjectionToken is a unique object created for exactly this purpose,
// and it carries the type parameter so inject() still comes back typed.
//
// A token with `{providedIn: "root", factory: …}` supplies its own default, so consumers
// work with no provider at all and an application overrides it only when it wants to.
// Without a factory, a missing provider is a NullInjectorError — which is the right
// behaviour for something like a base URL that has no sensible default.

export interface ApiConfig {
  readonly baseUrl: string;
  readonly timeoutMs: number;
}

/**
 * TODO: a token for ApiConfig, with no default.
 *
 * Give it a readable description — it is what shows up in a NullInjectorError.
 */
export const API_CONFIG = null as unknown as InjectionToken<ApiConfig>;

/** TODO: a token for the page size that defaults to 25 via a root-provided factory. */
export const PAGE_SIZE = null as unknown as InjectionToken<number>;

@Injectable({ providedIn: "root" })
export class ApiClient {
  /** TODO: inject the API config. */
  readonly config!: ApiConfig;

  /** TODO: inject the page size. */
  readonly pageSize!: number;

  /**
   * Join the base URL and a path with exactly one slash between them, whatever slashes
   * the two happen to arrive with. `("https://x/", "/users")` is `"https://x/users"`.
   */
  url(path: string): string {
    throw new Error("TODO: implement url");
  }

  /** `"<url> (timeout <ms>ms, page <n>)"` — proof that all three values arrived. */
  describe(path: string): string {
    throw new Error("TODO: implement describe");
  }
}
