import { describe, expect, it } from "vitest";
import { LABELS, assertNever, labelOf, statusOf } from "@ex/02-intermediate/ex055_assert_never/index";

describe("ex055 LABELS", () => {
  it("has an entry for every kind and no others", () => {
    expect(LABELS).toEqual({ draft: "Draft", published: "Published", archived: "Archived" });
  });
});

describe("ex055 labelOf", () => {
  it("looks each kind up", () => {
    expect(labelOf("draft")).toBe("Draft");
    expect(labelOf("published")).toBe("Published");
    expect(labelOf("archived")).toBe("Archived");
  });
});

describe("ex055 statusOf", () => {
  it("answers for every kind", () => {
    expect(statusOf("draft")).toBe("not yet");
    expect(statusOf("published")).toBe("live");
    expect(statusOf("archived")).toBe("gone");
  });
});

describe("ex055 assertNever", () => {
  it("throws, naming the value", () => {
    expect(() => assertNever("surprise" as never)).toThrow(/^Unexpected value: /);
  });
});
