// Reference solution — exercise 034.
import * as deps from "./deps.js";
import scale, { count, increment } from "./deps.js";

export function readCount() {
  // `count` is a live binding into deps.js, not a copy taken at import time.
  return count;
}

export function bumpTwice() {
  increment();
  increment();
  return count;
}

export function sumThenScale(a, b, factor) {
  return scale(deps.add(a, b), factor);
}

export function namespaceKeys() {
  return Object.keys(deps).toSorted();
}

export { add } from "./deps.js";
