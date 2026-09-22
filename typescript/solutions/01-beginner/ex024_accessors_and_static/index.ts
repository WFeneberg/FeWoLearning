// Reference solution — exercise 024.
export class Temperature {
  // An initializer, not a static block: a readonly static cannot be assigned
  // from one (TS2540), in either the `Temperature.X` or the `this.X` form.
  static readonly ABSOLUTE_ZERO_C = -273.15;

  // Mutable, so the block may assign it. This is what a static block is for:
  // a value that takes more than an expression, or one derived from members
  // declared above it.
  static ABSOLUTE_ZERO_F: number = 0;

  static {
    Temperature.ABSOLUTE_ZERO_F = Temperature.ABSOLUTE_ZERO_C * (9 / 5) + 32;
  }

  private celsiusValue = 0;

  get celsius(): number {
    return this.celsiusValue;
  }

  set celsius(value: number) {
    this.celsiusValue = Math.max(value, Temperature.ABSOLUTE_ZERO_C);
  }

  // No setter, so the type system treats this as readonly.
  get fahrenheit(): number {
    return this.celsiusValue * (9 / 5) + 32;
  }

  static fromFahrenheit(f: number): Temperature {
    const temperature = new Temperature();
    temperature.celsius = (f - 32) * (5 / 9);
    return temperature;
  }
}
