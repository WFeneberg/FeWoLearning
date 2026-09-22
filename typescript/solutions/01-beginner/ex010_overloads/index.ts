// Reference solution — exercise 010.
export function parse(input: string): string[];
export function parse(input: number): number[];
// Not an overload itself: callers never see this signature.
export function parse(input: string | number): string[] | number[] {
  if (typeof input === "string") {
    return input.split(",");
  }
  return [...String(input)].map(Number);
}
