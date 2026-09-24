// Reference solution — exercise 074.

export function share(target) {
  const { proxy, revoke } = Proxy.revocable(target, {});
  return { view: proxy, revoke };
}

export function withTemporaryAccess(resource, fn) {
  const { view, revoke } = share(resource);
  try {
    return fn(view);
  } finally {
    revoke();
  }
}

export function afterRevocation() {
  const { view, revoke } = share({ value: 1 });
  revoke();
  const attempt = (fn) => {
    try {
      fn();
      return "ok";
    } catch (error) {
      return error.name;
    }
  };
  return {
    read: attempt(() => view.value),
    write: attempt(() => {
      view.value = 2;
    }),
    typeOf: typeof view, // still "object": typeof runs no trap
    isProxyEqual: view === view,
  };
}
