import { describe, expect, it } from "vitest";
import { createLruCache } from "@ex/03-advanced/ex088_lru_cache/index.js";

describe("ex088 createLruCache", () => {
  it("stores and reads", () => {
    const cache = createLruCache(2);
    cache.set("a", 1);
    expect(cache.get("a")).toBe(1);
    expect(cache.get("missing")).toBeUndefined();
    expect(cache.size).toBe(1);
  });

  it("evicts the oldest when it overflows", () => {
    const cache = createLruCache(2);
    cache.set("a", 1);
    cache.set("b", 2);
    cache.set("c", 3);
    expect(cache.size).toBe(2);
    expect(cache.has("a")).toBe(false);
    expect(cache.keys()).toEqual(["b", "c"]);
  });

  it("protects a key that was READ — the LRU part", () => {
    // A FIFO cache evicts "a" here. An LRU evicts "b", because "a" was
    // used more recently.
    const cache = createLruCache(2);
    cache.set("a", 1);
    cache.set("b", 2);
    cache.get("a");
    cache.set("c", 3);
    expect(cache.has("a")).toBe(true);
    expect(cache.has("b")).toBe(false);
  });

  it("protects a key that was WRITTEN again", () => {
    const cache = createLruCache(2);
    cache.set("a", 1);
    cache.set("b", 2);
    cache.set("a", 11);
    cache.set("c", 3);
    expect(cache.get("a")).toBe(11);
    expect(cache.has("b")).toBe(false);
  });

  it("does not grow when an existing key is updated", () => {
    const cache = createLruCache(2);
    cache.set("a", 1);
    cache.set("a", 2);
    expect(cache.size).toBe(1);
    expect(cache.keys()).toEqual(["a"]);
  });

  it("does not mark a key as used through has()", () => {
    const cache = createLruCache(2);
    cache.set("a", 1);
    cache.set("b", 2);
    cache.has("a");
    cache.set("c", 3);
    expect(cache.has("a")).toBe(false);
  });

  it("reports keys oldest first", () => {
    const cache = createLruCache(3);
    cache.set("a", 1);
    cache.set("b", 2);
    cache.set("c", 3);
    cache.get("a");
    expect(cache.keys()).toEqual(["b", "c", "a"]);
  });

  it("holds a cached undefined without treating it as a miss", () => {
    const cache = createLruCache(2);
    cache.set("a", undefined);
    expect(cache.has("a")).toBe(true);
    expect(cache.size).toBe(1);
  });

  it("works with a max of 1", () => {
    const cache = createLruCache(1);
    cache.set("a", 1);
    cache.set("b", 2);
    expect(cache.keys()).toEqual(["b"]);
  });

  it("keys on identity for objects", () => {
    const cache = createLruCache(2);
    const key = {};
    cache.set(key, "value");
    expect(cache.get(key)).toBe("value");
    expect(cache.get({})).toBeUndefined();
  });
});
