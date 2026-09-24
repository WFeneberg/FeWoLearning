// Reference solution — exercise 013.

export function sum(...numbers) {
  return numbers.reduce((total, value) => total + value, 0);
}

export function maxOf(values) {
  return Math.max(...values);
}

export function mergeConfig(base, override) {
  return { ...base, ...override };
}

export function withoutKey(object, key) {
  const { [key]: _removed, ...rest } = object;
  return rest;
}

export function cloneShallow(object) {
  return { ...object };
}
