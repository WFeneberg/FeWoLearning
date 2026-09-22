// Reference solution — exercise 056.
export async function partition(promises: readonly Promise<number>[]) {
  // allSettled, not all: `all` discards every other outcome as soon as one
  // input rejects, and both halves are wanted here.
  const settled = await Promise.allSettled(promises);
  const values: number[] = [];
  const errors: string[] = [];
  for (const outcome of settled) {
    if (outcome.status === "fulfilled") {
      values.push(outcome.value);
    } else {
      const reason: unknown = outcome.reason;
      errors.push(reason instanceof Error ? reason.message : String(reason));
    }
  }
  return { values, errors };
}

export async function firstSettled(promises: readonly Promise<string>[]): Promise<string> {
  try {
    return `ok:${await Promise.race(promises)}`;
  } catch (caught: unknown) {
    return `fail:${caught instanceof Error ? caught.message : String(caught)}`;
  }
}

export async function firstSuccess(promises: readonly Promise<string>[]): Promise<string> {
  return Promise.any(promises);
}
