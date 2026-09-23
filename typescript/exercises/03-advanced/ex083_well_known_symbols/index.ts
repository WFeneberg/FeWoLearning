// Exercise 083 — the well-known symbols (advanced).
// Goal:   change how the language itself treats your object.
// Drills: Symbol.hasInstance, Symbol.toPrimitive, Symbol.toStringTag.
// Passes: instanceof answers for something that is not an instance, the
//         same object coerces differently per hint, and it renames itself
//         for Object.prototype.toString.
//
// This row replaces the method-decorator one the catalog originally
// planned, for the reason ex082's header records: standard decorators do
// not survive this track's transform. What a method decorator does —
// reaching into how a member behaves from outside its own definition —
// the well-known symbols do to the LANGUAGE's own operators, which is
// the same idea one level deeper and has no C# equivalent at all.
//
// `Symbol.hasInstance` is a STATIC method, and it takes over `instanceof`
// entirely: `x instanceof C` calls `C[Symbol.hasInstance](x)` when that
// exists, prototype chain or no prototype chain. That makes a duck-typing
// check usable with the operator everyone already knows — and it also
// means `instanceof` is not the identity guarantee it looks like, which
// is worth remembering next to ex025's brand check.
//
// `Symbol.toPrimitive` decides what happens when an object is coerced,
// and it is passed a HINT: "number" for arithmetic, "string" for
// interpolation, and "default" for `+` and `==`, where the engine has no
// opinion. Handling only two of the three is the usual bug.
//
// `Symbol.toStringTag` changes what `Object.prototype.toString` reports,
// which is what distinguishes `[object Map]` from `[object Object]`.

export class Money {
  constructor(
    public readonly amount: number,
    public readonly currency: string,
  ) {}

  /** TODO: "number" -> the amount; "string" -> `<amount> <currency>`;
   *  "default" -> the string form, since `+` on money is ambiguous. */

  /** TODO: report "Money" to Object.prototype.toString. */
}

/** TODO: make `x instanceof Sequence` true for ANY iterable — including
 *  arrays and strings — rather than only for its own instances. */
export class Sequence {
  static from(values: Iterable<unknown>): unknown[] {
    return [...values];
  }
}

/** The tag Object.prototype.toString reports for `value`. Given. */
export function tagOf(value: unknown): string {
  return Object.prototype.toString.call(value).slice("[object ".length, -1);
}
