import { describe, expect, it } from "vitest";
import {
  countByKey,
  groupByKey,
  groupToMap,
  groupWithReduce,
} from "@ex/02-intermediate/ex058_group_by/index.js";

const users = () => [
  { name: "ada", role: "admin", age: 36 },
  { name: "linus", role: "user", age: 54 },
  { name: "grace", role: "admin", age: 45 },
];

const byRole = (user) => user.role;

describe("ex058 groupByKey", () => {
  it("groups into buckets, in first-seen key order", () => {
    const groups = groupByKey(users(), byRole);
    expect(Object.keys(groups)).toEqual(["admin", "user"]);
    expect(groups.admin.map((user) => user.name)).toEqual(["ada", "grace"]);
    expect(groups.user.map((user) => user.name)).toEqual(["linus"]);
  });

  it("returns an object with NO prototype", () => {
    // Measured: this is why `expect(groups).toEqual({ admin: […] })` fails
    // even when the contents match. Compare entries, or assert per key.
    const groups = groupByKey(users(), byRole);
    expect(Object.getPrototypeOf(groups)).toBeNull();
    expect(groups.hasOwnProperty).toBeUndefined();
  });

  it("stringifies its keys", () => {
    const groups = groupByKey([1, 2, 3, 4], (n) => n % 2 === 0);
    expect(Object.keys(groups).toSorted()).toEqual(["false", "true"]);
  });

  it("keeps the original objects", () => {
    const list = users();
    expect(groupByKey(list, byRole).admin[0]).toBe(list[0]);
  });
});

describe("ex058 groupToMap", () => {
  it("groups into a Map", () => {
    const groups = groupToMap(users(), byRole);
    expect(groups).toBeInstanceOf(Map);
    expect(groups.get("admin")).toHaveLength(2);
    expect([...groups.keys()]).toEqual(["admin", "user"]);
  });

  it("keeps non-string keys as they are", () => {
    const groups = groupToMap([1, 2, 3, 4], (n) => n % 2 === 0);
    expect(groups.get(true)).toEqual([2, 4]);
    expect(groups.get(false)).toEqual([1, 3]);
    expect(groups.get("true")).toBeUndefined();
  });
});

describe("ex058 groupWithReduce", () => {
  it("matches the built-in", () => {
    const byHand = groupWithReduce(users(), byRole);
    const builtIn = groupToMap(users(), byRole);
    expect([...byHand.keys()]).toEqual([...builtIn.keys()]);
    expect(byHand.get("admin").map((u) => u.name)).toEqual(
      builtIn.get("admin").map((u) => u.name),
    );
  });

  it("handles an empty list", () => {
    expect(groupWithReduce([], byRole).size).toBe(0);
  });

  it("does not lose the first item of a bucket", () => {
    // The classic off-by-one here: initialising the bucket without the item
    // that created it.
    expect(groupWithReduce([{ role: "solo" }], byRole).get("solo")).toHaveLength(1);
  });
});

describe("ex058 countByKey", () => {
  it("counts per key", () => {
    const counts = countByKey(users(), byRole);
    expect(counts).toBeInstanceOf(Map);
    expect(counts.get("admin")).toBe(2);
    expect(counts.get("user")).toBe(1);
  });

  it("is empty for no items", () => {
    expect(countByKey([], byRole).size).toBe(0);
  });
});
