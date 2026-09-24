// Reference solution — exercise 045.

export function createWith(proto, ownProps) {
  return Object.assign(Object.create(proto), ownProps);
}

export function chainOf(value) {
  const chain = [];
  let current = Object.getPrototypeOf(value);
  while (current !== null) {
    chain.push(current);
    current = Object.getPrototypeOf(current);
  }
  chain.push(null);
  return chain;
}

export function describeProperty(object, key) {
  return {
    own: Object.hasOwn(object, key),
    inChain: key in object,
    value: object[key],
  };
}

export function shadow(object, key, value) {
  const proto = Object.getPrototypeOf(object);
  // A write always lands on the object itself; it never reaches through to
  // the prototype (barring a setter up there).
  object[key] = value;
  return { objectValue: object[key], protoValue: proto?.[key] };
}

export function bareObject() {
  const bare = Object.create(null);
  bare.safe = true;
  return bare;
}
