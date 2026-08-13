import { Injectable, InjectionToken, inject } from "@angular/core";

// Exercise 097 — plugin injection tokens: multi-provider extension points (expert).
// Goal:   let independent pieces of code — features, modules, even a lazily-loaded plugin — each
//         contribute their OWN provider for the same extension point, and have a single consumer
//         see all of them, without anyone editing a shared list by hand.
// Drills: `multi: true` on an `InjectionToken<readonly T[]>`, `inject()` collecting every
//         registered contribution into one array (in registration order), and an `optional`
//         injection so "nobody registered any plugins" is `[]`, not a NullInjectorError.
// Passes: when `npx jest exercises/04-expert/ex097_plugin_injection_tokens` is green.
//
// Exercise 019's InjectionToken has exactly one provider — the second `{ provide: API_CONFIG,
// useValue: ... }` registered for the same token in the same injector does not add a second config,
// it REPLACES the first (DI's normal, single-value rule: last provider wins). `multi: true` flips
// that rule for one token: every provider registered for it is kept, and `inject()` returns them
// all as an array, in registration order — which is exactly what an extension point needs. A
// report-exporter registry, an HTTP interceptor chain (`HTTP_INTERCEPTORS`, the real one Angular's
// own HttpClient uses), a set of validators contributed by different feature modules — all of them
// are `multi: true` tokens for this reason.
//
// The contrast worth internalizing (not a second test, just the actual failure mode): declare the
// SAME three `{ provide: REPORT_EXPORTERS, useValue: ..., multi: true }` providers below but drop
// `multi: true` from all three, and `inject(REPORT_EXPORTERS)` no longer returns an array at all —
// it returns whichever single exporter was registered LAST (Angular's normal "last provider wins"
// rule for a non-multi token), and `availableFormats()` would report exactly one format instead of
// three. There's no error, no warning — just a service that silently sees one plugin where three
// were registered, which is why the token's `multi: true` is the load-bearing detail here, not an
// afterthought.
//
// Declaring the token itself as `InjectionToken<readonly ReportExporter[]>` (the ARRAY type, not
// the item type) matches how Angular declares its own multi-provider tokens (see
// `HTTP_INTERCEPTORS` in `@angular/common/http`) — it lets `inject(REPORT_EXPORTERS)` come back
// already typed as `readonly ReportExporter[]`, with no manual cast at the call site.

export interface ReportExporter {
  readonly format: string;
  export(data: readonly string[]): string;
}

/**
 * TODO: a multi-provider InjectionToken for ReportExporter — every provider registered for it must
 * be collected, none of them dropped. Give it a readable description string.
 */
export const REPORT_EXPORTERS = null as unknown as InjectionToken<readonly ReportExporter[]>;

@Injectable({ providedIn: "root" })
export class ReportExportService {
  /**
   * TODO: inject REPORT_EXPORTERS. Because this app itself never registers a default exporter (only
   * tests do, via TestBed providers), injecting it plainly would throw a NullInjectorError when
   * nothing is registered — inject it `{ optional: true }` and fall back to an empty array.
   */
  private readonly exporters: readonly ReportExporter[] = [];

  /** The format of every registered exporter, in registration order. */
  availableFormats(): readonly string[] {
    throw new Error("TODO: implement availableFormats");
  }

  /**
   * Export using the exporter whose format matches (case-sensitive). No exporter registered for
   * that format is a RangeError.
   */
  exportAs(format: string, data: readonly string[]): string {
    throw new Error("TODO: implement exportAs");
  }
}
