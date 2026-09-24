// Reference solution — exercise 042.

export function withAbort(promise, signal) {
  if (signal.aborted) return Promise.reject(signal.reason);
  return new Promise((resolve, reject) => {
    const onAbortOnce = () => reject(signal.reason);
    signal.addEventListener("abort", onAbortOnce, { once: true });
    promise.then(resolve, reject).finally(() => {
      signal.removeEventListener("abort", onAbortOnce);
    });
  });
}

export function abortReasons(explicitReason) {
  const byDefault = AbortSignal.abort();
  const explicit = AbortSignal.abort(explicitReason);
  return {
    defaultName: byDefault.reason.name,
    defaultIsDomException: byDefault.reason instanceof DOMException,
    explicit: explicit.reason,
  };
}

export function onAbort(signal, fn) {
  if (signal.aborted) {
    fn(signal.reason);
    return () => undefined;
  }
  const handler = () => fn(signal.reason);
  signal.addEventListener("abort", handler, { once: true });
  return () => signal.removeEventListener("abort", handler);
}

export async function runUntilAborted(signal, step) {
  let done = 0;
  try {
    while (true) {
      signal.throwIfAborted();
      await step();
      done += 1;
    }
  } catch (error) {
    if (error !== signal.reason) throw error;
  }
  return done;
}
