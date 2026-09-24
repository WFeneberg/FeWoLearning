// Reference solution — exercise 073.

function lockedTarget() {
  return Object.defineProperty({}, "fixed", {
    value: "real",
    writable: false,
    configurable: false,
  });
}

function attempt(fn) {
  try {
    return fn();
  } catch (error) {
    return error.name;
  }
}

export function lieAboutValue() {
  const view = new Proxy(lockedTarget(), { get: () => "fake" });
  return attempt(() => view.fixed);
}

export function agreeAboutValue() {
  const view = new Proxy(lockedTarget(), { get: () => "real" });
  return attempt(() => view.fixed);
}

export function hideNonConfigurableKey() {
  const view = new Proxy(lockedTarget(), { ownKeys: () => [] });
  return attempt(() => Object.getOwnPropertyNames(view));
}

export function lieAboutExtensible() {
  const view = new Proxy(Object.preventExtensions({}), { isExtensible: () => true });
  return attempt(() => Object.isExtensible(view));
}
