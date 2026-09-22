// Exercise 022 — `implements` checks, but it does not create a type
// (beginner).
// Goal:   see that declaring an interface changes nothing about who
//         satisfies it.
// Drills: structural assignability of class types, `implements` as a
//         local assertion.
// Passes: both classes below are assignable to Logger — including the one
//         that never mentions it — and an incomplete shape is not.
//
// In C#, `class X : ILogger` is what MAKES X an ILogger, and a class that
// forgets the declaration is simply not one. Here it is the reverse: the
// shape decides, and `implements` only asks the compiler to check the class
// against the interface AT ITS DECLARATION, so mistakes are reported on the
// class rather than at every call site. It adds nothing to the class's type.
//
// Which is why `implements` itself cannot be graded, by any test: removing it
// from a correct class changes no type and no behaviour. What the facts below
// grade is the consequence — who is assignable to Logger — and SilentLogger
// is there to make the point, because it never declares anything.

export interface Logger {
  readonly prefix: string;
  log(message: string): void;
}

/** Given. Note that it says nothing about Logger. Do not change. */
export class SilentLogger {
  readonly prefix = "silent";
  readonly seen: string[] = [];

  log(message: string): void {
    this.seen.push(message);
  }
}

/**
 * TODO: collect messages into `lines`, each prefixed as `<prefix>: <message>`.
 * Declare `implements Logger` while you are at it — not because the type
 * needs it, but because it moves the error to this line if you get it wrong.
 */
export class CollectingLogger {
  readonly prefix = "collect";
  readonly lines: string[] = [];

  log(_message: string): void {
    throw new Error("TODO: implement CollectingLogger.log");
  }
}

/** TODO: resolve to true when C is assignable to Logger, false otherwise. */
export type SatisfiesLogger<C> = unknown;

/** Logs every message through `logger`. */
export function logAll(_logger: Logger, _messages: readonly string[]): void {
  throw new Error("TODO: implement logAll");
}
