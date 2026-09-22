// Reference solution — exercise 067.
export interface Bag {
  [key: string]: number;
}

// Numeric keys coerce to strings at runtime, so a string index signature
// accepts them and keyof says so.
export type BagKeys = keyof Bag;

export function readFrom(bag: Bag, key: string) {
  // number | undefined: the signature says what a key maps to if present,
  // not whether it is present.
  return bag[key];
}

export function countWords(text: string): Bag {
  const counts: Bag = {};
  for (const word of text.split(/\s+/).filter((part) => part.length > 0)) {
    counts[word] = (counts[word] ?? 0) + 1;
  }
  return counts;
}
