// Reference solution — exercise 024.

function guard(celsius) {
  if (celsius < -273.15) throw new RangeError("below absolute zero");
  return celsius;
}

export function makeTemperature(celsius) {
  // The number lives in the closure, so there is no field to write around
  // the setter.
  let value = guard(celsius);
  return {
    get celsius() {
      return value;
    },
    set celsius(next) {
      value = guard(next);
    },
    get fahrenheit() {
      return value * (9 / 5) + 32;
    },
    set fahrenheit(next) {
      value = guard((next - 32) * (5 / 9));
    },
  };
}

export function makeProbe() {
  return {
    callCount: 0,
    get value() {
      this.callCount++;
      return "read";
    },
  };
}
