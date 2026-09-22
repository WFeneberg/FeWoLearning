// Reference solution — exercise 007.
export function at<T>(items: readonly T[], index: number) {
  // The inferred return type is T | undefined. A trailing `!` here would
  // infer plain T and quietly reintroduce the lie.
  return items[index];
}

export function firstOr(items: readonly string[], fallback: string): string {
  return items[0] ?? fallback;
}

export function sumAt(values: readonly number[], indices: readonly number[]): number {
  let total = 0;
  for (const index of indices) {
    total += values[index] ?? 0;
  }
  return total;
}
