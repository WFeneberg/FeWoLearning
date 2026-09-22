// Exercise 053 — narrowing with `in` and `instanceof` (intermediate).
// Goal:   tell union members apart when there is no discriminant to switch
//         on, and tell errors apart at a catch boundary.
// Drills: the `in` operator as a type guard, `instanceof` on a class
//         hierarchy, a predicate built from `in`.
// Passes: area handles both shapes, isCircle narrows, and describeThrown
//         sorts three kinds of failure.
//
// Shape below has NO shared tag, so ex015's switch has nothing to switch
// on. `"radius" in shape` narrows instead: the operator is a type guard as
// well as a runtime check.
//
// Worth knowing what `in` actually asks. It is about the KEY EXISTING, not
// about the value — `"radius" in { radius: undefined }` is true. It also
// walks the prototype chain, so an inherited property counts. For a plain
// data union neither matters; for anything class-shaped, both can.
//
// `instanceof` is the other half, and it is the one that works on classes
// rather than on shapes: it walks the prototype chain, so a subclass
// satisfies its base. It cannot narrow to an INTERFACE — there is nothing
// at runtime to test against — which is why describeThrown's error union
// is built from classes.

export interface Circle {
  radius: number;
}

export interface Square {
  side: number;
}

export type Shape = Circle | Square;

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

export class TimeoutError extends ApiError {
  constructor(public readonly afterMs: number) {
    super(`timed out after ${afterMs}ms`, 408);
    this.name = "TimeoutError";
  }
}

/** TODO: PI * r^2 for a circle, side^2 for a square. */
export function area(_shape: Shape): number {
  throw new Error("TODO: implement area");
}

/** TODO: true when `shape` is a Circle — and narrow it for the caller. */
export function isCircle(_shape: Shape): boolean {
  throw new Error("TODO: implement isCircle");
}

/**
 * TODO: describe whatever was thrown —
 *   a TimeoutError  -> `timeout:<afterMs>`
 *   any other ApiError -> `api:<status>`
 *   any other Error -> `error:<message>`
 *   anything else   -> `unknown`
 * Order matters: a TimeoutError is also an ApiError.
 */
export function describeThrown(_value: unknown): string {
  throw new Error("TODO: implement describeThrown");
}
