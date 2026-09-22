// Exercise 024 — accessors and static members (beginner).
// Goal:   put a rule behind a property, and hang constants and a factory off
//         the class itself.
// Drills: get/set accessors, a computed getter, static members, a static
//         initialization block.
// Passes: celsius clamps at absolute zero, fahrenheit is derived, the
//         Celsius constant is readonly, and the factory round-trips.
//
// An accessor pair looks like a field from outside and runs code on every
// read and write — which is how `celsius` refuses a value below absolute zero
// without the caller doing anything. A getter with no setter is readonly to
// the type system, so `fahrenheit` needs no defending.
//
// MEASURED, and it decides the shape of the two constants below: a `readonly`
// static CANNOT be assigned from a static block. Both `Temperature.X = …` and
// `this.X = …` inside `static { }` fail with TS2540, unlike a readonly
// instance field, which a constructor may assign. So a readonly static needs
// a plain initializer, and a `static { }` block is for the mutable ones —
// which is why ABSOLUTE_ZERO_C and ABSOLUTE_ZERO_F are declared differently.
//
// Note also that nothing can observe WHICH mechanism set a static: a block
// and an initializer leave identical evidence. The facts grade the values and
// the readonly modifier, never the block.

export class Temperature {
  /** TODO: -273.15, and readonly. A readonly static needs an initializer. */
  static ABSOLUTE_ZERO_C: number = 0;

  /** TODO: the same temperature in Fahrenheit (-459.67), derived from
   *  ABSOLUTE_ZERO_C in a `static { }` block rather than written out. */
  static ABSOLUTE_ZERO_F: number = 0;

  private celsiusValue = 0;

  /** The temperature in Celsius. */
  get celsius(): number {
    throw new Error("TODO: implement the celsius getter");
  }

  /** TODO: store the value, but never below ABSOLUTE_ZERO_C. */
  set celsius(_value: number) {
    throw new Error("TODO: implement the celsius setter");
  }

  /** TODO: the same temperature in Fahrenheit — c * 9/5 + 32. Read-only. */
  get fahrenheit(): number {
    throw new Error("TODO: implement the fahrenheit getter");
  }

  /** TODO: a Temperature built from a Fahrenheit reading. */
  static fromFahrenheit(_f: number): Temperature {
    throw new Error("TODO: implement fromFahrenheit");
  }
}
