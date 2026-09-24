// Reference solution — exercise 001.

export function describeValue(value) {
  if (value === null) return "null";
  if (Array.isArray(value)) return "array";
  return typeof value;
}

export function isPrimitive(value) {
  return value === null || (typeof value !== "object" && typeof value !== "function");
}
