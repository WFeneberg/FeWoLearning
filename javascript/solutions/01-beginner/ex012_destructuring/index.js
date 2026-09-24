// Reference solution — exercise 012.

export function firstAndRest(list) {
  const [first, ...rest] = list;
  return { first, rest };
}

export function swap(pair) {
  const [a, b] = pair;
  return [b, a];
}

export function pickCoords({ x: left = 0, y: top = 0 }) {
  return { left, top };
}

export function parseOptions({ retries = 3, tag: label = "job" } = {}) {
  return { retries, label };
}

export function firstTitle(response) {
  // A default fires on undefined at every level, so one pattern covers a
  // missing `data`, a missing `items` and an empty list alike.
  const { data: { items: [{ title = "untitled" } = {}] = [] } = {} } = response ?? {};
  return title;
}
