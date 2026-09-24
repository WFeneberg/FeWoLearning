// Reference solution — exercise 076.

export function firstMatching(iterator, predicate, count) {
  return iterator.filter(predicate).take(count).toArray();
}

export function sumOfSquares(iterator) {
  return iterator.map((n) => n * n).reduce((total, n) => total + n, 0);
}

export function page(iterator, offset, limit) {
  return iterator.drop(offset).take(limit).toArray();
}

export function mapIterable(iterable, fn) {
  return Iterator.from(iterable).map(fn).toArray();
}
