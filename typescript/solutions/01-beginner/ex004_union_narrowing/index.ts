// Reference solution — exercise 004.
export type Input = string | number | boolean | string[];

export function describe(input: Input): string {
  // Array.isArray comes first: typeof an array is "object", so a typeof
  // chain alone never reaches the array branch.
  if (Array.isArray(input)) {
    return `list:${input.length}`;
  }
  if (typeof input === "string") {
    return `text:${input}`;
  }
  if (typeof input === "number") {
    return `number:${input}`;
  }
  return `boolean:${input}`;
}
