// Reference solution — exercise 033.
export async function loadAll(
  a: Promise<string>,
  b: Promise<number>,
  c: Promise<boolean>,
) {
  // The array literal is what makes this a tuple. Collecting the three into
  // a variable first would infer Promise<string | number | boolean>[] and
  // the positions would blur into a union.
  return Promise.all([a, b, c]);
}

export async function loadAllOrMessage(
  promises: readonly Promise<number>[],
): Promise<number[] | string> {
  try {
    return await Promise.all(promises);
  } catch (caught: unknown) {
    return `failed:${caught instanceof Error ? caught.message : String(caught)}`;
  }
}
