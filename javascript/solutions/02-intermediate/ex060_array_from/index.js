// Reference solution — exercise 060.

export function sequence(count) {
  return Array.from({ length: count }, (_unused, index) => index);
}

export function fromArrayLike(arrayLike) {
  return Array.from(arrayLike);
}

export function codePoints(text) {
  // Array.from iterates a string by CODE POINT, so an emoji stays whole.
  return Array.from(text, (character) => character.codePointAt(0));
}

export function arrayOf(...values) {
  return Array.of(...values);
}

export function holesVsUndefined() {
  return {
    holes: new Array(3).map(() => 1),
    filled: Array.from({ length: 3 }).map(() => 1),
  };
}
