// Reference solution — exercise 050.

export function makeSideTable() {
  const table = new WeakMap();
  return {
    set(key, value) {
      table.set(key, value);
      return value;
    },
    get(key) {
      return table.get(key);
    },
    has(key) {
      return table.has(key);
    },
  };
}

export function makeMarker() {
  const marked = new WeakSet();
  return {
    mark(object) {
      marked.add(object);
    },
    wasMarked(object) {
      return marked.has(object);
    },
  };
}

export function useAsWeakKey(key) {
  try {
    new WeakMap().set(key, 1);
    return "ok";
  } catch (error) {
    return error.name;
  }
}

export function makeWeakHolder(object) {
  const ref = new WeakRef(object);
  return { get: () => ref.deref() };
}
