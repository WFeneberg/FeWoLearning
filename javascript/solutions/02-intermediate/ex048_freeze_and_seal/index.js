// Reference solution — exercise 048.

export function classify(object) {
  return {
    frozen: Object.isFrozen(object),
    sealed: Object.isSealed(object),
    extensible: Object.isExtensible(object),
  };
}

export function freezeDeep(object, seen = new WeakSet()) {
  if (object === null || typeof object !== "object") return object;
  if (seen.has(object)) return object; // the cycle guard
  seen.add(object);
  Object.freeze(object);
  for (const value of Object.values(object)) freezeDeep(value, seen);
  return object;
}

export function shallowGap() {
  const outer = Object.freeze({ nested: { value: 1 } });
  outer.nested.value = 2; // allowed: only `outer` was frozen
  return { frozenOuter: Object.isFrozen(outer), nestedValue: outer.nested.value };
}

export function probeMutations(object) {
  const attempt = (fn) => {
    try {
      fn();
      return "ok";
    } catch (error) {
      return error.name;
    }
  };
  return {
    write: attempt(() => {
      object.a = 99;
    }),
    addKey: attempt(() => {
      object.fresh = 1;
    }),
    deleteKey: attempt(() => {
      delete object.a;
    }),
  };
}
