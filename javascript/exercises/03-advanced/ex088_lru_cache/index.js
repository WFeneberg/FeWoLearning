// Exercise 088 — an LRU cache on Map ordering (advanced).
// Goal:   use the insertion order a Map already maintains as the recency
//         list, instead of building a linked list.
// Drills: Map iteration order, delete-then-set to move an entry to the
//         end, keys().next() for the oldest, and a hit not being an
//         eviction candidate any more.
// Passes: reading a key protects it from the next eviction — which is the
//         difference between an LRU and a plain FIFO.

/**
 * A cache with a maximum size:
 *   get(key)        the value, or undefined — and marks the key as used
 *   set(key, value) inserts or updates, marking the key as used, and
 *                   evicts the least recently used entry when over `max`
 *   has(key)        without marking it as used
 *   size            how many entries are held
 *   keys()          the keys, oldest first
 *
 * TODO: implement over a single Map. A Map iterates in insertion order,
 * and re-inserting a key moves it to the end.
 */
export function createLruCache(_max) {
  throw new Error("TODO: implement createLruCache");
}
