// Reference solution — exercise 094.

export async function runGroup(tasks, options = {}) {
  const controller = new AbortController();
  const stopOuter = options.signal
    ? (() => {
        const onAbort = () => controller.abort(options.signal.reason);
        if (options.signal.aborted) onAbort();
        else options.signal.addEventListener("abort", onAbort, { once: true });
        return () => options.signal.removeEventListener("abort", onAbort);
      })()
    : () => undefined;

  let firstError;
  const settled = await Promise.allSettled(
    tasks.map(async (task) => {
      try {
        return await task(controller.signal);
      } catch (error) {
        // The first real failure wins and cancels the siblings; their own
        // abort errors arrive later and must not overwrite it.
        firstError ??= error;
        controller.abort(error);
        throw error;
      }
    }),
  );
  stopOuter();

  if (firstError !== undefined) throw firstError;
  return settled.map((result) => result.value);
}

export async function runFirst(tasks) {
  const controller = new AbortController();
  let winner;
  let won = false;
  const errors = [];

  await Promise.allSettled(
    tasks.map(async (task) => {
      try {
        const value = await task(controller.signal);
        if (!won) {
          won = true;
          winner = value;
          controller.abort(new Error("another task already succeeded"));
        }
      } catch (error) {
        errors.push(error);
      }
    }),
  );

  if (won) return winner;
  throw new AggregateError(errors, "every task failed");
}
