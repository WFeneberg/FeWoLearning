// Reference solution — exercise 058.
export function withAbort<T>(work: Promise<T>, signal: AbortSignal): Promise<T> {
  if (signal.aborted) {
    // The listener below would never fire for a signal that has already
    // aborted, so this check is not an optimisation.
    return Promise.reject(signal.reason as unknown);
  }
  return new Promise<T>((resolve, reject) => {
    signal.addEventListener("abort", () => reject(signal.reason as unknown), { once: true });
    work.then(resolve, reject);
  });
}

export async function runSequentially<T>(
  tasks: readonly (() => Promise<T>)[],
  signal: AbortSignal,
): Promise<T[]> {
  const results: T[] = [];
  for (const task of tasks) {
    // Checked BEFORE each task, so an already-aborted signal runs none.
    if (signal.aborted) {
      break;
    }
    results.push(await task());
  }
  return results;
}
