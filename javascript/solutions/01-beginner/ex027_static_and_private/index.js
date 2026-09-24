// Reference solution — exercise 027.

export class Counter {
  #count = 0;
  static #created = 0;

  constructor() {
    Counter.#created++;
  }

  increment(amount = 1) {
    this.#count += amount;
    return this.#count;
  }

  get value() {
    return this.#count;
  }

  #format() {
    return `Counter(${this.#count})`;
  }

  toString() {
    return this.#format();
  }

  static get created() {
    return Counter.#created;
  }

  static isCounter(candidate) {
    // `#count in x` is the brand check: it is false for anything that was
    // not constructed by this class, and it cannot throw.
    return #count in Object(candidate);
  }
}
