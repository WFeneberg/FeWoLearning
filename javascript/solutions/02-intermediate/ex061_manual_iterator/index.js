// Reference solution — exercise 061.

export function countdown(from) {
  return {
    [Symbol.iterator]() {
      let current = from;
      return {
        next() {
          if (current <= 0) return { value: undefined, done: true };
          return { value: current--, done: false };
        },
      };
    },
  };
}

export function closeable(values, onClose) {
  return {
    [Symbol.iterator]() {
      let index = 0;
      let closed = false;
      return {
        next() {
          if (index >= values.length) return { value: undefined, done: true };
          return { value: values[index++], done: false };
        },
        return(value) {
          // for..of calls this on an early exit. It must report done, or
          // the loop keeps asking.
          if (!closed) {
            closed = true;
            onClose();
          }
          return { value, done: true };
        },
      };
    },
  };
}

export function chain(...iterables) {
  return {
    [Symbol.iterator]() {
      let outer = 0;
      let inner = null;
      return {
        next() {
          while (outer < iterables.length) {
            inner ??= iterables[outer][Symbol.iterator]();
            const result = inner.next();
            if (!result.done) return result;
            inner = null;
            outer += 1;
          }
          return { value: undefined, done: true };
        },
      };
    },
  };
}
