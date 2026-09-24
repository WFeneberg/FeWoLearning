// Reference solution — exercise 038.

export function collectAll(promises) {
  return Promise.all(promises);
}

export function collectSettled(promises) {
  return Promise.allSettled(promises).then((results) => ({
    fulfilled: results.filter((r) => r.status === "fulfilled").map((r) => r.value),
    rejected: results.filter((r) => r.status === "rejected").map((r) => r.reason),
  }));
}

export function firstSettled(promises) {
  return Promise.race(promises);
}

export function firstSuccess(promises) {
  return Promise.any(promises);
}
