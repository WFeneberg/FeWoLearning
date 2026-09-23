// Reference solution — exercise 087.
export interface GroupResult<T> {
  values: (T | undefined)[];
  failure: string | undefined;
}

export async function runGroup<T>(
  tasks: readonly ((signal: AbortSignal) => Promise<T>)[],
): Promise<GroupResult<T>> {
  const controller = new AbortController();
  let failure: string | undefined;

  // Started together, not awaited in turn: this is what makes the group
  // concurrent rather than serial.
  const values = await Promise.all(
    tasks.map(async (task): Promise<T | undefined> => {
      try {
        return await task(controller.signal);
      } catch (caught: unknown) {
        failure ??= caught instanceof Error ? caught.message : String(caught);
        // Abort the siblings, then let this one resolve as undefined.
        // Rethrowing would reject the Promise.all and return before the
        // others had wound down, which is the leak this row is about.
        controller.abort(caught);
        return undefined;
      }
    }),
  );

  return { values, failure };
}
