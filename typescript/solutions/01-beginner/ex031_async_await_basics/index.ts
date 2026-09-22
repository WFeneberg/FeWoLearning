// Reference solution — exercise 031.
export async function loadLabel(load: () => Promise<number>) {
  return `n=${await load()}`;
}

export async function passThrough(load: () => Promise<number>) {
  // Returning a Promise from an async function flattens: the result is
  // Promise<number>, not Promise<Promise<number>>.
  return load();
}

export async function loadOrFallback(load: () => Promise<number>) {
  try {
    return await load();
  } catch {
    return -1;
  }
}

// `async` is the whole point: the throw becomes a rejected promise.
export async function alwaysFails(): Promise<never> {
  throw new Error("nope");
}
