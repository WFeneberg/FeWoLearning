// Reference solution — exercise 022.

export function countWords(words) {
  const counts = new Map();
  for (const word of words) counts.set(word, (counts.get(word) ?? 0) + 1);
  return counts;
}

export function uniqueBy(items, keyFn) {
  const seen = new Set();
  const kept = [];
  for (const item of items) {
    const key = keyFn(item);
    if (seen.has(key)) continue;
    seen.add(key);
    kept.push(item);
  }
  return kept;
}

export function groupToMap(items, keyFn) {
  const groups = new Map();
  for (const item of items) {
    const key = keyFn(item);
    const bucket = groups.get(key);
    if (bucket === undefined) groups.set(key, [item]);
    else bucket.push(item);
  }
  return groups;
}

export function objectKeyCollision() {
  const object = {};
  object[1] = "number key";
  object["1"] = "string key"; // same property — the number was stringified

  const map = new Map();
  map.set(1, "number key");
  map.set("1", "string key");

  return {
    objectAtNumber: object[1],
    objectAtString: object["1"],
    mapAtNumber: map.get(1),
    mapAtString: map.get("1"),
  };
}
