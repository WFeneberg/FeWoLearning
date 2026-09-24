// Reference solution — exercise 087.
import { AsyncLocalStorage } from "node:async_hooks";

// One store for the module: the context is keyed by the async execution
// chain, not by this object, so a single instance serves every caller.
const storage = new AsyncLocalStorage();

export function withContext(context, fn) {
  return storage.run(context, fn);
}

export function currentContext() {
  return storage.getStore();
}

export async function idAfterAwait() {
  await Promise.resolve();
  return currentContext()?.id;
}

export function withExtra(extra, fn) {
  return storage.run({ ...storage.getStore(), ...extra }, fn);
}
