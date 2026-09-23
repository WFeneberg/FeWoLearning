// Reference solution — exercise 083.
export class Money {
  constructor(
    public readonly amount: number,
    public readonly currency: string,
  ) {}

  [Symbol.toPrimitive](hint: "number" | "string" | "default"): number | string {
    if (hint === "number") {
      return this.amount;
    }
    // "default" is what `+` and `==` use, and money has no meaningful
    // numeric addition across currencies — so it renders rather than
    // silently adding.
    return `${this.amount} ${this.currency}`;
  }

  get [Symbol.toStringTag](): string {
    return "Money";
  }
}

export class Sequence {
  // Static, and it replaces `instanceof` outright: no prototype chain is
  // consulted once this exists.
  static [Symbol.hasInstance](value: unknown): boolean {
    return (
      value !== null &&
      value !== undefined &&
      typeof (value as { [Symbol.iterator]?: unknown })[Symbol.iterator] === "function"
    );
  }

  static from(values: Iterable<unknown>): unknown[] {
    return [...values];
  }
}

export function tagOf(value: unknown): string {
  return Object.prototype.toString.call(value).slice("[object ".length, -1);
}
