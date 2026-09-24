// Reference solution — exercise 053.

export function pipe(...fns) {
  return (value) => fns.reduce((current, fn) => fn(current), value);
}

export function compose(...fns) {
  return (value) => fns.reduceRight((current, fn) => fn(current), value);
}

export function pipeAsync(...fns) {
  return async (value) => {
    let current = value;
    for (const fn of fns) current = await fn(current);
    return current;
  };
}

export function tap(fn) {
  return (value) => {
    fn(value);
    return value;
  };
}
