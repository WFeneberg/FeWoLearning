// Exercise 080 — merging a declaration into something that already exists
// (advanced).
// Goal:   attach members to a function and to a class from outside their
//         own declaration.
// Drills: function + namespace merging, class + interface merging.
// Passes: the counter is callable AND carries its own statics, and the
//         class gains a member it never declared.
//
// ex002 showed two interfaces merging. The rule is broader: several
// declarations of the same NAME in the same scope merge when their
// declaration spaces allow it, and the useful pairs are these two.
//
// A FUNCTION and a NAMESPACE of the same name merge into a callable that
// also has properties. That is how `jQuery(...)` and `jQuery.ajax` coexist,
// and how a factory can carry its own `reset` without a wrapper object.
// The namespace's exports become properties of the function, and the type
// follows automatically — no separate interface to keep in step.
//
// A CLASS and an INTERFACE of the same name merge too, which adds members
// to the class's TYPE without adding them to its body. That is the
// declarative half of a mixin: the interface promises the member, some
// other code assigns it, and callers see it. TypeScript does not check
// that anyone actually assigned it — the promise is unverified, exactly
// like a type predicate (ex014) — so it is a statement of intent backed
// by the code next to it.
//
// A caveat that shapes the runtime half: merging declares the member, and
// something still has to PUT IT THERE. The facts below check both.

/** TODO: call it to get the next number. */
export function counter(): number {
  throw new Error("TODO: implement counter");
}

/** TODO: merge a namespace into `counter` above, exporting a `reset()`
 *  that sends the sequence back to the start, and a readonly `start`
 *  holding the first number the counter will produce. */

export class Widget {
  constructor(public readonly name: string) {}

  describe(): string {
    throw new Error("TODO: implement describe");
  }
}

/** TODO: merge an interface into Widget above, declaring a `render()`
 *  that returns a string — then assign an implementation onto
 *  Widget.prototype below, since merging only declares it. */
