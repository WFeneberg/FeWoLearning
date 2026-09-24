// Reference solution — exercise 018.

export function invokeAs(fn, receiver, args) {
  return fn.apply(receiver, args);
}

export function bindMethod(object, name) {
  return object[name].bind(object);
}

export function thisWhenCalledBare() {
  function probe() {
    return this;
  }
  return probe();
}

export function borrowSlice(arrayLike) {
  return Array.prototype.slice.call(arrayLike);
}
