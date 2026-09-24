// Reference solution — exercise 052.

export function curry3(fn) {
  return (a) => (b) => (c) => fn(a, b, c);
}

export function curry(fn) {
  const collect =
    (...collected) =>
    (...next) => {
      const all = [...collected, ...next];
      return all.length >= fn.length ? fn(...all) : collect(...all);
    };
  return collect();
}

export function partial(fn, ...preset) {
  // bind also fixes up length: (fn.length - preset.length), floored at 0.
  return fn.bind(undefined, ...preset);
}

export function partialRight(fn, ...preset) {
  return (...args) => fn(...args, ...preset);
}
