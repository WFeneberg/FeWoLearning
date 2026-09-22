import { describe, expect, it } from "vitest";
import {
  alwaysFails,
  loadLabel,
  loadOrFallback,
  passThrough,
} from "@ex/01-beginner/ex031_async_await_basics/index";

describe("ex031 loadLabel", () => {
  it("labels the loaded value", async () => {
    await expect(loadLabel(async () => 42)).resolves.toBe("n=42");
  });
});

describe("ex031 passThrough", () => {
  it("resolves to the loader's value", async () => {
    await expect(passThrough(async () => 7)).resolves.toBe(7);
  });

  it("propagates the loader's rejection", async () => {
    const failing = (): Promise<number> => Promise.reject(new Error("down"));
    await expect(passThrough(failing)).rejects.toThrow("down");
  });
});

describe("ex031 loadOrFallback", () => {
  it("returns the value when the loader succeeds", async () => {
    await expect(loadOrFallback(async () => 7)).resolves.toBe(7);
  });

  it("falls back when the loader rejects", async () => {
    const failing = (): Promise<number> => Promise.reject(new Error("down"));
    await expect(loadOrFallback(failing)).resolves.toBe(-1);
  });
});

describe("ex031 alwaysFails", () => {
  // The row's real subject. A non-async body throws HERE, before the caller
  // can attach a handler; an async one hands back a rejected promise.
  it("does not throw at the call site", () => {
    let promise: Promise<never> | undefined;
    expect(() => {
      promise = alwaysFails();
    }).not.toThrow();
    // Consume the rejection so it does not surface as unhandled.
    void promise?.catch(() => undefined);
  });

  it("rejects with the error instead", async () => {
    await expect(alwaysFails()).rejects.toThrow("nope");
  });
});
