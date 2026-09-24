// Reference solution — exercise 055.

export function debounce(fn, ms) {
  let timer;
  const debounced = (...args) => {
    clearTimeout(timer);
    timer = setTimeout(() => fn(...args), ms);
  };
  debounced.cancel = () => clearTimeout(timer);
  return debounced;
}

export function throttle(fn, ms) {
  let blockedUntil = 0;
  return (...args) => {
    // Date.now() rather than a boolean plus a timer: no pending timer means
    // nothing to leak, and fake timers move the clock too.
    const now = Date.now();
    if (now < blockedUntil) return undefined;
    blockedUntil = now + ms;
    return fn(...args);
  };
}
