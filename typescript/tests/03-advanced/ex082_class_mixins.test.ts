import { describe, expect, it, vi } from "vitest";
import { Entity, Taggable, Timestamped } from "@ex/03-advanced/ex082_class_mixins/index";

// Applied lazily. At module level the untouched stub throws while this
// file is being evaluated, which takes the whole file down instead of
// failing its facts one by one.
const timestamped = (): any => Timestamped(Entity) as any;
const tagged = (): any => Taggable(Entity) as any;
const both = (): any => Taggable(Timestamped(Entity) as any) as any;

describe("ex082 Timestamped", () => {
  it("records the construction time", () => {
    vi.spyOn(Date, "now").mockReturnValue(5_000);
    try {
      expect(new (timestamped())("e-1").createdAt).toBe(5_000);
    } finally {
      vi.restoreAllMocks();
    }
  });

  it("measures age against a given clock", () => {
    vi.spyOn(Date, "now").mockReturnValue(1_000);
    try {
      expect(new (timestamped())("e-1").age(1_250)).toBe(250);
    } finally {
      vi.restoreAllMocks();
    }
  });

  it("keeps the base class's own members", () => {
    const entity = new (timestamped())("e-1");
    expect(entity.id).toBe("e-1");
    expect(entity.describe()).toBe("entity:e-1");
  });

  it("is still an Entity", () => {
    expect(new (timestamped())("e-1")).toBeInstanceOf(Entity);
  });
});

describe("ex082 Taggable", () => {
  it("starts with no tags", () => {
    expect(new (tagged())("e-1").tags).toEqual([]);
  });

  it("chains, because addTag returns this", () => {
    const entity = new (tagged())("e-1").addTag("a").addTag("b");
    expect(entity.tags).toEqual(["a", "b"]);
    expect(entity.hasTag("a")).toBe(true);
    expect(entity.hasTag("z")).toBe(false);
  });

  it("gives each instance its own list", () => {
    const first = new (tagged())("a").addTag("x");
    const second = new (tagged())("b");
    expect(second.tags).toEqual([]);
    expect(first.tags).toEqual(["x"]);
  });
});

describe("ex082 composition", () => {
  it("stacks both mixins onto the base", () => {
    vi.spyOn(Date, "now").mockReturnValue(7_000);
    try {
      const entity = new (both())("e-9").addTag("t");
      expect(entity.id).toBe("e-9");
      expect(entity.describe()).toBe("entity:e-9");
      expect(entity.createdAt).toBe(7_000);
      expect(entity.tags).toEqual(["t"]);
    } finally {
      vi.restoreAllMocks();
    }
  });
});
