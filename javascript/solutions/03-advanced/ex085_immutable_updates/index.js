// Reference solution — exercise 085.

const shallowCopy = (value, key) => {
  if (Array.isArray(value)) return [...value];
  if (value !== null && typeof value === "object") return { ...value };
  // Nothing to copy: create the container the next key implies.
  return typeof key === "number" ? [] : {};
};

export function setIn(object, path, value) {
  if (path.length === 0) return value;
  const [key, ...rest] = path;
  const copy = shallowCopy(object, key);
  // Only the child on the path is rebuilt; every sibling is carried over
  // by the spread above, identity intact.
  copy[key] = setIn(object?.[key], rest, value);
  return copy;
}

export function updateIn(object, path, fn) {
  return setIn(object, path, fn(getIn(object, path, undefined)));
}

export function getIn(object, path, fallback) {
  let current = object;
  for (const key of path) {
    if (current === null || current === undefined) return fallback;
    current = current[key];
  }
  return current === undefined ? fallback : current;
}

export function removeIn(object, path) {
  if (path.length === 0) return object;
  const [key, ...rest] = path;
  const copy = shallowCopy(object, key);
  if (rest.length === 0) {
    if (Array.isArray(copy)) copy.splice(key, 1);
    else delete copy[key];
    return copy;
  }
  copy[key] = removeIn(object?.[key], rest);
  return copy;
}
