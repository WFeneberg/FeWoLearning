// Reference solution — exercise 092.

const RAW = Symbol("raw");

export function reactive(target, onChange) {
  // One cache per root, so the same child object always yields the same
  // proxy — otherwise `state.user === state.user` would be false.
  const proxies = new WeakMap();

  const wrap = (value, path) => {
    if (value === null || typeof value !== "object") return value;
    const existing = proxies.get(value);
    if (existing !== undefined) return existing;
    const proxy = build(value, path);
    proxies.set(value, proxy);
    return proxy;
  };

  const build = (object, path) =>
    new Proxy(object, {
      get(current, key, receiver) {
        if (key === RAW) return current;
        const value = Reflect.get(current, key, receiver);
        if (typeof key === "symbol") return value;
        return wrap(value, [...path, key]);
      },
      set(current, key, value, receiver) {
        const previous = current[key];
        const same = previous === value || (previous !== previous && value !== value);
        const result = Reflect.set(current, key, toRaw(value), receiver);
        if (!same) onChange({ path: [...path, key], value, previous });
        return result;
      },
      deleteProperty(current, key) {
        const had = Object.hasOwn(current, key);
        const previous = current[key];
        const result = Reflect.deleteProperty(current, key);
        if (had) onChange({ path: [...path, key], value: undefined, previous });
        return result;
      },
    });

  return wrap(target, []);
}

export function toRaw(value) {
  if (value === null || typeof value !== "object") return value;
  return value[RAW] ?? value;
}
