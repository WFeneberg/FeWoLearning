// Reference solution — exercise 075.

export function makeTypeGuard(predicate) {
  return { [Symbol.hasInstance]: (value) => predicate(value) };
}

export function makeDetachedArrayClass() {
  return class Detached extends Array {
    // Species is what Array.prototype.map consults when it builds its
    // result; without it the result would be another Detached.
    static get [Symbol.species]() {
      return Array;
    }

    get first() {
      return this[0];
    }
  };
}

export function concatSpreading() {
  const arrayLike = { length: 2, 0: "a", 1: "b", [Symbol.isConcatSpreadable]: true };
  const realArray = ["a", "b"];
  realArray[Symbol.isConcatSpreadable] = false;
  return { spreadable: [1].concat(arrayLike), notSpreadable: [1].concat(realArray) };
}
