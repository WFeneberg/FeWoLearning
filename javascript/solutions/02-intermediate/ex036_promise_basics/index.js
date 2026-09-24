// Reference solution — exercise 036.

export function deferred() {
  let resolve;
  let reject;
  // The executor runs synchronously, so both handles exist by the time the
  // constructor returns.
  const promise = new Promise((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

export function settleOnce() {
  return new Promise((resolve, reject) => {
    resolve("first");
    resolve("second");
    reject(new Error("too late"));
  });
}

export function mapResolved(promise, fn) {
  return promise.then(fn);
}

export function withFallback(promise, fallback) {
  return promise.catch(() => fallback);
}

export function runFinally(promise, onDone) {
  return promise.finally(onDone);
}
