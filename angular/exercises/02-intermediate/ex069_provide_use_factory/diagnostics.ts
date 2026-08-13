import { Injectable, InjectionToken, Provider, inject } from "@angular/core";

// Exercise 069 — useFactory, useExisting, useValue (intermediate).
// Goal:   pick the right provider recipe for three different shapes of dependency.
// Drills: a token computed from another injected token (useFactory), aliasing one token to an
//         existing instance rather than creating a second one (useExisting), and a static
//         constant supplied with no computation at all (useValue).
// Passes: when `npx jest exercises/02-intermediate/ex069_provide_use_factory` is green.
//
// The three recipes answer three different questions. `useValue` is for "here is the value,
// verbatim" — a constant with nothing to compute. `useFactory` is for "run this function, in an
// injection context, to produce the value" — and since Angular 14 the function can just call
// inject() itself rather than declaring a `deps` array, which is what LOG_LEVEL below does to read
// DEBUG_MODE. `useExisting` is for neither a value nor a factory: it says "this token means the
// same instance as that other token," so injecting either one gets you the identical object,
// constructed exactly once. useClass would build a second ConsoleLogger — a different instance with
// its own state — which defeats the point of a shared logger.
//
// Providing an abstract class as a token, rather than an interface, is what lets an interface-like
// contract exist at runtime at all: `inject(Logger)` needs Logger to be a real value, and an
// `interface` is erased by the time the code runs.

export abstract class Logger {
  abstract readonly lines: readonly string[];
  abstract log(message: string): void;
}

@Injectable()
export class ConsoleLogger extends Logger {
  readonly lines: string[] = [];

  log(message: string): void {
    throw new Error("TODO: implement log");
  }
}

/** A root default of "false" — DEBUG_MODE is itself overridable, same as LOG_PREFIX in ex068. */
export const DEBUG_MODE = new InjectionToken<boolean>("DEBUG_MODE", {
  providedIn: "root",
  factory: () => false,
});

/**
 * TODO: a factory that reads DEBUG_MODE (via inject(), no `deps` array needed) and returns
 * "debug" when it is true, "info" otherwise.
 */
export const LOG_LEVEL = new InjectionToken<string>("LOG_LEVEL", {
  providedIn: "root",
  factory: () => {
    throw new Error("TODO: implement the LOG_LEVEL factory");
  },
});

/** No default — every caller must supply one with useValue. */
export const APP_VERSION = new InjectionToken<string>("APP_VERSION");

/**
 * TODO: the three provider mechanics this exercise drills, assembled into one array:
 *  - ConsoleLogger itself, so there is an instance to alias to.
 *  - useExisting:  Logger -> ConsoleLogger (the same instance, not a second one).
 *  - useValue:     APP_VERSION -> "1.4.0".
 *
 * DEBUG_MODE and LOG_LEVEL are deliberately not here — they already have root defaults, and a
 * caller only needs to override DEBUG_MODE when it wants something other than that default.
 */
export const DIAGNOSTICS_PROVIDERS: Provider[] = [];

@Injectable({ providedIn: "root" })
export class Diagnostics {
  private readonly logger = inject(Logger);
  private readonly consoleLogger = inject(ConsoleLogger);
  private readonly level = inject(LOG_LEVEL);
  private readonly version = inject(APP_VERSION);

  /**
   * TODO: "v<version> [<level>] <message>", and also send that same string to the logger.
   *
   * e.g. version "1.4.0", level "info", message "ready" -> "v1.4.0 [info] ready".
   */
  report(message: string): string {
    throw new Error("TODO: implement report");
  }

  /** TODO: true when Logger and ConsoleLogger resolved to the exact same instance. */
  sameInstance(): boolean {
    throw new Error("TODO: implement sameInstance");
  }
}
