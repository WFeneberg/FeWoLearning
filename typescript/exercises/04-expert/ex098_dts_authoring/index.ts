// Exercise 098 — writing a declaration file (expert).
// Goal:   type a JavaScript module you cannot change, and feel the
//         weight of the promise.
// Drills: `declare` in a .d.ts, optional parameters, a default export,
//         and how little the compiler checks.
// Passes: the declarations match the implementation, and the wrapper
//         below compiles and runs against them.
//
// ./legacy.js is untyped JavaScript. ./legacy.d.ts is where you tell the
// compiler what it contains — and the work is in that file, not this
// one.
//
// The thing to internalise: a .d.ts is NOT CHECKED against the code it
// describes. TypeScript never opens ./legacy.js. Every line you write
// there is taken on trust, so a wrong declaration is not a compile
// error anywhere — it is a lie that spreads to every consumer and only
// surfaces as a runtime failure far away. That is the same unverified
// promise as a type predicate (ex014), a class merge (ex080) and a
// module augmentation (ex081), and it is the largest of the three in
// blast radius.
//
// Which is also why `any` in a hand-written .d.ts is so expensive: it
// is not a gap in the types, it is a hole that everything downstream
// falls into. `unknown` at least forces the consumer to decide.
//
// This file is the consumer, and it is short on purpose — the facts
// grade the declarations through it.

import format, { DEFAULTS, slugify, truncate } from "./legacy";

/** TODO: slugify `title`, then truncate it to `limit`. */
export function shortSlug(_title: string, _limit: number): string {
  throw new Error("TODO: implement shortSlug");
}

/** TODO: slugify with an underscore instead of the default separator. */
export function underscored(_title: string): string {
  throw new Error("TODO: implement underscored");
}

/** TODO: the module's default export, applied to `title`. */
export function formatted(_title: string): string {
  throw new Error("TODO: implement formatted");
}

/** TODO: the limit the module ships with. */
export function defaultLimit(): number {
  throw new Error("TODO: implement defaultLimit");
}

void format;
void DEFAULTS;
void slugify;
void truncate;
