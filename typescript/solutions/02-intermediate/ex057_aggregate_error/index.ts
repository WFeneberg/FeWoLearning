// Reference solution — exercise 057.
export function reasonsOf(error: unknown) {
  if (!(error instanceof AggregateError)) {
    return [];
  }
  // `errors` is any[] in the standard library, so each entry is narrowed
  // rather than trusted.
  const entries: unknown[] = error.errors;
  return entries.map((entry) => (entry instanceof Error ? entry.message : String(entry)));
}

export async function firstSuccessOrSummary(promises: readonly Promise<string>[]) {
  try {
    return await Promise.any(promises);
  } catch (caught: unknown) {
    return `none:${reasonsOf(caught).join("|")}`;
  }
}
