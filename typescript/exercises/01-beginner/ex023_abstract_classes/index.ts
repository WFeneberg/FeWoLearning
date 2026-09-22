// Exercise 023 — abstract classes and protected members (beginner).
// Goal:   publish a base class that cannot be used directly, and a contract
//         only its subclasses can see.
// Drills: `abstract` on a class and on members, `protected`.
// Passes: Shape cannot be constructed, name() is not callable from outside,
//         and both subclasses describe themselves.
//
// The stub's Shape is an ordinary class with ordinary public methods, and
// tightening it is the exercise. Both of the things you are adding are
// refusals — you may not instantiate this, you may not call that from out
// here — so both are graded with @ts-expect-error, which is itself an error
// when the line below it compiles. On the untouched stub those lines do
// compile, which is why the facts start red.
//
// `protected` is the same compile-time-only promise as `private` (ex021):
// erased in the output, and readable from plain JavaScript.

/**
 * TODO: make this class abstract, and make area() and name() abstract
 * members with no bodies. name() must be protected. Implement describe()
 * here, once, in terms of the two.
 */
export class Shape {
  /** The area. */
  area(): number {
    throw new Error("TODO: make area abstract");
  }

  /** A short name for this kind of shape — for subclasses only. */
  name(): string {
    throw new Error("TODO: make name abstract and protected");
  }

  /** `<name>: <area>` with the area at two decimals, e.g. `circle: 12.57`. */
  describe(): string {
    throw new Error("TODO: implement describe");
  }
}

/** TODO: name "circle", area PI * r^2. */
export class Circle extends Shape {
  constructor(public readonly radius: number) {
    super();
  }
}

/** TODO: name "rect", area width * height. */
export class Rect extends Shape {
  constructor(
    public readonly width: number,
    public readonly height: number,
  ) {
    super();
  }
}
