import { inject, Injectable } from "@angular/core";

// Exercise 018 — inject() vs constructor injection (reference solution).

@Injectable({ providedIn: "root" })
export class Logger {
  readonly entries: string[] = [];

  log(message: string): void {
    this.entries.push(message);
  }
}

export class Telemetry {
  readonly pings: string[] = [];
}

@Injectable({ providedIn: "root" })
export class AuditService {
  // A field initialiser is an injection context, so inject() works here.
  readonly logger = inject(Logger);

  // Nothing provides Telemetry; without `optional` this line would throw.
  readonly telemetry = inject(Telemetry, { optional: true });

  record(action: string): void {
    this.logger.log(`audit: ${action}`);
    // Optional means "possibly null", so the null check is not defensive noise.
    this.telemetry?.pings.push(action);
  }
}

@Injectable({ providedIn: "root" })
export class ClassicAuditService {
  // The older style: a parameter property. Same instance, more ceremony, and it cannot
  // be shared with a plain function the way inject() can.
  constructor(readonly logger: Logger) {}

  record(action: string): void {
    this.logger.log(`audit: ${action}`);
  }
}

export function createTicker(): () => void {
  // inject() runs while the *caller* is in an injection context, which is what lets a
  // free function participate in DI at all.
  const logger = inject(Logger);
  let count = 0;
  return () => {
    count += 1;
    logger.log(`tick ${count}`);
  };
}
