import { describe, expect, it } from "vitest";
import {
  countWords,
  groupToMap,
  objectKeyCollision,
  uniqueBy,
} from "@ex/01-beginner/ex022_map_and_set/index.js";

describe("ex022 countWords", () => {
  it("counts into a real Map", () => {
    const counts = countWords(["a", "b", "a"]);
    expect(counts).toBeInstanceOf(Map);
    expect(counts.get("a")).toBe(2);
    expect(counts.get("b")).toBe(1);
    expect(counts.size).toBe(2);
  });

  it("keeps first-seen order", () => {
    expect([...countWords(["z", "a", "z"]).keys()]).toEqual(["z", "a"]);
  });

  it("is empty for no words, and reports undefined rather than 0 for a miss", () => {
    expect(countWords([]).size).toBe(0);
    expect(countWords(["a"]).get("b")).toBeUndefined();
  });

  it("handles keys a plain object would have inherited", () => {
    // On a plain object, obj["constructor"] is already a function before
    // anyone writes to it. A Map starts genuinely empty.
    const counts = countWords(["constructor", "__proto__", "constructor"]);
    expect(counts.get("constructor")).toBe(2);
    expect(counts.get("__proto__")).toBe(1);
  });
});

describe("ex022 uniqueBy", () => {
  it("keeps the first of each key", () => {
    const items = [
      { id: 1, tag: "first" },
      { id: 1, tag: "second" },
      { id: 2, tag: "third" },
    ];
    expect(uniqueBy(items, (item) => item.id).map((item) => item.tag)).toEqual([
      "first",
      "third",
    ]);
  });

  it("returns the original objects, not copies", () => {
    const items = [{ id: 1 }];
    expect(uniqueBy(items, (item) => item.id)[0]).toBe(items[0]);
  });

  it("handles an empty list", () => {
    expect(uniqueBy([], (item) => item)).toEqual([]);
  });
});

describe("ex022 groupToMap", () => {
  it("groups in first-seen key order", () => {
    const groups = groupToMap(["apple", "avocado", "banana"], (word) => word[0]);
    expect(groups).toBeInstanceOf(Map);
    expect([...groups.keys()]).toEqual(["a", "b"]);
    expect(groups.get("a")).toEqual(["apple", "avocado"]);
    expect(groups.get("b")).toEqual(["banana"]);
  });

  it("can group on a non-string key", () => {
    const groups = groupToMap([1, 2, 3, 4], (n) => n % 2 === 0);
    expect(groups.get(true)).toEqual([2, 4]);
    expect(groups.get(false)).toEqual([1, 3]);
  });
});

describe("ex022 objectKeyCollision", () => {
  it("shows the object collapsing 1 and \"1\" into one property", () => {
    const result = objectKeyCollision();
    expect(result.objectAtNumber).toBe("string key");
    expect(result.objectAtString).toBe("string key");
  });

  it("shows the Map keeping them apart", () => {
    const result = objectKeyCollision();
    expect(result.mapAtNumber).toBe("number key");
    expect(result.mapAtString).toBe("string key");
  });
});
