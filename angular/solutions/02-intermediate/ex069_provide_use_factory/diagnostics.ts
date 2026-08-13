import { Injectable, InjectionToken, Provider, inject } from "@angular/core";

// Exercise 069 — useFactory, useExisting, useValue (reference solution).

export abstract class Logger {
  abstract readonly lines: readonly string[];
  abstract log(message: string): void;
}

@Injectable()
export class ConsoleLogger extends Logger {
  readonly lines: string[] = [];

  log(message: string): void {
    this.lines.push(message);
  }
}

export const DEBUG_MODE = new InjectionToken<boolean>("DEBUG_MODE", {
  providedIn: "root",
  factory: () => false,
});

export const LOG_LEVEL = new InjectionToken<string>("LOG_LEVEL", {
  providedIn: "root",
  // No `deps` array needed — inject() works directly inside a token factory.
  factory: () => (inject(DEBUG_MODE) ? "debug" : "info"),
});

export const APP_VERSION = new InjectionToken<string>("APP_VERSION");

export const DIAGNOSTICS_PROVIDERS: Provider[] = [
  ConsoleLogger,
  // Same instance as ConsoleLogger, not a second one built from useClass.
  { provide: Logger, useExisting: ConsoleLogger },
];

@Injectable({ providedIn: "root" })
export class Diagnostics {
  private readonly logger = inject(Logger);
  private readonly consoleLogger = inject(ConsoleLogger);
  private readonly level = inject(LOG_LEVEL);
  private readonly version = inject(APP_VERSION);

  report(message: string): string {
    const line = `v${this.version} [${this.level}] ${message}`;
    this.logger.log(line);
    return line;
  }

  sameInstance(): boolean {
    return this.logger === this.consoleLogger;
  }
}
