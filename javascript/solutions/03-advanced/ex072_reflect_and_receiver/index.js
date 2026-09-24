// Reference solution — exercise 072.

export function logReads(target, log) {
  return new Proxy(target, {
    get(object, key, receiver) {
      log.push(key);
      // The receiver is the proxy. Passing it makes a getter's `this` the
      // proxy, so what the getter reads is trapped too.
      return Reflect.get(object, key, receiver);
    },
  });
}

export function logReadsWithoutReceiver(target, log) {
  return new Proxy(target, {
    get(object, key) {
      log.push(key);
      return object[key]; // `this` inside a getter is now the raw target
    },
  });
}

export function withDefault(target, fallback) {
  return new Proxy(target, {
    get: (object, key, receiver) =>
      Reflect.has(object, key) ? Reflect.get(object, key, receiver) : fallback,
  });
}

export function allOwnKeys(target) {
  return Reflect.ownKeys(target);
}
