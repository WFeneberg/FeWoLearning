// Reference solution — exercise 039.

export function recordOrder() {
  const log = [];
  return new Promise((resolve) => {
    setTimeout(() => {
      log.push("timeout");
      resolve(log);
    }, 0);
    Promise.resolve().then(() => log.push("promise"));
    queueMicrotask(() => log.push("microtask"));
    log.push("sync");
  });
}

export function microtaskStarvation(depth) {
  const log = [];
  return new Promise((resolve) => {
    setTimeout(() => {
      log.push("timer");
      resolve(log);
    }, 0);

    let chain = Promise.resolve();
    for (let i = 0; i < depth; i++) {
      chain = chain.then(() => {
        log.push("micro");
      });
    }
  });
}

export async function afterTurns(turns) {
  for (let i = 0; i < turns; i++) await Promise.resolve();
  return turns;
}
