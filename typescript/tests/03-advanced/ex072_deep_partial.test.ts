import { describe, expect, it } from "vitest";
import { mergeDeep } from "@ex/03-advanced/ex072_deep_partial/index";
import type { DeepPartial, Profile } from "@ex/03-advanced/ex072_deep_partial/index";

const when = new Date("2026-01-01T00:00:00.000Z");

// One shared reference: a fresh arrow per call would make two otherwise
// identical bases compare unequal, since functions compare by identity.
const render = (value: string): string => value;

function base(): Profile {
  return {
    id: "p-1",
    createdAt: when,
    owner: { name: "Ada", email: "ada@example.com" },
    tags: ["a"],
    rows: [{ label: "x", count: 1 }],
    render,
  };
}

describe("ex072 mergeDeep", () => {
  it("returns the base unchanged for an empty patch", () => {
    expect(mergeDeep(base(), {} as DeepPartial<Profile>)).toEqual(base());
  });

  it("replaces a top-level value", () => {
    expect(mergeDeep(base(), { id: "p-2" } as DeepPartial<Profile>).id).toBe("p-2");
  });

  it("merges into a nested object rather than replacing it", () => {
    const merged = mergeDeep(base(), { owner: { name: "Grace" } } as DeepPartial<Profile>);
    expect(merged.owner).toEqual({ name: "Grace", email: "ada@example.com" });
  });

  it("replaces an array outright", () => {
    expect(mergeDeep(base(), { tags: ["b", "c"] } as DeepPartial<Profile>).tags).toEqual([
      "b",
      "c",
    ]);
  });

  // A Date is a leaf: merging into it would produce something that is
  // no longer a Date.
  it("replaces a Date outright and keeps it a Date", () => {
    const other = new Date("2027-06-01T00:00:00.000Z");
    const merged = mergeDeep(base(), { createdAt: other } as DeepPartial<Profile>);
    expect(merged.createdAt).toBeInstanceOf(Date);
    expect(merged.createdAt.toISOString()).toBe("2027-06-01T00:00:00.000Z");
  });

  it("does not mutate the base", () => {
    const original = base();
    mergeDeep(original, { owner: { name: "Grace" } } as DeepPartial<Profile>);
    expect(original.owner.name).toBe("Ada");
  });

  // A fact for "an explicitly undefined entry is ignored" was written and
  // dropped: under this track's exactOptionalPropertyTypes (ex005), an
  // optional `id?: string` does not accept undefined at all, so a patch
  // OMITS a key rather than setting it to undefined. The case is not
  // expressible here, which is the flag doing its job.
});
