// Reference solution — exercise 057.

export function deepClone(value, seen = new WeakMap()) {
  if (value === null || typeof value !== "object") return value; // functions included
  if (seen.has(value)) return seen.get(value); // cycles and shared references

  if (value instanceof Date) return new Date(value.getTime());

  if (value instanceof Map) {
    const copy = new Map();
    seen.set(value, copy); // register BEFORE recursing, or a cycle recurses forever
    for (const [key, item] of value) copy.set(deepClone(key, seen), deepClone(item, seen));
    return copy;
  }

  if (value instanceof Set) {
    const copy = new Set();
    seen.set(value, copy);
    for (const item of value) copy.add(deepClone(item, seen));
    return copy;
  }

  if (Array.isArray(value)) {
    const copy = [];
    seen.set(value, copy);
    for (const item of value) copy.push(deepClone(item, seen));
    return copy;
  }

  const copy = {};
  seen.set(value, copy);
  for (const [key, item] of Object.entries(value)) copy[key] = deepClone(item, seen);
  return copy;
}
