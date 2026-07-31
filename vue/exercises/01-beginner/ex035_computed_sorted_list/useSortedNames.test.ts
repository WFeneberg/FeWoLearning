import { describe, expect, it } from "vitest";
import { useSortedNames } from "./useSortedNames";

describe("useSortedNames", () => {
  it("returns an alphabetically sorted copy", () => {
    const { sortedNames } = useSortedNames(["Charlie", "Alice", "Bob"]);
    expect(sortedNames.value).toEqual(["Alice", "Bob", "Charlie"]);
  });

  it("does not mutate the original names array", () => {
    const { names, sortedNames } = useSortedNames(["Charlie", "Alice", "Bob"]);
    // Force evaluation of the computed.
    expect(sortedNames.value).toEqual(["Alice", "Bob", "Charlie"]);
    expect(names.value).toEqual(["Charlie", "Alice", "Bob"]);
  });

  it("stays reactive when names changes", () => {
    const { names, sortedNames } = useSortedNames(["Zoe", "Amy"]);
    names.value = ["Zoe", "Amy", "Ben"];
    expect(sortedNames.value).toEqual(["Amy", "Ben", "Zoe"]);
    expect(names.value).toEqual(["Zoe", "Amy", "Ben"]);
  });
});
