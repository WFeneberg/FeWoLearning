import { describe, expect, it } from "vitest";
import {
  cycleReport,
  loadByKey,
  loadRegistry,
  loadTwice,
} from "@ex/03-advanced/ex086_dynamic_import/index.js";

describe("ex086 loadRegistry", () => {
  it("resolves to the module's namespace object", async () => {
    const module = await loadRegistry();
    expect(typeof module.bump).toBe("function");
    expect(Array.isArray(module.evaluatedAt)).toBe(true);
  });

  it("returns a promise, not the module", async () => {
    const pending = loadRegistry();
    expect(pending).toBeInstanceOf(Promise);
    // Anchored: an `async` stub already returns a promise.
    expect(typeof (await pending).bump).toBe("function");
  });
});

describe("ex086 loadTwice", () => {
  it("evaluates the module once and shares its state", async () => {
    const { same, firstCall, secondCall } = await loadTwice();
    expect(same).toBe(true);
    expect(secondCall).toBe(firstCall + 1);
  });

  it("keeps counting across separate calls, because the module persists", async () => {
    const before = (await loadRegistry()).callCount();
    await loadTwice();
    expect((await loadRegistry()).callCount()).toBe(before + 2);
  });
});

describe("ex086 loadByKey", () => {
  it("loads a known module", async () => {
    expect(typeof (await loadByKey("registry")).bump).toBe("function");
    expect(typeof (await loadByKey("cycle")).fromA).toBe("function");
  });

  it("rejects an unknown key", async () => {
    await expect(loadByKey("nope")).rejects.toThrow(RangeError);
  });

  it("gives the same instance the direct loader does", async () => {
    expect(await loadByKey("registry")).toBe(await loadRegistry());
  });
});

describe("ex086 cycleReport", () => {
  it("shows a hoisted function surviving the cycle", async () => {
    // cycle-b reads cycle-a while cycle-a is still evaluating. Function
    // declarations are initialised before any of the body runs.
    expect((await cycleReport()).bSawFunction).toBe("function");
  });

  it("shows a const NOT surviving it", async () => {
    // Measured: on plain Node the same read is a ReferenceError (the
    // temporal dead zone); under Vitest's module runner it is undefined.
    // What holds in both, and is the actual lesson, is that it is not the
    // value the module finished with.
    const { bSawConst } = await cycleReport();
    expect(bSawConst).not.toBe("a-const");
    expect(["ReferenceError", undefined]).toContain(bSawConst);
  });

  it("shows the second half fully available to the first", async () => {
    expect((await cycleReport()).aSeesB).toBe("function");
  });
});
