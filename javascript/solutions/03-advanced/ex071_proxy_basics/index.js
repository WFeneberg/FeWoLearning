// Reference solution — exercise 071.

export function strict(target) {
  return new Proxy(target, {
    get(object, key, receiver) {
      if (typeof key === "symbol" || Reflect.has(object, key)) {
        return Reflect.get(object, key, receiver);
      }
      throw new ReferenceError(`unknown property: ${key}`);
    },
  });
}

export function validated(target, validators) {
  return new Proxy(target, {
    set(object, key, value, receiver) {
      const validate = validators[key];
      if (validate !== undefined && !validate(value)) {
        throw new TypeError(`invalid value for ${key}`);
      }
      return Reflect.set(object, key, value, receiver);
    },
  });
}

export function counting(target) {
  const counts = new Map();
  const view = new Proxy(target, {
    get(object, key, receiver) {
      counts.set(key, (counts.get(key) ?? 0) + 1);
      return Reflect.get(object, key, receiver);
    },
  });
  return { view, counts };
}

export function hide(target, hiddenKeys) {
  const hidden = new Set(hiddenKeys);
  return new Proxy(target, {
    get: (object, key, receiver) =>
      hidden.has(key) ? undefined : Reflect.get(object, key, receiver),
    has: (object, key) => !hidden.has(key) && Reflect.has(object, key),
    ownKeys: (object) => Reflect.ownKeys(object).filter((key) => !hidden.has(key)),
    getOwnPropertyDescriptor: (object, key) =>
      hidden.has(key) ? undefined : Reflect.getOwnPropertyDescriptor(object, key),
    deleteProperty: (object, key) => (hidden.has(key) ? true : Reflect.deleteProperty(object, key)),
  });
}
