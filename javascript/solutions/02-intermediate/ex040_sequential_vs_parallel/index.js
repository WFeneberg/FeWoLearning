// Reference solution — exercise 040.

export async function runSequential(tasks) {
  const results = [];
  for (const task of tasks) results.push(await task());
  return results;
}

export async function runParallel(tasks) {
  // map() starts every task synchronously; the awaiting happens afterwards.
  return Promise.all(tasks.map((task) => task()));
}

export async function runParallelIgnoringFailures(tasks) {
  const settled = await Promise.allSettled(tasks.map((task) => task()));
  return settled.filter((r) => r.status === "fulfilled").map((r) => r.value);
}

export async function runBoth(first, second) {
  const firstPromise = first();
  const secondPromise = second();
  return { first: await firstPromise, second: await secondPromise };
}
