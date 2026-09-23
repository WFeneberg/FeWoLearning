// Exercise 081 — augmenting a module you do not own (advanced).
// Goal:   add a member to someone else's interface, and make the type
//         system believe you.
// Drills: `declare module`, interface merging across files, the
//         difference between declaring and providing.
// Passes: Request carries a member ./library never declared, and the
//         helper that sets it type-checks without a cast.
//
// ex080 merged two declarations in one file. Module augmentation is the
// same mechanism reaching ACROSS files: inside a module, a
// `declare module "./library" { … }` block reopens that module's
// declarations and merges into them. This is how a plugin adds `req.user`
// to Express's Request, or how a library adds a method to Array.
//
// Three rules that are easy to get wrong:
//
// The augmenting file must itself BE a module — it needs a top-level
// import or export. In a script file, `declare module "x"` means
// something entirely different (an ambient declaration for a whole
// module) and will not merge.
//
// The specifier must resolve the same way an import would. `"./library"`
// here, not the resolved path and not a rename.
//
// And you may only ADD. An augmentation cannot change the type of an
// existing member or remove one; merging an incompatible declaration is
// an error rather than an override.
//
// MEASURED, and it is why both new members below are OPTIONAL: an
// augmentation adding a REQUIRED member breaks the augmented module's own
// code. Declaring `startedAt: number` makes ./library's own makeRequest
// stop compiling, because it does not set one — and you cannot fix that,
// since you do not own the file. An augmentation that must be safe for
// code you cannot change therefore adds optional members only.
//
// The last rule is the one worth internalising: augmentation is a
// PROMISE, exactly like ex080's class merge and ex014's predicate.
// Declaring `traceId` does not put one there. Something has to assign it,
// and nothing checks that anything did — which is why `withTrace` below
// is part of the exercise rather than an afterthought.

import { makeRequest } from "./library";
import type { Request } from "./library";

/** TODO: augment "./library" so Request also has an optional
 *  `traceId?: string` and an optional `startedAt?: number`. */

/** TODO: build a request via makeRequest and set both new members —
 *  `startedAt` from the clock argument, `traceId` from the argument. */
export function tracedRequest(_url: string, _traceId: string, _now: number): Request {
  throw new Error("TODO: implement tracedRequest");
}

/** TODO: the trace id if the request has one, otherwise "untraced". */
export function traceOf(_request: Request): string {
  throw new Error("TODO: implement traceOf");
}
