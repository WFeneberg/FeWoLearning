// Reference solution — exercise 095.

export function pipeline(source, ...stages) {
  return stages.reduce((current, stage) => stage(current), source);
}

export function mapStage(fn) {
  return async function* map(input) {
    for await (const value of input) yield await fn(value);
  };
}

export function filterStage(predicate) {
  return async function* filter(input) {
    for await (const value of input) {
      if (await predicate(value)) yield value;
    }
  };
}

export function traceStage(name, log) {
  return async function* trace(input) {
    log.push(`${name}:start`);
    try {
      for await (const value of input) {
        log.push(`${name}:${value}`);
        yield value;
      }
    } finally {
      // Runs on normal completion and on an early exit alike: breaking
      // out of the consumer calls return() on this generator.
      log.push(`${name}:end`);
    }
  };
}

export async function collectLimit(asyncIterable, limit) {
  const values = [];
  if (limit <= 0) return values;
  for await (const value of asyncIterable) {
    values.push(value);
    if (values.length >= limit) break; // closes the chain behind us
  }
  return values;
}
