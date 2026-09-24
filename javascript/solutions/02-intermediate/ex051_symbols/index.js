// Reference solution — exercise 051.

export function symbolIdentity() {
  const first = Symbol("x");
  const second = Symbol("x");
  return {
    same: first === second,
    equal: first.description === second.description,
    registeredSame: Symbol.for("x") === Symbol.for("x"),
    keyFor: Symbol.keyFor(Symbol.for("x")),
  };
}

export function hiddenKey(value) {
  const key = Symbol("hidden");
  return { object: { visible: 1, [key]: value }, key };
}

export function taggedObject(tag) {
  return { [Symbol.toStringTag]: tag };
}

export function money(cents) {
  return {
    cents,
    [Symbol.toPrimitive](hint) {
      if (hint === "number") return cents / 100;
      return `${(cents / 100).toFixed(2)} CHF`;
    },
  };
}
