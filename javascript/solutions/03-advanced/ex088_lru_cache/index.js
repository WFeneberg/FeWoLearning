// Reference solution — exercise 088.

export function createLruCache(max) {
  const entries = new Map();

  const touch = (key, value) => {
    // delete + set is what moves a key to the end of a Map's order.
    entries.delete(key);
    entries.set(key, value);
  };

  return {
    get(key) {
      if (!entries.has(key)) return undefined;
      const value = entries.get(key);
      touch(key, value);
      return value;
    },
    set(key, value) {
      touch(key, value);
      if (entries.size > max) {
        const oldest = entries.keys().next().value;
        entries.delete(oldest);
      }
      return value;
    },
    has: (key) => entries.has(key),
    get size() {
      return entries.size;
    },
    keys: () => [...entries.keys()],
  };
}
