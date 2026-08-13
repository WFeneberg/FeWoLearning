import { Injectable, InjectionToken, inject } from "@angular/core";

// Exercise 097 — plugin injection tokens: multi-provider extension points (reference solution).

export interface ReportExporter {
  readonly format: string;
  export(data: readonly string[]): string;
}

// InjectionToken<readonly ReportExporter[]> — the ARRAY type, matching how HTTP_INTERCEPTORS is
// declared — so inject() below comes back already typed, no cast needed at the call site.
export const REPORT_EXPORTERS = new InjectionToken<readonly ReportExporter[]>(
  "app.report-exporters",
);

@Injectable({ providedIn: "root" })
export class ReportExportService {
  // optional: true — no app-level default is registered, only tests provide any; without it, an
  // app with zero plugins registered would throw instead of just having an empty extension point.
  private readonly exporters: readonly ReportExporter[] =
    inject(REPORT_EXPORTERS, { optional: true }) ?? [];

  availableFormats(): readonly string[] {
    return this.exporters.map((exporter) => exporter.format);
  }

  exportAs(format: string, data: readonly string[]): string {
    const exporter = this.exporters.find((candidate) => candidate.format === format);
    if (!exporter) {
      throw new RangeError(`no exporter registered for format: ${format}`);
    }
    return exporter.export(data);
  }
}
