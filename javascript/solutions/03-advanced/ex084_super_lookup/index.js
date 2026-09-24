// Reference solution — exercise 084.

export function makeChild(parent) {
  const child = {
    describe() {
      return `child(${super.describe()})`;
    },
  };
  // The home object is the literal above; setPrototypeOf decides what
  // `super` reaches from it.
  return Object.setPrototypeOf(child, parent);
}

export function describeWithNewParent(child, newParent) {
  Object.setPrototypeOf(child, newParent);
  return child.describe();
}

export function describeAfterCopying(child, otherParent) {
  const other = Object.setPrototypeOf({ describe: child.describe }, otherParent);
  return other.describe();
}

export function makeAccessorChild(base, extra) {
  const child = {
    extra,
    get total() {
      return super.total + this.extra;
    },
  };
  return Object.setPrototypeOf(child, base);
}
