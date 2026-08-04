import { Injectable } from "@angular/core";

// Exercise 018 — inject() vs constructor injection (beginner).
// Goal:   get a dependency without a constructor, and learn where that is allowed.
// Drills: the inject() function in a field initialiser, constructor injection for
//         comparison, inject(X, {optional: true}) for a provider that may not exist,
//         the "injection context" rule (NG0203), and reusable functions that inject.
// Passes: when `npx jest exercises/01-beginner/ex018_inject_function` is green.
//
// inject() is not magic and not a service locator: it only works while Angular is
// constructing something — a field initialiser, a constructor body, a factory, or an
// explicit runInInjectionContext(). Call it later, say from a click handler, and you get
// NG0203. The payoff is that plain *functions* can inject too, which is how the small
// composable helpers at the bottom of this file work; a constructor parameter cannot do
// that because there is no class to put it on.
//
// Why prefer it: no constructor to thread through subclasses, no parameter-property
// boilerplate, and it composes.

@Injectable({ providedIn: "root" })
export class Logger {
  readonly entries: string[] = [];

  log(message: string): void {
    this.entries.push(message);
  }
}

/** Deliberately never provided anywhere — the optional-injection test depends on that. */
export class Telemetry {
  readonly pings: string[] = [];
}

@Injectable({ providedIn: "root" })
export class AuditService {
  /** TODO: obtain the Logger with inject(), not a constructor parameter. */
  readonly logger!: Logger;

  /**
   * TODO: obtain the Telemetry with inject(), tolerating its absence.
   *
   * Nothing provides Telemetry, so a plain inject() would throw. Ask for it optionally
   * and this is null instead — which is the whole point of the flag.
   */
  readonly telemetry!: Telemetry | null;

  /** Log `"audit: <action>"`, and ping telemetry as well if it happens to be there. */
  record(action: string): void {
    throw new Error("TODO: implement record");
  }
}

@Injectable({ providedIn: "root" })
export class ClassicAuditService {
  /**
   * The older style, for comparison: a constructor parameter property.
   *
   * TODO: take the Logger as a private readonly constructor parameter and keep a
   * `logger` field, so the spec can reach it.
   */
  readonly logger!: Logger;

  record(action: string): void {
    throw new Error("TODO: implement record");
  }
}

/**
 * TODO: a reusable helper that injects.
 *
 * Returns a function that logs `"tick <n>"` on each call, counting from 1. Grab the
 * Logger with inject() *here*, in the helper's own body — that is what makes this
 * callable from any injection context without being a class.
 */
export function createTicker(): () => void {
  throw new Error("TODO: implement createTicker");
}
