// Exercise 025 — `#private` is real, `private` is a promise (beginner).
// Goal:   see the runtime difference between the two kinds of private.
// Drills: `#` fields, the `#x in obj` brand check, enumerability.
// Passes: a SoftSecret leaks its field to Object.keys and a HardSecret does
//         not, and isHardSecret recognises its own instances and narrows.
//
// `private` (ex021) is erased: the field is an ordinary property, visible to
// Object.keys, JSON.stringify, the debugger and any JavaScript caller. A `#`
// field is enforced by the runtime — genuinely unreachable from outside, not
// merely discouraged, and absent from every enumeration.
//
// The second half is the trick `#` makes possible and nothing else does.
// `#value in candidate` is a BRAND CHECK: it asks whether that object was
// constructed by this class, and an object of the same shape cannot fake it —
// unlike `instanceof`, which a forged prototype defeats. There is no C#
// analogue; the nearest thing is a sealed type identity.

/** Given, for comparison. `private` is a compile-time promise only. */
export class SoftSecret {
  constructor(private value: string) {}

  reveal(): string {
    return this.value;
  }
}

export class HardSecret {
  /** TODO: store the value in a real `#` private field. */
  constructor(_value: string) {
    throw new Error("TODO: implement the HardSecret constructor");
  }

  reveal(): string {
    throw new Error("TODO: implement reveal");
  }

  /**
   * TODO: true when `candidate` was constructed by this class. Use the brand
   * check rather than instanceof — and narrow `candidate` for the caller.
   */
  static isHardSecret(_candidate: unknown): boolean {
    throw new Error("TODO: implement isHardSecret");
  }
}

/** The own enumerable property names of an object. */
export function visibleFields(_instance: object): string[] {
  throw new Error("TODO: implement visibleFields");
}
