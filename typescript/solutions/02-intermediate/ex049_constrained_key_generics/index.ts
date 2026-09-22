// Reference solution — exercise 049.
export function getIn<T extends object, K extends keyof T>(obj: T, key: K): T[K] {
  return obj[key];
}

// T[K] in a parameter position: the value is checked against the key that
// was actually passed.
export function setIn<T extends object, K extends keyof T>(
  obj: T,
  key: K,
  value: T[K],
): T {
  return { ...obj, [key]: value };
}

export function pluck<T extends object, K extends keyof T>(
  items: readonly T[],
  key: K,
): T[K][] {
  return items.map((item) => item[key]);
}
