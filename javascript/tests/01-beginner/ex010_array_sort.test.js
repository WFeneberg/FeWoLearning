import { describe, expect, it } from "vitest";
import { sortNames, sortNumbers, sortStaff } from "@ex/01-beginner/ex010_array_sort/index.js";

describe("ex010 sortNumbers", () => {
  it("sorts numerically, not lexicographically", () => {
    // The default comparison stringifies, which is where [1, 10, 9] comes from.
    expect([10, 9, 1].sort()).toEqual([1, 10, 9]);
    expect(sortNumbers([10, 9, 1])).toEqual([1, 9, 10]);
  });

  it("handles negatives and repeats", () => {
    expect(sortNumbers([3, -1, 3, 0, -10])).toEqual([-10, -1, 0, 3, 3]);
  });

  it("copies instead of reordering the input", () => {
    const input = [3, 1, 2];
    const result = sortNumbers(input);
    expect(input).toEqual([3, 1, 2]);
    expect(result).not.toBe(input);
  });
});

describe("ex010 sortStaff", () => {
  const staff = () => [
    { name: "a", dept: "ops", score: 5 },
    { name: "b", dept: "dev", score: 3 },
    { name: "c", dept: "dev", score: 9 },
    { name: "d", dept: "dev", score: 3 },
    { name: "e", dept: "ops", score: 7 },
  ];

  it("groups by dept, then descending score", () => {
    expect(sortStaff(staff()).map((person) => person.name)).toEqual(["c", "b", "d", "e", "a"]);
  });

  it("keeps equal entries in their original order (stability)", () => {
    // b and d are both dev/3. b came first in the input, so it comes first here.
    const names = sortStaff(staff()).map((person) => person.name);
    expect(names.indexOf("b")).toBeLessThan(names.indexOf("d"));
  });

  it("copies", () => {
    const input = staff();
    const result = sortStaff(input);
    expect(result).not.toBe(input);
    expect(input.map((person) => person.name)).toEqual(["a", "b", "c", "d", "e"]);
  });
});

describe("ex010 sortNames", () => {
  const names = () => ["Zoe", "Ärger", "Anna"];

  it("uses the locale it was given", () => {
    // German files Ä with A; Swedish files it after Z. Same input, same
    // code point, different answer — which is what proves the locale
    // argument actually arrived.
    expect(sortNames(names(), "de")).toEqual(["Anna", "Ärger", "Zoe"]);
    expect(sortNames(names(), "sv")).toEqual(["Anna", "Zoe", "Ärger"]);
  });

  it("copies", () => {
    const input = names();
    expect(sortNames(input, "de")).not.toBe(input);
    expect(input).toEqual(["Zoe", "Ärger", "Anna"]);
  });
});
