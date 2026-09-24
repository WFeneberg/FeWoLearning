// Reference solution — exercise 097.

const REDUCED = Symbol("reduced");

export function reduced(value) {
  return { [REDUCED]: true, value };
}

export function isReduced(value) {
  return value?.[REDUCED] === true;
}

export function mapping(fn) {
  return (next) => (accumulator, value) => next(accumulator, fn(value));
}

export function filtering(predicate) {
  return (next) => (accumulator, value) =>
    predicate(value) ? next(accumulator, value) : accumulator;
}

export function taking(count) {
  return (next) => {
    // Per-run state: the reducer is built once per transduce() call.
    let taken = 0;
    return (accumulator, value) => {
      if (taken >= count) return reduced(accumulator);
      taken += 1;
      const result = next(accumulator, value);
      return taken >= count && !isReduced(result) ? reduced(result) : result;
    };
  };
}

export function compose(...transducers) {
  // Each transducer wraps the reducer built by the one to its right, which
  // is why reducing from the right gives left-to-right data flow.
  return (next) => transducers.reduceRight((current, transducer) => transducer(current), next);
}

export function transduce(transducer, reducer, initial, input) {
  const step = transducer(reducer);
  let accumulator = initial;
  for (const value of input) {
    const result = step(accumulator, value);
    if (isReduced(result)) return result.value;
    accumulator = result;
  }
  return accumulator;
}

export function into(transducer, input) {
  return transduce(
    transducer,
    (list, value) => {
      list.push(value);
      return list;
    },
    [],
    input,
  );
}
