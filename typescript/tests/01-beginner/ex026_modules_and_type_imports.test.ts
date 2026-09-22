import { beforeEach, describe, expect, it } from "vitest";
import formatLengthDefault, {
  formatLength,
  mmFrom,
  nextId,
  resetIds,
} from "@ex/01-beginner/ex026_modules_and_type_imports/index";

beforeEach(() => {
  resetIds();
});

describe("ex026 mmFrom", () => {
  it("converts centimetres", () => {
    expect(mmFrom(2.5, "cm")).toBe(25);
  });

  it("converts inches", () => {
    expect(mmFrom(1, "in")).toBe(25.4);
  });

  it("leaves millimetres alone", () => {
    expect(mmFrom(7, "mm")).toBe(7);
  });
});

describe("ex026 formatLength", () => {
  it("joins the value and the unit", () => {
    expect(formatLength(2.5, "cm")).toBe("2.5cm");
  });

  it("does not pad a whole number", () => {
    expect(formatLength(3, "cm")).toBe("3cm");
  });

  it("is reachable as the default export, and is the same function", () => {
    expect(formatLengthDefault).toBe(formatLength);
    expect(formatLengthDefault(1, "in")).toBe("1in");
  });
});

describe("ex026 module state", () => {
  it("counts up across calls", () => {
    expect([nextId(), nextId(), nextId()]).toEqual(["id-1", "id-2", "id-3"]);
  });

  it("starts again after a reset", () => {
    nextId();
    nextId();
    resetIds();
    expect(nextId()).toBe("id-1");
  });

  // The singleton fact: a second import of the same module is the same
  // module, so the counter does not start over.
  it("is shared by every importer, not copied per import", async () => {
    expect(nextId()).toBe("id-1");
    const again = await import("@ex/01-beginner/ex026_modules_and_type_imports/index");
    expect(again.nextId()).toBe("id-2");
  });
});
