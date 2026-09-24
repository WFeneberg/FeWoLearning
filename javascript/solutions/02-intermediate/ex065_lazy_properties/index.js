// Reference solution — exercise 065.

export function defineLazy(object, key, compute) {
  return Object.defineProperty(object, key, {
    configurable: true, // required: the getter has to redefine itself
    enumerable: true,
    get() {
      const value = compute();
      Object.defineProperty(this, key, {
        value,
        writable: true,
        enumerable: true,
        configurable: true,
      });
      return value;
    },
  });
}

export function makeConfig(load) {
  const config = {
    get loadedYet() {
      // A data property means it has been materialised; an accessor means
      // the getter is still in place.
      const descriptor = Object.getOwnPropertyDescriptor(this, "settings");
      return descriptor?.get === undefined;
    },
  };
  return defineLazy(config, "settings", load);
}
