import { describe, expect, it } from "vitest";
import {
  agreeAboutValue,
  hideNonConfigurableKey,
  lieAboutExtensible,
  lieAboutValue,
} from "@ex/03-advanced/ex073_proxy_invariants/index.js";

describe("ex073 invariants", () => {
  it("refuses a get trap that contradicts a locked property", () => {
    expect(lieAboutValue()).toBe("TypeError");
  });

  it("allows the same trap when it tells the truth", () => {
    // The invariant is about the ANSWER, not about having a trap.
    expect(agreeAboutValue()).toBe("real");
  });

  it("refuses to hide a non-configurable key from ownKeys", () => {
    expect(hideNonConfigurableKey()).toBe("TypeError");
  });

  it("refuses to call a non-extensible target extensible", () => {
    expect(lieAboutExtensible()).toBe("TypeError");
  });
});
