import { ErrorHandler, Injectable, InjectionToken, inject } from "@angular/core";

// Exercise 088 — a custom global ErrorHandler with reporting (reference solution).

export interface ErrorReport {
  readonly message: string;
  readonly stack?: string;
  readonly timestamp: number;
}

export interface ErrorSink {
  report(entry: ErrorReport): void;
}

export const ERROR_SINK = new InjectionToken<ErrorSink>("ERROR_SINK");

@Injectable()
export class AppErrorHandler implements ErrorHandler {
  private readonly sink = inject(ERROR_SINK);
  private lastReported: ErrorReport | null = null;

  handleError(error: unknown): void {
    const report: ErrorReport =
      error instanceof Error
        ? { message: error.message, stack: error.stack, timestamp: Date.now() }
        : { message: String(error), timestamp: Date.now() };

    if (this.lastReported?.message === report.message) {
      return;
    }

    this.sink.report(report);
    this.lastReported = report;
  }
}
