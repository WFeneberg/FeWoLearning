// Reference solution — exercise 086.

const loaders = {
  registry: () => import("./registry.js"),
  cycle: () => import("./cycle-a.js"),
};

export async function loadRegistry() {
  return import("./registry.js");
}

export async function loadTwice() {
  const first = await import("./registry.js");
  const second = await import("./registry.js");
  return { same: first === second, firstCall: first.bump(), secondCall: second.bump() };
}

export async function loadByKey(key) {
  const load = loaders[key];
  if (load === undefined) throw new RangeError(`unknown module: ${key}`);
  return load();
}

export async function cycleReport() {
  const { report } = await import("./cycle-a.js");
  return report;
}
