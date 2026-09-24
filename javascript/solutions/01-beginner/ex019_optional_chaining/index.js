// Reference solution — exercise 019.

export function cityOf(user) {
  return user?.address?.city ?? "unknown";
}

export function notify(target, payload) {
  // `?.()` skips the call when the property is null/undefined, and still
  // throws for a property that exists but is not callable.
  return target?.onEvent?.(payload);
}

export function readKey(source, keyFn) {
  return source?.[keyFn()];
}

export function itemCount(order) {
  return order?.items?.length ?? 0;
}
