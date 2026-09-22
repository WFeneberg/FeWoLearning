// Reference solution — exercise 029.
export function tryRun(fn: () => string): string {
  try {
    return fn();
  } catch (caught: unknown) {
    if (caught instanceof Error) {
      return `error:${caught.message}`;
    }
    if (typeof caught === "string") {
      return `thrown:${caught}`;
    }
    return "unknown";
  }
}

export function toError(value: unknown) {
  if (value instanceof Error) {
    return value;
  }
  // Keeping the original as `cause` means nothing is lost to String().
  return new Error(String(value), { cause: value });
}
