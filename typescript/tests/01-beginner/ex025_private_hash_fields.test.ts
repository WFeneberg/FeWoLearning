import { describe, expect, it } from "vitest";
import {
  HardSecret,
  SoftSecret,
  visibleFields,
} from "@ex/01-beginner/ex025_private_hash_fields/index";

describe("ex025 visibleFields", () => {
  it("sees a `private` field, because private is erased", () => {
    expect(visibleFields(new SoftSecret("hunter2"))).toEqual(["value"]);
  });

  it("does not see a `#` field, because the runtime enforces it", () => {
    expect(visibleFields(new HardSecret("hunter2"))).toEqual([]);
  });
});

describe("ex025 HardSecret", () => {
  it("reveals the value it was given", () => {
    expect(new HardSecret("hunter2").reveal()).toBe("hunter2");
  });

  it("serialises to an empty object", () => {
    expect(JSON.stringify(new HardSecret("hunter2"))).toBe("{}");
  });

  it("keeps separate values per instance", () => {
    const a = new HardSecret("a");
    const b = new HardSecret("b");
    expect([a.reveal(), b.reveal()]).toEqual(["a", "b"]);
  });
});

describe("ex025 isHardSecret", () => {
  it("recognises its own instances", () => {
    expect(HardSecret.isHardSecret(new HardSecret("x"))).toBe(true);
  });

  it("rejects a plain object", () => {
    expect(HardSecret.isHardSecret({})).toBe(false);
  });

  it("rejects a different class with the same surface", () => {
    expect(HardSecret.isHardSecret(new SoftSecret("x"))).toBe(false);
  });

  it("rejects an impostor with a forged prototype", () => {
    // instanceof would say true here; the brand check cannot be fooled.
    const impostor = Object.create(HardSecret.prototype) as unknown;
    expect(impostor instanceof HardSecret).toBe(true);
    expect(HardSecret.isHardSecret(impostor)).toBe(false);
  });

  it("rejects primitives and null without throwing", () => {
    expect(HardSecret.isHardSecret(null)).toBe(false);
    expect(HardSecret.isHardSecret("x")).toBe(false);
    expect(HardSecret.isHardSecret(7)).toBe(false);
  });
});
