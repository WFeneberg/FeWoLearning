// Reference solution — exercise 056.

export function deepEqual(a, b, seen = new Map()) {
  // SameValueZero: covers every primitive, NaN included, and identical
  // object references.
  if (a === b || (a !== a && b !== b)) return true;

  if (a === null || b === null) return false;
  if (typeof a !== "object" || typeof b !== "object") return false;

  // The cycle guard: if this exact pair is already under comparison, the
  // structures agree so far and the recursion must stop.
  const inProgress = seen.get(a);
  if (inProgress?.has(b)) return true;
  if (inProgress === undefined) seen.set(a, new Set([b]));
  else inProgress.add(b);

  if (a instanceof Date || b instanceof Date) {
    return a instanceof Date && b instanceof Date && a.getTime() === b.getTime();
  }

  if (Array.isArray(a) !== Array.isArray(b)) return false;

  if (Array.isArray(a)) {
    if (a.length !== b.length) return false;
    return a.every((value, index) => deepEqual(value, b[index], seen));
  }

  const aKeys = Object.keys(a);
  if (aKeys.length !== Object.keys(b).length) return false;
  return aKeys.every((key) => Object.hasOwn(b, key) && deepEqual(a[key], b[key], seen));
}
