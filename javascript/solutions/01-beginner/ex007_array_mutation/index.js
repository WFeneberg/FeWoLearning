// Reference solution — exercise 007.

export function appendCopy(list, value) {
  return [...list, value];
}

export function removeAt(list, index) {
  if (index < 0 || index >= list.length) return [...list];
  return list.toSpliced(index, 1);
}

export function insertAt(list, index, value) {
  return list.toSpliced(index, 0, value);
}

export function drainInto(target, source) {
  for (const value of source) target.push(value);
  return target;
}

export function takeFirst(list, count) {
  return list.slice(0, count);
}
