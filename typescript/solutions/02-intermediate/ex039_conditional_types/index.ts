// Reference solution — exercise 039.
export type Classify<T> = T extends readonly unknown[]
  ? "array"
  : T extends (...args: never[]) => unknown
    ? "function"
    : T extends object
      ? "object"
      : "primitive";

export type HasLength<T> = T extends { length: number } ? true : false;

export function classifyValue(
  value: unknown,
): "array" | "function" | "object" | "primitive" {
  // Same ordering problem as the type: typeof an array is "object".
  if (Array.isArray(value)) {
    return "array";
  }
  if (typeof value === "function") {
    return "function";
  }
  if (typeof value === "object" && value !== null) {
    return "object";
  }
  return "primitive";
}
