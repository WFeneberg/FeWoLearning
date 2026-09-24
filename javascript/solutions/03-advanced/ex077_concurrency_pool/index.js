// Reference solution — exercise 077.

async function run(items, limit, handle) {
  const results = new Array(items.length);
  let cursor = 0;
  const runner = async () => {
    while (cursor < items.length) {
      const index = cursor++; // claimed before the await, so no two runners share it
      results[index] = await handle(items[index], index);
    }
  };
  const runners = Array.from({ length: Math.min(limit, items.length) }, runner);
  await Promise.all(runners);
  return results;
}

export function mapWithConcurrency(items, limit, worker) {
  return run(items, limit, (item, index) => worker(item, index));
}

export function settleWithConcurrency(items, limit, worker) {
  return run(items, limit, async (item, index) => {
    try {
      return { status: "fulfilled", value: await worker(item, index) };
    } catch (reason) {
      return { status: "rejected", reason };
    }
  });
}
