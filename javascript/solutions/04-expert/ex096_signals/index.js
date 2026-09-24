// Reference solution — exercise 096.

// Whatever is currently computing. A read registers itself here, which is
// the whole of "automatic dependency tracking".
let active = null;
let batchDepth = 0;
const pending = new Set();

const notify = (subscribers) => {
  for (const subscriber of [...subscribers]) {
    if (batchDepth > 0) pending.add(subscriber);
    else subscriber();
  }
};

export function signal(initial) {
  let value = initial;
  const subscribers = new Set();
  return {
    get() {
      if (active !== null) subscribers.add(active);
      return value;
    },
    peek: () => value,
    set(next) {
      if (value === next || (value !== value && next !== next)) return;
      value = next;
      notify(subscribers);
    },
  };
}

export function computed(fn) {
  const subscribers = new Set();
  let cache;
  let stale = true;

  const invalidate = () => {
    if (stale) return;
    stale = true;
    notify(subscribers);
  };

  const read = () => {
    if (stale) {
      const previous = active;
      active = invalidate; // the dependencies subscribe THIS, not the reader
      try {
        cache = fn();
        stale = false;
      } finally {
        active = previous;
      }
    }
    return cache;
  };

  return {
    get() {
      if (active !== null) subscribers.add(active);
      return read();
    },
    peek: read,
  };
}

export function effect(fn) {
  let stopped = false;
  const run = () => {
    if (stopped) return;
    const previous = active;
    active = run;
    try {
      fn();
    } finally {
      active = previous;
    }
  };
  run();
  return () => {
    stopped = true;
  };
}

export function batch(fn) {
  batchDepth += 1;
  try {
    return fn();
  } finally {
    batchDepth -= 1;
    if (batchDepth === 0) {
      const queued = [...pending];
      pending.clear();
      for (const subscriber of queued) subscriber();
    }
  }
}
